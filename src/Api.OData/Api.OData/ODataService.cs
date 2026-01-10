using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;

namespace Api.OData;

/// <summary>
/// Supports OData query options for IQueryable and IEnumerable collections.
/// <para>Order:</para>
/// <list type="number">
/// <item>$apply</item>
/// <item>$filter</item>
/// <item>$orderby</item>
/// <item>$skip</item>
/// <item>$top</item>
/// <item>$select</item>
/// </list>
/// <para>$count and $expand are ignored by default (configurable via <see cref="ODataOptions"/>).</para>
/// </summary>
public sealed class ODataService : IODataService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEdmModel _edmModel;
    private readonly ODataQuerySettings _oDataQuerySettings;

    /// <summary>
    /// Initializes a new instance of the ODataService.
    /// </summary>
    public ODataService(IHttpContextAccessor httpContextAccessor, IEdmModel edmModel, IOptions<ODataOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _edmModel = edmModel;
        _oDataQuerySettings = BuildQuerySettings(options.Value);
    }

    private static ODataQuerySettings BuildQuerySettings(ODataOptions options)
    {
        var ignoredOptions = AllowedQueryOptions.None;

        if (options.IgnoreExpand)
            ignoredOptions |= AllowedQueryOptions.Expand;

        if (options.IgnoreCount)
            ignoredOptions |= AllowedQueryOptions.Count;

        return new ODataQuerySettings
        {
            PageSize = options.PageSize,
            IgnoredQueryOptions = ignoredOptions,
            HandleNullPropagation = options.HandleNullPropagation
        };
    }

    /// <inheritdoc />
    public IQueryable<T> ApplyODataQuery<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();

        return (IQueryable<T>)queryOptions.ApplyTo(query, _oDataQuerySettings);
    }

    /// <inheritdoc />
    public IQueryable<T> ApplyODataFilter<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.Filter is null)
        {
            return query;
        }

        return (IQueryable<T>)queryOptions.Filter.ApplyTo(query, _oDataQuerySettings);
    }

    /// <inheritdoc />
    public IQueryable<T> ApplyODataOrderBy<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.OrderBy is null)
        {
            return query;
        }

        return queryOptions.OrderBy.ApplyTo(query, _oDataQuerySettings);
    }

    /// <inheritdoc />
    public IQueryable<T> ApplyODataPagination<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();

        var resultQuery = query;
        if (queryOptions.Skip is not null)
        {
            resultQuery = queryOptions.Skip.ApplyTo(query, _oDataQuerySettings);
        }

        if (queryOptions.Top is not null)
        {
            resultQuery = queryOptions.Top.ApplyTo(resultQuery, _oDataQuerySettings);
        }

        return resultQuery;
    }

    /// <inheritdoc />
    public IQueryable<T> ApplyODataSelect<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.SelectExpand is null)
        {
            return query;
        }

        return (IQueryable<T>)queryOptions.SelectExpand.ApplyTo(query, _oDataQuerySettings);
    }

    /// <inheritdoc />
    public IQueryable<T> ApplyODataApply<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.Apply is null)
        {
            return query;
        }

        return (IQueryable<T>)queryOptions.Apply.ApplyTo(query, _oDataQuerySettings);
    }

    private ODataQueryOptions<T> GetQueryOptions<T>()
    {
        var odataContext = new ODataQueryContext(_edmModel, typeof(T), path: null);
        return new ODataQueryOptions<T>(odataContext, _httpContextAccessor.HttpContext!.Request);
    }
}
