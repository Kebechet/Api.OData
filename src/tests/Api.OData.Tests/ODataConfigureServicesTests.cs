using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Xunit;

namespace Api.OData.Tests;

public class ODataConfigureServicesTests
{
    [Fact]
    public void AddOData_WhenCalled_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOData(builder => { });
        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<IEdmModel>().Should().NotBeNull();
        provider.GetService<IODataService>().Should().NotBeNull();
    }

    [Fact]
    public void AddOData_WhenCalled_RegistersEdmModelAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddOData(builder => { });
        var provider = services.BuildServiceProvider();

        // Act
        var model1 = provider.GetService<IEdmModel>();
        var model2 = provider.GetService<IEdmModel>();

        // Assert
        model1.Should().BeSameAs(model2);
    }

    [Fact]
    public void AddOData_WhenCalled_RegistersODataServiceAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddOData(builder => { });
        var provider = services.BuildServiceProvider();

        // Act
        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();
        var service1 = scope1.ServiceProvider.GetService<IODataService>();
        var service2 = scope2.ServiceProvider.GetService<IODataService>();

        // Assert
        service1.Should().NotBeSameAs(service2);
    }

    [Fact]
    public void AddOData_WithOptionsConfigured_AppliesOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOData(
            options =>
            {
                options.PageSize = 50;
                options.EnableCaseInsensitiveFilter = true;
            },
            builder => { });
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ODataOptions>>().Value;

        // Assert
        options.PageSize.Should().Be(50);
        options.EnableCaseInsensitiveFilter.Should().BeTrue();
    }

    [Fact]
    public void AddOData_WithoutOptionsConfigured_UsesDefaults()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOData(builder => { });
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<ODataOptions>>().Value;

        // Assert
        options.PageSize.Should().BeNull();
        options.EnableCaseInsensitiveFilter.Should().BeFalse();
    }
}

public interface ITestEntity { }

public class TestEntity : ITestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
