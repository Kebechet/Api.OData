using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Xunit;

namespace Api.OData.Tests;

public class ODataServiceIntegrationTests
{
    private readonly IEdmModel _edmModel;

    public ODataServiceIntegrationTests()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<TestProduct>("Products");
        _edmModel = builder.GetEdmModel();
    }

    private ODataService CreateService(string queryString, Action<ODataOptions>? configureOptions = null)
    {
        var options = new ODataOptions();
        configureOptions?.Invoke(options);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString(queryString);

        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

        return new ODataService(
            httpContextAccessor,
            _edmModel,
            Options.Create(options));
    }

    private static IQueryable<TestProduct> GetTestProducts() =>
        new List<TestProduct>
        {
            new() { Id = 1, Name = "Apple", Price = 1.50m, Category = "Fruit" },
            new() { Id = 2, Name = "Banana", Price = 0.75m, Category = "Fruit" },
            new() { Id = 3, Name = "Carrot", Price = 0.50m, Category = "Vegetable" },
            new() { Id = 4, Name = "Date", Price = 2.00m, Category = "Fruit" },
            new() { Id = 5, Name = "Eggplant", Price = 1.25m, Category = "Vegetable" },
        }.AsQueryable();

    [Fact]
    public void ApplyODataFilter_WithEqualityFilter_ReturnsMatchingItems()
    {
        // Arrange
        var service = CreateService("?$filter=Category eq 'Fruit'");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataFilter(query).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(p => p.Category == "Fruit");
    }

    [Fact]
    public void ApplyODataFilter_WithNumericComparison_ReturnsMatchingItems()
    {
        // Arrange
        var service = CreateService("?$filter=Price gt 1.00");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataFilter(query).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(p => p.Price > 1.00m);
    }

    [Fact]
    public void ApplyODataFilter_WithContainsFunction_ReturnsMatchingItems()
    {
        // Arrange
        var service = CreateService("?$filter=contains(Name, 'a')");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataFilter(query).ToList();

        // Assert
        // lowercase 'a' matches: Banana, Carrot, Date, Eggplant (not Apple - has uppercase 'A')
        result.Should().HaveCount(4);
        result.Select(p => p.Name).Should().Contain(new[] { "Banana", "Carrot", "Date", "Eggplant" });
    }

    [Fact]
    public void ApplyODataFilter_WithNoFilter_ReturnsAllItems()
    {
        // Arrange
        var service = CreateService("");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataFilter(query).ToList();

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public void ApplyODataOrderBy_WithAscending_ReturnsSortedAscending()
    {
        // Arrange
        var service = CreateService("?$orderby=Name");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataOrderBy(query).ToList();

        // Assert
        result.Select(p => p.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public void ApplyODataOrderBy_WithDescending_ReturnsSortedDescending()
    {
        // Arrange
        var service = CreateService("?$orderby=Price desc");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataOrderBy(query).ToList();

        // Assert
        result.Select(p => p.Price).Should().BeInDescendingOrder();
    }

    [Fact]
    public void ApplyODataOrderBy_WithNoOrderBy_RetainsOriginalOrder()
    {
        // Arrange
        var service = CreateService("");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataOrderBy(query).ToList();

        // Assert
        result.Select(p => p.Id).Should().ContainInOrder(1, 2, 3, 4, 5);
    }

    [Fact]
    public void ApplyODataPagination_WithSkip_SkipsSpecifiedItems()
    {
        // Arrange
        var service = CreateService("?$skip=2");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataPagination(query).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.First().Id.Should().Be(3);
    }

    [Fact]
    public void ApplyODataPagination_WithTop_LimitsResultCount()
    {
        // Arrange
        var service = CreateService("?$top=2");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataPagination(query).ToList();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyODataPagination_WithSkipAndTop_ReturnsCorrectPage()
    {
        // Arrange
        var service = CreateService("?$skip=1&$top=2");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataPagination(query).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Select(p => p.Id).Should().ContainInOrder(2, 3);
    }

    [Fact]
    public void ApplyODataQuery_WithMultipleOptions_AppliesAllCorrectly()
    {
        // Arrange
        var service = CreateService("?$filter=Category eq 'Fruit'&$orderby=Price desc&$top=2");
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataQuery(query).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Category == "Fruit");
        result.Select(p => p.Price).Should().BeInDescendingOrder();
    }

    [Fact]
    public void ApplyODataFilter_WithLowercaseValue_ReturnsNoMatchByDefault()
    {
        // Arrange
        var service = CreateService("?$filter=Name eq 'apple'"); // lowercase 'apple'
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataFilter(query).ToList();

        // Assert
        // Default is case-sensitive, so 'apple' won't match 'Apple'
        result.Should().BeEmpty();
    }

    [Fact]
    public void ApplyODataFilter_WithExactCase_ReturnsMatch()
    {
        // Arrange
        var service = CreateService("?$filter=Name eq 'Apple'"); // exact case
        var query = GetTestProducts();

        // Act
        var result = service.ApplyODataFilter(query).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Apple");
    }

    [Fact]
    public void ApplyODataFilter_WithCaseInsensitiveEnabled_ReturnsMatchIgnoringCase()
    {
        // Arrange
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        using var dbContext = new TestDbContext(dbContextOptions);
        dbContext.Database.EnsureCreated();
        dbContext.Products.AddRange(
            new TestProduct { Id = 1, Name = "Apple", Price = 1.50m, Category = "Fruit" },
            new TestProduct { Id = 2, Name = "Banana", Price = 0.75m, Category = "Fruit" }
        );
        dbContext.SaveChanges();

        var service = CreateService(
            "?$filter=Name eq 'apple'",
            options =>
            {
                options.EnableCaseInsensitiveFilter = true;
                options.CaseInsensitiveCollation = "NOCASE";
            });

        // Act
        var result = service.ApplyODataFilter(dbContext.Products).ToList();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Apple");

        connection.Close();
    }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<TestProduct> Products => Set<TestProduct>();
}

public class TestProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}
