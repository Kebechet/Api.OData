using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Expressions;
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
    private readonly ODataOptions _options;

    /// <summary>
    /// Initializes a new instance of the ODataService.
    /// </summary>
    public ODataService(IHttpContextAccessor httpContextAccessor, IEdmModel edmModel, IOptions<ODataOptions> options)
    {
        _httpContextAccessor = httpContextAccessor;
        _edmModel = edmModel;
        _options = options.Value;
        _oDataQuerySettings = BuildQuerySettings(_options);
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
    public IQueryable ApplyODataQuery<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();

        return (IQueryable)queryOptions.ApplyTo(query, _oDataQuerySettings);
    }

    /// <inheritdoc />
    public IQueryable<T> ApplyODataQueryWithoutProjection<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        var settingsWithoutProjection = new ODataQuerySettings
        {
            PageSize = _oDataQuerySettings.PageSize,
            IgnoredQueryOptions = _oDataQuerySettings.IgnoredQueryOptions
                | AllowedQueryOptions.Select
                | AllowedQueryOptions.Apply,
            HandleNullPropagation = _oDataQuerySettings.HandleNullPropagation,
        };

        return (IQueryable<T>)queryOptions.ApplyTo(query, settingsWithoutProjection);
    }

    /// <inheritdoc />
    public IQueryable<T> ApplyODataFilter<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.Filter is null)
        {
            return query;
        }

        if (_options.EnableCaseInsensitiveFilter)
        {
            var filterBinder = new CaseInsensitiveFilterBinder(_options.CaseInsensitiveCollation);
            var context = new QueryBinderContext(_edmModel, _oDataQuerySettings, typeof(T));
            var filterExpression = filterBinder.BindFilter(queryOptions.Filter.FilterClause, context);
            return query.Where((Expression<Func<T, bool>>)filterExpression);
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
    public IQueryable ApplyODataSelect<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.SelectExpand is null)
        {
            return query;
        }

        return queryOptions.SelectExpand.ApplyTo(query, _oDataQuerySettings);
    }

    /// <inheritdoc />
    public IQueryable ApplyODataApply<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.Apply is null)
        {
            return query;
        }

        return queryOptions.Apply.ApplyTo(query, _oDataQuerySettings);
    }

    private ODataQueryOptions<T> GetQueryOptions<T>()
    {
        var odataContext = new ODataQueryContext(_edmModel, typeof(T), path: null);
        return new ODataQueryOptions<T>(odataContext, _httpContextAccessor.HttpContext!.Request);
    }
}
