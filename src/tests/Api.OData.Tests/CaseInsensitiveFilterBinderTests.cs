using FluentAssertions;
using Xunit;

namespace Api.OData.Tests;

public class CaseInsensitiveFilterBinderTests
{
    [Fact]
    public void Constructor_WithCollation_CreatesInstance()
    {
        // Arrange
        var collation = "NOCASE";

        // Act
        var binder = new CaseInsensitiveFilterBinder(collation);

        // Assert
        binder.Should().NotBeNull();
    }

    [Theory]
    [InlineData("NOCASE")]
    [InlineData("Latin1_General_CI_AS")]
    [InlineData("SQL_Latin1_General_CP1_CI_AS")]
    public void Constructor_WithVariousCollations_CreatesInstance(string collation)
    {
        // Arrange & Act
        var binder = new CaseInsensitiveFilterBinder(collation);

        // Assert
        binder.Should().NotBeNull();
    }
}
