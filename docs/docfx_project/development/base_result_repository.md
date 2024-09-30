# BaseResultRepository Documentation

The `BaseResultRepository<TEntity, TId>` is a generic base class that provides common CRUD (Create, Read, Update, Delete) operations for entities in a database context. It uses the `ResultDTO` class to encapsulate the results of operations, providing a consistent way to handle success, failures, and exceptions.

## Table of Contents

1. [Setup](#setup)
2. [Basic Usage](#basic-usage)
3. [CRUD Operations](#crud-operations)
   - [Create](#create)
   - [Read](#read)
   - [Update](#update)
   - [Delete](#delete)
4. [Advanced Features](#advanced-features)
   - [Filtering and Ordering](#filtering-and-ordering)
   - [Including Related Entities](#including-related-entities)
   - [Transactions](#transactions)
5. [Error Handling](#error-handling)

## Setup

To use the `BaseResultRepository`, you need to create a repository for your specific entity that inherits from `BaseResultRepository<TEntity, TId>`. For example:

```csharp
public class DatasetsRepository : BaseResultRepository<Dataset, Guid>, IDatasetsRepository
{
    public DatasetsRepository(ApplicationDbContext db) : base(db)
    {
    }
}
```

## Basic Usage

After setting up your repository, you can use it in your services or controllers like this:

```csharp
public class DatasetService
{
    private readonly DatasetsRepository _repository;

    public DatasetService(DatasetsRepository repository)
    {
        _repository = repository;
    }

    // Use the repository methods here
}
```

## CRUD Operations

### Create

To create a new entity:

```csharp
Dataset newDataset = new Dataset { Name = "New Dataset", Description = "Description" };
ResultDTO result = await _repository.Create(newDataset);

if (result.IsSuccess)
{
    // Entity created successfully
}
else
{
    // Handle error
    Console.WriteLine(result.ErrMsg);
}
```

To create and return the entity:

```csharp
ResultDTO<Dataset> result = await _repository.CreateAndReturnEntity(newDataset);

if (result.IsSuccess && result.Data != null)
{
    Dataset createdDataset = result.Data;
    // Use the created dataset
}
```

### Read

To get an entity by ID:

```csharp
Guid datasetId = Guid.NewGuid(); // Replace with actual ID
ResultDTO<Dataset?> result = await _repository.GetById(datasetId);

if (result.IsSuccess && result.Data != null)
{
    Dataset dataset = result.Data;
    // Use the dataset
}
```

To get all entities:

```csharp
ResultDTO<IEnumerable<Dataset>> result = await _repository.GetAll();

if (result.IsSuccess && result.Data != null)
{
    foreach (var dataset in result.Data)
    {
        // Process each dataset
    }
}
```

### Update

To update an entity:

```csharp
Dataset datasetToUpdate = // ... get the dataset to update
datasetToUpdate.Name = "Updated Name";

ResultDTO result = await _repository.Update(datasetToUpdate);

if (result.IsSuccess)
{
    // Entity updated successfully
}
else
{
    // Handle error
    Console.WriteLine(result.ErrMsg);
}
```

### Delete

To delete an entity:

```csharp
Dataset datasetToDelete = // ... get the dataset to delete

ResultDTO result = await _repository.Delete(datasetToDelete);

if (result.IsSuccess)
{
    // Entity deleted successfully
}
else
{
    // Handle error
    Console.WriteLine(result.ErrMsg);
}
```

## Advanced Features

### Filtering and Ordering

Use the `GetAll` method with filtering and ordering:

```csharp
ResultDTO<IEnumerable<Dataset>> result = await _repository.GetAll(
    filter: d => d.IsPublished == true,
    orderBy: q => q.OrderBy(d => d.Name)
);
```

### Including Related Entities

#### Using GetByIdInclude

The `GetByIdInclude` method allows you to include related entities when fetching an entity by its ID. This is useful when you need to load specific related entities along with the main entity in a single database query.

```csharp
ResultDTO<Dataset?> result = await _repository.GetByIdInclude(
    id: datasetId,
    includeProperties: new Expression<Func<Dataset, object>>[] 
    {
        d => d.DatasetImages,
        d => d.DatasetClasses
    }
);

if (result.IsSuccess && result.Data != null)
{
    Dataset dataset = result.Data;
    // Now you can access dataset.DatasetImages and dataset.DatasetClasses
    // without additional database queries
}
```

#### Using GetByIdIncludeThenAll

The `GetByIdIncludeThenAll` method provides even more flexibility by allowing you to include nested related entities. This is particularly useful when you need to load a complex object graph in a single query.

```csharp
ResultDTO<Dataset?> result = await _repository.GetByIdIncludeThenAll(
    id: datasetId,
    includeProperties: new[] 
    {
        (Expression<Func<Dataset, object>>)(d => d.DatasetImages),
        (d => d.DatasetClasses, new Expression<Func<object, object>>[] 
        {
            c => ((Dataset_DatasetClass)c).DatasetClass
        })
    }
);

if (result.IsSuccess && result.Data != null)
{
    Dataset dataset = result.Data;
    // Now you can access dataset.DatasetImages, dataset.DatasetClasses,
    // and the DatasetClass for each Dataset_DatasetClass
    // without additional database queries
}
```

In this example, we're including `DatasetImages` and `DatasetClasses`, and then for each `DatasetClass`, we're also including its related `DatasetClass` entity.

### Transactions

The repository supports transactions. You can use `SaveChangesAsync` to commit changes:

```csharp
ResultDTO<int> saveResult = await _repository.SaveChangesAsync();
if (saveResult.IsSuccess)
{
    // Changes saved successfully
}
```

## Error Handling

The `BaseResultRepository` uses the `ResultDTO` class to encapsulate the results of operations. Always check the `IsSuccess` property before accessing the `Data`. If `IsSuccess` is `false`, check the `ErrMsg` for error details.

For methods that can throw exceptions (like database errors), you can use the `HandleError` extension method:

```csharp
ResultDTO<Dataset> result = await _repository.CreateAndReturnEntity(newDataset);
if (!result.HandleError())
{
    // Handle the error
    return;
}

// Use result.Data safely here
```

This documentation provides a comprehensive guide to using the `BaseResultRepository`, including advanced features for including related entities. Remember to adapt the examples to your specific entity types and business logic. The `GetByIdInclude` and `GetByIdIncludeThenAll` methods are powerful tools for optimizing database queries and reducing the number of round-trips to the database when working with complex entity relationships.