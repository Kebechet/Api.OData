namespace Api.OData;

/// <summary>
/// Interface for OData query service, enabling mocking and testing.
/// </summary>
public interface IODataService
{
    /// <summary>
    /// Applies all OData query options to the provided IQueryable collection.
    /// </summary>
    IQueryable<T> ApplyODataQuery<T>(IQueryable<T> query);

    /// <summary>
    /// Applies OData $filter query option to the provided IQueryable collection.
    /// </summary>
    IQueryable<T> ApplyODataFilter<T>(IQueryable<T> query);

    /// <summary>
    /// Applies OData $orderby query option to the provided IQueryable collection.
    /// </summary>
    IQueryable<T> ApplyODataOrderBy<T>(IQueryable<T> query);

    /// <summary>
    /// Applies OData $skip and $top query options to the provided IQueryable collection.
    /// </summary>
    IQueryable<T> ApplyODataPagination<T>(IQueryable<T> query);

    /// <summary>
    /// Applies OData $select query option to the provided IQueryable collection.
    /// </summary>
    IQueryable<T> ApplyODataSelect<T>(IQueryable<T> query);

    /// <summary>
    /// Applies OData $apply query option to the provided IQueryable collection.
    /// </summary>
    IQueryable<T> ApplyODataApply<T>(IQueryable<T> query);
}
