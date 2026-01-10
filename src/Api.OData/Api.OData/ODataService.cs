using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.Edm;

namespace Api.OData;

/// <summary>
/// Supports OData query options for IQueryable & IEnumerable collections. Order:
/// 1) $apply
/// 2) $filter
/// 3) $orderby
/// 4) $skip
/// 5) $top
/// 6) $select
/// Ignored ones: $count, $expand
/// </summary>
public sealed class ODataService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEdmModel _edmModel;
    private readonly ODataQuerySettings _oDataQuerySettings = new()
    {
        PageSize = null,
        IgnoredQueryOptions = AllowedQueryOptions.Expand | AllowedQueryOptions.Count,
        HandleNullPropagation = HandleNullPropagationOption.False
    };

    public ODataService(IHttpContextAccessor httpContextAccessor, IEdmModel edmModel)
    {
        _httpContextAccessor = httpContextAccessor;
        _edmModel = edmModel;
    }

    /// <summary>
    /// Applies OData query options to the provided IQueryable collection in order: $filter, $orderby, $skip, $top, $select.
    /// </summary>
    public IQueryable<T> ApplyODataQuery<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();

        return (IQueryable<T>)queryOptions.ApplyTo(query, _oDataQuerySettings);
    }

    /// <summary>
    /// Applies OData $filter query option to the provided IQueryable collection.
    /// </summary>
    public IQueryable<T> ApplyODataFilter<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.Filter is null)
        {
            return query;
        }

        return (IQueryable<T>)queryOptions.Filter.ApplyTo(query, _oDataQuerySettings);
    }

    /// <summary>
    /// Applies OData $orderby query option to the provided IQueryable collection.
    /// </summary>
    public IQueryable<T> ApplyODataOrderBy<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.OrderBy is null)
        {
            return query;
        }

        return queryOptions.OrderBy.ApplyTo(query, _oDataQuerySettings);
    }

    /// <summary>
    /// Applies OData $skip and $top query options to the provided IQueryable collection.
    /// </summary>
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

    /// <summary>
    /// Applies OData $select query option to the provided IQueryable collection.
    /// </summary>
    public IQueryable<T> ApplyODataSelect<T>(IQueryable<T> query)
    {
        var queryOptions = GetQueryOptions<T>();
        if (queryOptions.SelectExpand is null)
        {
            return query;
        }

        return (IQueryable<T>)queryOptions.SelectExpand.ApplyTo(query, _oDataQuerySettings);
    }

    /// <summary>
    /// Applies OData $apply query option to the provided IQueryable collection.
    /// </summary>
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
