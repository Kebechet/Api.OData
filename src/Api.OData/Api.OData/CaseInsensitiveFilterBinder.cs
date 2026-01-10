using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.UriParser;

namespace Api.OData;

/// <summary>
/// Custom FilterBinder that applies case-insensitive collation to string comparisons.
/// </summary>
public class CaseInsensitiveFilterBinder : FilterBinder
{
    private readonly string _collation;

    private static readonly MethodInfo CollateMethod = typeof(RelationalDbFunctionsExtensions)
        .GetMethod(nameof(RelationalDbFunctionsExtensions.Collate), [typeof(DbFunctions), typeof(string), typeof(string)])!;

    /// <summary>
    /// Initializes a new instance of CaseInsensitiveFilterBinder with the specified collation.
    /// </summary>
    /// <param name="collation">The collation to use for case-insensitive comparisons (e.g., "NOCASE" for SQLite).</param>
    public CaseInsensitiveFilterBinder(string collation)
    {
        _collation = collation;
    }

    /// <inheritdoc />
    public override Expression BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode, QueryBinderContext context)
    {
        var left = Bind(binaryOperatorNode.Left, context);
        var right = Bind(binaryOperatorNode.Right, context);

        var isStringComparison = left.Type == typeof(string) && right.Type == typeof(string);
        var isEqualityOperation = binaryOperatorNode.OperatorKind is
            BinaryOperatorKind.Equal or
            BinaryOperatorKind.NotEqual;

        if (isStringComparison && isEqualityOperation)
        {
            var functions = Expression.Constant(EF.Functions);
            var collation = Expression.Constant(_collation);

            left = Expression.Call(CollateMethod, functions, left, collation);
            right = Expression.Call(CollateMethod, functions, right, collation);

            return binaryOperatorNode.OperatorKind == BinaryOperatorKind.Equal
                ? Expression.Equal(left, right)
                : Expression.NotEqual(left, right);
        }

        return base.BindBinaryOperatorNode(binaryOperatorNode, context);
    }
}
