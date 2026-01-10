using FluentAssertions;
using Microsoft.AspNetCore.OData.Query;
using Xunit;

namespace Api.OData.Tests;

public class ODataOptionsTests
{
    [Fact]
    public void Constructor_WithDefaults_SetsCorrectValues()
    {
        // Arrange & Act
        var options = new ODataOptions();

        // Assert
        options.PageSize.Should().BeNull();
        options.IgnoreExpand.Should().BeTrue();
        options.IgnoreCount.Should().BeTrue();
        options.HandleNullPropagation.Should().Be(HandleNullPropagationOption.False);
        options.EnableCaseInsensitiveFilter.Should().BeFalse();
        options.CaseInsensitiveCollation.Should().Be("Latin1_General_CI_AS");
    }

    [Fact]
    public void PageSize_WhenSet_ReturnsValue()
    {
        // Arrange
        var options = new ODataOptions();

        // Act
        options.PageSize = 100;

        // Assert
        options.PageSize.Should().Be(100);
    }

    [Fact]
    public void IgnoreExpand_WhenSetToFalse_ReturnsFalse()
    {
        // Arrange
        var options = new ODataOptions();

        // Act
        options.IgnoreExpand = false;

        // Assert
        options.IgnoreExpand.Should().BeFalse();
    }

    [Fact]
    public void IgnoreCount_WhenSetToFalse_ReturnsFalse()
    {
        // Arrange
        var options = new ODataOptions();

        // Act
        options.IgnoreCount = false;

        // Assert
        options.IgnoreCount.Should().BeFalse();
    }

    [Fact]
    public void EnableCaseInsensitiveFilter_WhenSetToTrue_ReturnsTrue()
    {
        // Arrange
        var options = new ODataOptions();

        // Act
        options.EnableCaseInsensitiveFilter = true;

        // Assert
        options.EnableCaseInsensitiveFilter.Should().BeTrue();
    }

    [Fact]
    public void CaseInsensitiveCollation_WhenChanged_ReturnsNewValue()
    {
        // Arrange
        var options = new ODataOptions();

        // Act
        options.CaseInsensitiveCollation = "NOCASE";

        // Assert
        options.CaseInsensitiveCollation.Should().Be("NOCASE");
    }
}
