using System.Collections;

namespace Api.OData.Extensions;

/// <summary>
/// Extension methods for applying OData query options to IQueryable and IEnumerable collections.
/// </summary>
public static class ODataExtensions
{
    // IQueryable

    /// <summary>
    /// Applies all OData query options to the provided IQueryable collection.
    /// </summary>
    /// <remarks>
    /// Returns non-generic <see cref="IQueryable"/> because <c>$select</c> reshapes the element
    /// type into OData wrapper types. When no <c>$select</c> is present the element type at
    /// runtime is still <typeparamref name="T"/>. Use
    /// <see cref="ApplyODataQueryWithoutSelect{T}(IQueryable{T}, IODataService, bool)"/>
    /// when the endpoint does not expose <c>$select</c>.
    /// </remarks>
    public static IQueryable ApplyODataQuery<T>(this IQueryable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataQuery(query);
    }

    /// <summary>
    /// Applies <c>$apply</c>, <c>$filter</c>, <c>$orderby</c>, <c>$skip</c>, and <c>$top</c> to the
    /// provided IQueryable collection. <c>$select</c> in the request URL is silently ignored, so the
    /// element type is preserved as <typeparamref name="T"/>.
    /// </summary>
    public static IQueryable<T> ApplyODataQueryWithoutSelect<T>(this IQueryable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataQueryWithoutSelect(query);
    }

    /// <summary>
    /// Applies OData $filter query option to the provided IQueryable collection.
    /// </summary>
    public static IQueryable<T> ApplyODataFilter<T>(this IQueryable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataFilter(query);
    }

    /// <summary>
    /// Applies OData $orderby query option to the provided IQueryable collection.
    /// </summary>
    public static IQueryable<T> ApplyODataOrderBy<T>(this IQueryable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataOrderBy(query);
    }

    /// <summary>
    /// Applies OData $skip and $top query options to the provided IQueryable collection.
    /// </summary>
    public static IQueryable<T> ApplyODataPagination<T>(this IQueryable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataPagination(query);
    }

    /// <summary>
    /// Applies OData $select query option to the provided IQueryable collection.
    /// </summary>
    /// <remarks>
    /// Returns non-generic <see cref="IQueryable"/> because <c>$select</c> projects into OData
    /// wrapper types whose element type is no longer <typeparamref name="T"/>.
    /// </remarks>
    public static IQueryable ApplyODataSelect<T>(this IQueryable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataSelect(query);
    }

    /// <summary>
    /// Applies OData $apply query option to the provided IQueryable collection.
    /// </summary>
    public static IQueryable<T> ApplyODataApply<T>(this IQueryable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataApply(query);
    }

    // IEnumerable

    /// <summary>
    /// Applies all OData query options to the provided IEnumerable collection.
    /// </summary>
    /// <remarks>
    /// Returns non-generic <see cref="IEnumerable"/> because <c>$select</c> reshapes the element
    /// type into OData wrapper types. When no <c>$select</c> is present the element type at
    /// runtime is still <typeparamref name="T"/>. Use
    /// <see cref="ApplyODataQueryWithoutSelect{T}(IEnumerable{T}, IODataService, bool)"/>
    /// when the endpoint does not expose <c>$select</c>.
    /// </remarks>
    public static IEnumerable ApplyODataQuery<T>(this IEnumerable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataQuery(oDataService, isEnabled);
    }

    /// <summary>
    /// Applies <c>$apply</c>, <c>$filter</c>, <c>$orderby</c>, <c>$skip</c>, and <c>$top</c> to the
    /// provided IEnumerable collection. <c>$select</c> in the request URL is silently ignored, so the
    /// element type is preserved as <typeparamref name="T"/>.
    /// </summary>
    public static IEnumerable<T> ApplyODataQueryWithoutSelect<T>(this IEnumerable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataQueryWithoutSelect(oDataService, isEnabled);
    }

    /// <summary>
    /// Applies OData $filter query option to the provided IEnumerable collection.
    /// </summary>
    public static IEnumerable<T> ApplyODataFilter<T>(this IEnumerable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataFilter(oDataService, isEnabled);
    }

    /// <summary>
    /// Applies OData $orderby query option to the provided IEnumerable collection.
    /// </summary>
    public static IEnumerable<T> ApplyODataOrderBy<T>(this IEnumerable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataOrderBy(oDataService, isEnabled);
    }

    /// <summary>
    /// Applies OData $skip and $top query options to the provided IEnumerable collection.
    /// </summary>
    public static IEnumerable<T> ApplyODataPagination<T>(this IEnumerable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataPagination(oDataService, isEnabled);
    }

    /// <summary>
    /// Applies OData $select query option to the provided IEnumerable collection.
    /// </summary>
    /// <remarks>
    /// Returns non-generic <see cref="IEnumerable"/> because <c>$select</c> projects into OData
    /// wrapper types whose element type is no longer <typeparamref name="T"/>.
    /// </remarks>
    public static IEnumerable ApplyODataSelect<T>(this IEnumerable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataSelect(oDataService, isEnabled);
    }

    /// <summary>
    /// Applies OData $apply query option to the provided IEnumerable collection.
    /// </summary>
    public static IEnumerable<T> ApplyODataApply<T>(this IEnumerable<T> query, IODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataApply(oDataService, isEnabled);
    }
}
