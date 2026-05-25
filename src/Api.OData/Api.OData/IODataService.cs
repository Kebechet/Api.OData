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
    /// Returns non-generic <see cref="IQueryable"/> because <c>$select</c> and <c>$apply</c> project
    /// rows into wrapper types (<c>SelectSome&lt;T&gt;</c>/<c>SelectAllAndExpand&lt;T&gt;</c>/<c>SelectAll&lt;T&gt;</c>
    /// for <c>$select</c>, <c>DynamicTypeWrapper</c> for <c>$apply</c> aggregations) whose element
    /// type is no longer <typeparamref name="T"/>. When neither is present the element type at
    /// runtime is still <typeparamref name="T"/>, so callers may cast with
    /// <see cref="Queryable.Cast{TResult}"/> if needed. Use
    /// <see cref="ApplyODataQueryWithoutProjection{T}"/> when callers do not expose <c>$select</c>
    /// or <c>$apply</c> and want a strongly-typed result.
    /// </remarks>
    IQueryable ApplyODataQuery<T>(IQueryable<T> query);

    /// <summary>
    /// Applies <c>$filter</c>, <c>$orderby</c>, <c>$skip</c>, and <c>$top</c> to the provided
    /// IQueryable collection. The projecting options <c>$select</c> and <c>$apply</c> are silently
    /// ignored, so the element type is preserved as <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// Prefer this overload when the endpoint does not expose <c>$select</c>/<c>$apply</c> to
    /// clients — it removes the need to cast back from non-generic <see cref="IQueryable"/>.
    /// </remarks>
    IQueryable<T> ApplyODataQueryWithoutProjection<T>(IQueryable<T> query);

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
    /// <remarks>
    /// Returns non-generic <see cref="IQueryable"/> because aggregation transforms project into
    /// <c>DynamicTypeWrapper</c> whose element type is no longer <typeparamref name="T"/>. When no
    /// <c>$apply</c> is present the original <paramref name="query"/> is returned unchanged.
    /// </remarks>
    IQueryable ApplyODataApply<T>(IQueryable<T> query);
}
