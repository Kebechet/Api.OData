namespace Api.OData.Extensions;

public static class ODataExtensions
{
    // IQueryable
    public static IQueryable<T> ApplyODataQuery<T>(this IQueryable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataQuery(query);
    }

    public static IQueryable<T> ApplyODataFilter<T>(this IQueryable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataFilter(query);
    }

    public static IQueryable<T> ApplyODataOrderBy<T>(this IQueryable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataOrderBy(query);
    }

    public static IQueryable<T> ApplyODataPagination<T>(this IQueryable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataPagination(query);
    }

    public static IQueryable<T> ApplyODataSelect<T>(this IQueryable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataSelect(query);
    }

    public static IQueryable<T> ApplyODataApply<T>(this IQueryable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        if (!isEnabled)
        {
            return query;
        }

        return oDataService.ApplyODataApply(query);
    }

    // IEnumerable
    public static IEnumerable<T> ApplyODataQuery<T>(this IEnumerable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataQuery(oDataService, isEnabled);
    }

    public static IEnumerable<T> ApplyODataFilter<T>(this IEnumerable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataFilter(oDataService, isEnabled);
    }

    public static IEnumerable<T> ApplyODataOrderBy<T>(this IEnumerable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataOrderBy(oDataService, isEnabled);
    }

    public static IEnumerable<T> ApplyODataPagination<T>(this IEnumerable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataPagination(oDataService, isEnabled);
    }

    public static IEnumerable<T> ApplyODataSelect<T>(this IEnumerable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataSelect(oDataService, isEnabled);
    }

    public static IEnumerable<T> ApplyODataApply<T>(this IEnumerable<T> query, ODataService oDataService, bool isEnabled = true)
    {
        return query.AsQueryable().ApplyODataApply(oDataService, isEnabled);
    }
}
