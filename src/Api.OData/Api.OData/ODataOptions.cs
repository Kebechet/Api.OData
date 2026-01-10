using Microsoft.AspNetCore.OData.Query;

namespace Api.OData;

/// <summary>
/// Configuration options for OData query processing.
/// </summary>
public class ODataOptions
{
    /// <summary>
    /// Maximum page size for query results. Null means no limit.
    /// </summary>
    public int? PageSize { get; set; } = null;

    /// <summary>
    /// When true, $expand query option is ignored. Default is true.
    /// </summary>
    public bool IgnoreExpand { get; set; } = true;

    /// <summary>
    /// When true, $count query option is ignored. Default is true.
    /// </summary>
    public bool IgnoreCount { get; set; } = true;

    /// <summary>
    /// Specifies how null propagation should be handled during query composition.
    /// Default is False (no null checks added).
    /// </summary>
    public HandleNullPropagationOption HandleNullPropagation { get; set; } = HandleNullPropagationOption.False;
}
