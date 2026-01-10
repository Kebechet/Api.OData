using Api.OData.Extensions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Api.OData.Tests;

public class ODataExtensionsTests
{
    private readonly Mock<IODataService> _mockODataService;

    public ODataExtensionsTests()
    {
        _mockODataService = new Mock<IODataService>();
    }

    [Fact]
    public void ApplyODataQuery_WhenDisabled_ReturnsOriginalQuery()
    {
        // Arrange
        var query = new List<string> { "a", "b", "c" }.AsQueryable();

        // Act
        var result = query.ApplyODataQuery(_mockODataService.Object, isEnabled: false);

        // Assert
        result.Should().BeSameAs(query);
        _mockODataService.Verify(x => x.ApplyODataQuery(It.IsAny<IQueryable<string>>()), Times.Never);
    }

    [Fact]
    public void ApplyODataQuery_WhenEnabled_CallsService()
    {
        // Arrange
        var query = new List<string> { "a", "b", "c" }.AsQueryable();
        var expectedResult = new List<string> { "a" }.AsQueryable();
        _mockODataService.Setup(x => x.ApplyODataQuery(query)).Returns(expectedResult);

        // Act
        var result = query.ApplyODataQuery(_mockODataService.Object, isEnabled: true);

        // Assert
        result.Should().BeSameAs(expectedResult);
        _mockODataService.Verify(x => x.ApplyODataQuery(query), Times.Once);
    }

    [Fact]
    public void ApplyODataFilter_WhenDisabled_ReturnsOriginalQuery()
    {
        // Arrange
        var query = new List<string> { "a", "b", "c" }.AsQueryable();

        // Act
        var result = query.ApplyODataFilter(_mockODataService.Object, isEnabled: false);

        // Assert
        result.Should().BeSameAs(query);
        _mockODataService.Verify(x => x.ApplyODataFilter(It.IsAny<IQueryable<string>>()), Times.Never);
    }

    [Fact]
    public void ApplyODataFilter_WhenEnabled_CallsService()
    {
        // Arrange
        var query = new List<string> { "a", "b", "c" }.AsQueryable();
        var expectedResult = new List<string> { "a" }.AsQueryable();
        _mockODataService.Setup(x => x.ApplyODataFilter(query)).Returns(expectedResult);

        // Act
        var result = query.ApplyODataFilter(_mockODataService.Object, isEnabled: true);

        // Assert
        result.Should().BeSameAs(expectedResult);
        _mockODataService.Verify(x => x.ApplyODataFilter(query), Times.Once);
    }

    [Fact]
    public void ApplyODataOrderBy_WhenDisabled_ReturnsOriginalQuery()
    {
        // Arrange
        var query = new List<string> { "a", "b", "c" }.AsQueryable();

        // Act
        var result = query.ApplyODataOrderBy(_mockODataService.Object, isEnabled: false);

        // Assert
        result.Should().BeSameAs(query);
        _mockODataService.Verify(x => x.ApplyODataOrderBy(It.IsAny<IQueryable<string>>()), Times.Never);
    }

    [Fact]
    public void ApplyODataPagination_WhenDisabled_ReturnsOriginalQuery()
    {
        // Arrange
        var query = new List<string> { "a", "b", "c" }.AsQueryable();

        // Act
        var result = query.ApplyODataPagination(_mockODataService.Object, isEnabled: false);

        // Assert
        result.Should().BeSameAs(query);
        _mockODataService.Verify(x => x.ApplyODataPagination(It.IsAny<IQueryable<string>>()), Times.Never);
    }

    [Fact]
    public void ApplyODataSelect_WhenDisabled_ReturnsOriginalQuery()
    {
        // Arrange
        var query = new List<string> { "a", "b", "c" }.AsQueryable();

        // Act
        var result = query.ApplyODataSelect(_mockODataService.Object, isEnabled: false);

        // Assert
        result.Should().BeSameAs(query);
        _mockODataService.Verify(x => x.ApplyODataSelect(It.IsAny<IQueryable<string>>()), Times.Never);
    }

    [Fact]
    public void ApplyODataApply_WhenDisabled_ReturnsOriginalQuery()
    {
        // Arrange
        var query = new List<string> { "a", "b", "c" }.AsQueryable();

        // Act
        var result = query.ApplyODataApply(_mockODataService.Object, isEnabled: false);

        // Assert
        result.Should().BeSameAs(query);
        _mockODataService.Verify(x => x.ApplyODataApply(It.IsAny<IQueryable<string>>()), Times.Never);
    }

    [Fact]
    public void ApplyODataQuery_OnIEnumerable_WhenDisabled_ReturnsOriginalItems()
    {
        // Arrange
        var enumerable = new List<string> { "a", "b", "c" };

        // Act
        var result = enumerable.ApplyODataQuery(_mockODataService.Object, isEnabled: false);

        // Assert
        result.Should().BeEquivalentTo(enumerable);
        _mockODataService.Verify(x => x.ApplyODataQuery(It.IsAny<IQueryable<string>>()), Times.Never);
    }

    [Fact]
    public void ApplyODataQuery_WithDefaultIsEnabled_CallsService()
    {
        // Arrange
        var query = new List<string> { "a", "b", "c" }.AsQueryable();
        var expectedResult = new List<string> { "a" }.AsQueryable();
        _mockODataService.Setup(x => x.ApplyODataQuery(query)).Returns(expectedResult);

        // Act
        var result = query.ApplyODataQuery(_mockODataService.Object);

        // Assert
        _mockODataService.Verify(x => x.ApplyODataQuery(query), Times.Once);
    }
}
