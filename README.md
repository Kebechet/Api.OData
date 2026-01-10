[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/kebechet)

# Api.OData
[![NuGet Version](https://img.shields.io/nuget/v/Kebechet.Api.OData)](https://www.nuget.org/packages/Kebechet.Api.OData/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Kebechet.Api.OData)](https://www.nuget.org/packages/Kebechet.Api.OData/)
[![Build](https://github.com/Kebechet/Api.OData/actions/workflows/build.yml/badge.svg)](https://github.com/Kebechet/Api.OData/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/Kebechet/Api.OData/graph/badge.svg)](https://codecov.io/gh/Kebechet/Api.OData)
![Last updated](https://img.shields.io/github/last-commit/Kebechet/Api.OData/main?label=last%20updated)
[![Twitter](https://img.shields.io/twitter/url/https/twitter.com/samuel_sidor.svg?style=social&label=Follow%20samuel_sidor)](https://x.com/samuel_sidor)

A lightweight library for applying OData query options (`$filter`, `$orderby`, `$skip`, `$top`, `$select`, `$apply`) to `IQueryable<T>` and `IEnumerable<T>` collections in ASP.NET Core applications.

## Features
- Apply OData query options from HTTP request to any collection
- Fluent extension methods for IQueryable and IEnumerable
- Configurable options (page size, ignored query options, null propagation)
- Case-insensitive `$filter` support (configurable collation)
- `IODataService` interface for easy mocking/testing
- Generic `RegisterEntitiesFromAssemblies<TMarker>()` - use your own marker interface
- Full XML documentation for IntelliSense support

## Installation
```bash
dotnet add package Kebechet.Api.OData
```

## Usage

### Setup
```csharp
// Basic setup with default options
services.AddOData(builder =>
{
    builder.RegisterEntitiesFromAssemblies<IEntity>(typeof(MyEntity).Assembly);
});

// With manual type registration
services.AddOData(builder =>
{
    builder.RegisterTypes(
        typeof(ProductResponse),
        typeof(OrderResponse),
        typeof(CustomerResponse)
    );
});

// With custom options
services.AddOData(
    options =>
    {
        options.PageSize = 100;
        options.IgnoreExpand = false;
        options.IgnoreCount = false;
    },
    builder =>
    {
        builder.RegisterEntitiesFromAssemblies<IEntity>(typeof(MyEntity).Assembly);
    });
```

### In Your Service/Handler
```csharp
public class ProductService
{
    private readonly AppDbContext _dbContext;
    private readonly IODataService _oDataService;

    public ProductService(AppDbContext dbContext, IODataService oDataService)
    {
        _dbContext = dbContext;
        _oDataService = oDataService;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await _dbContext.Products
            .Where(p => !p.IsDeleted)
            .ApplyODataQuery(_oDataService)
            .ToListAsync();
    }
}
```

### Selective Application
```csharp
public async Task<List<Product>> GetProductsAsync()
{
    return await _dbContext.Products
        .ApplyODataFilter(_oDataService)      // only $filter from request
        .OrderByDescending(p => p.CreatedAt)  // custom ordering
        .Take(50)                             // custom limit
        .ToListAsync();
}
```

### Available Extension Methods
```csharp
query.ApplyODataQuery(oDataService);       // all OData options
query.ApplyODataFilter(oDataService);      // $filter
query.ApplyODataOrderBy(oDataService);     // $orderby
query.ApplyODataPagination(oDataService);  // $skip and $top
query.ApplyODataSelect(oDataService);      // $select
query.ApplyODataApply(oDataService);       // $apply

// Conditional application
query.ApplyODataFilter(oDataService, isEnabled: shouldFilter);
```

## Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `PageSize` | `null` | Maximum page size for results. Null means no limit. |
| `IgnoreExpand` | `true` | When true, `$expand` query option is ignored. |
| `IgnoreCount` | `true` | When true, `$count` query option is ignored. |
| `HandleNullPropagation` | `False` | How null propagation is handled during query composition. |
| `EnableCaseInsensitiveFilter` | `false` | When true, string comparisons in `$filter` are case-insensitive. |
| `CaseInsensitiveCollation` | `"Latin1_General_CI_AS"` | Collation for case-insensitive comparisons. Default is SQL Server. For SQLite, use `"NOCASE"`. |

### Case-Insensitive Filtering Example
```csharp
services.AddOData(
    options =>
    {
        options.EnableCaseInsensitiveFilter = true;
        // Default collation is "Latin1_General_CI_AS" (SQL Server)
        // For SQLite use: options.CaseInsensitiveCollation = "NOCASE";
    },
    builder =>
    {
        builder.RegisterEntitiesFromAssemblies<IEntity>(typeof(MyEntity).Assembly);
    });
```
Query `?$filter=Name eq 'john'` will match "John", "JOHN", "john", etc.

## Query Processing Order
1. `$apply`
2. `$filter`
3. `$orderby`
4. `$skip`
5. `$top`
6. `$select`

# License
This repository is licensed with the [MIT](LICENSE) license.
