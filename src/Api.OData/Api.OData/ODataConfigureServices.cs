using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.ModelBuilder;

namespace Api.OData;

public static class ODataConfigureServices
{
    /// <summary>
    /// Adds OData services to the service collection with custom entity registration.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="registerEntities">Action to register entities in the OData model builder</param>
    public static void AddOData(this IServiceCollection services, Action<ODataConventionModelBuilder> registerEntities)
    {
        var modelBuilder = new ODataConventionModelBuilder();

        registerEntities(modelBuilder);

        var edmModel = modelBuilder.GetEdmModel();
        services.AddSingleton(edmModel);

        services.AddHttpContextAccessor();

        services.AddScoped<ODataService>();
    }

    /// <summary>
    /// Helper method to automatically register all types implementing specified marker interface from specified assemblies.
    /// </summary>
    /// <typeparam name="TMarker">The marker interface type to scan for (e.g., IEntity)</typeparam>
    /// <param name="modelBuilder">The OData model builder</param>
    /// <param name="assemblies">Assemblies to scan for types implementing TMarker</param>
    public static void RegisterEntitiesFromAssemblies<TMarker>(this ODataConventionModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        var entityTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass
                && !type.IsAbstract
                && type.IsPublic
                && typeof(TMarker).IsAssignableFrom(type));

        foreach (var type in entityTypes)
        {
            modelBuilder.AddEntityType(type);
        }
    }

    /// <summary>
    /// Helper method to register specific types (e.g., response models) in the OData model.
    /// </summary>
    /// <param name="modelBuilder">The OData model builder</param>
    /// <param name="types">Types to register</param>
    public static void RegisterTypes(this ODataConventionModelBuilder modelBuilder, params Type[] types)
    {
        foreach (var type in types)
        {
            modelBuilder.AddEntityType(type);
        }
    }
}
