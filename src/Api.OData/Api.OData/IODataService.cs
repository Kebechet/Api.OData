namespace Api.OData;

/// <summary>
/// Interface for OData query service, enabling mocking and testing.
/// </summary>
public interface IODataService
{
    /// <summary>
    /// Applies all OData query options to the provided IQueryable collection.
    /// </summary>
    /// <remarks>
    /// Returns non-generic <see cref="IQueryable"/> because <c>$select</c> reshapes the projection
    /// into <c>SelectSome&lt;T&gt;</c>/<c>SelectAllAndExpand&lt;T&gt;</c>/<c>SelectAll&lt;T&gt;</c> wrappers
    /// whose element type is no longer <typeparamref name="T"/>. When no <c>$select</c> is present
    /// the element type at runtime is still <typeparamref name="T"/>, so callers may cast with
    /// <see cref="Queryable.Cast{TResult}"/> if needed.
    /// Use <see cref="ApplyODataQueryWithoutSelect{T}"/> when callers do not expose <c>$select</c>
    /// and want a strongly-typed result.
    /// </remarks>
    IQueryable ApplyODataQuery<T>(IQueryable<T> query);

    /// <summary>
    /// Applies <c>$apply</c>, <c>$filter</c>, <c>$orderby</c>, <c>$skip</c>, and <c>$top</c> to the
    /// provided IQueryable collection. <c>$select</c> in the request URL is silently ignored, so the
    /// element type is preserved as <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// Prefer this overload when the endpoint does not expose <c>$select</c> to clients — it removes
    /// the need to cast back from non-generic <see cref="IQueryable"/>.
    /// </remarks>
    IQueryable<T> ApplyODataQueryWithoutSelect<T>(IQueryable<T> query);

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
    /// <remarks>
    /// Returns non-generic <see cref="IQueryable"/> because <c>$select</c> projects into
    /// <c>SelectSome&lt;T&gt;</c>/<c>SelectAllAndExpand&lt;T&gt;</c>/<c>SelectAll&lt;T&gt;</c> wrappers
    /// whose element type is no longer <typeparamref name="T"/>. When no <c>$select</c> is present
    /// the original <paramref name="query"/> is returned unchanged.
    /// </remarks>
    IQueryable ApplyODataSelect<T>(IQueryable<T> query);

    /// <summary>
    /// Applies OData $apply query option to the provided IQueryable collection.
    /// </summary>
    IQueryable<T> ApplyODataApply<T>(IQueryable<T> query);
}
