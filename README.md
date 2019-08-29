# Lib.Core.Mongodb.Helper

Refer:

> Mongodb.driver, version:2.9.1

Setting in appsetting.json of project:

> {
>
> ​	"MongoConnection": "mongodb://user:password@server:port/dbname",
>
> }

Example code - with data class:

```c#
public class dataclass
{
    public const string DATABASE = "DatabaseName";
    public const string COLLECTION = "CollectionName";
    
    public ObjectId _id { get; set; }
    public string some_field { get; set; }
    ...
}

public class dbclass : MongoHelper<dataclass>
{
	protected override async task CreateIndexAsync()
	{
		//add your index creating code here
	}
	public dbclass() : base(dataclass.DATABASE, dataclass.COLLECTION)
	{
		//
	}
	internal async Task<result> db_function(some parameter)
	{
		var filter = Builders<dataclass>.filter.Eq(p->p.some_field, value);
		var project = Builders<dataclass>.Projection.Include(p->p.some_field);
		return result = await SelectOneAsync(filter, project);
	}
}
```

Example code - with BsonDocument:

```c#
public class dbclass : MongoHelper<BsonDocument>
{
	protected override async task CreateIndexAsync()
	{
		//add your index creating code here
	}
	public dbclass() : base("DatabaseName", "CollectionName")
	{
		//
	}
	internal async Task<result> db_function(some parameter)
	{
		var filter = Builders<dataclass>.filter.Eq("some_field", value);
		var project = Builders<dataclass>.Projection.Include("some_field");
		return result = await SelectOneAsync(filter, project);
	}
}
```

Example code - call dbclass:

```c#
some_function()
{
    using( dbclass myclass = new dbclass() )
    {
        var result = await myclass.db_function(some parameter);
    }
}
```





Ready function:

**Property**

```
- DBContext
- BsonDBContext
- Count
```

**Create**

```
- async Task<bool> CreateAsync(T data, InsertOneOptions option = null)
- async Task<bool> CreateAsync(BsonDocument document, InsertOneOptions option = null)
- async Task<bool> CreateManyAsync(IEnumerable<T> data, InsertManyOptions option = null)
- async Task<bool> CreateManyAsync(IEnumerable<BsonDocument> documents, InsertManyOptions option = null)
```

**Update**

```
- async Task<bool> UpdateOneAsync<TField>(Expression<Func<T, TField>> field, TField value, UpdateDefinition<T> update, FindOneAndUpdateOptions<T> option = null)
- async Task<bool> UpdateOneAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value, FindOneAndUpdateOptions<T> option = null)
- async Task<bool> UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T> option = null)
- async Task<bool> UpdateOneAsync<Filter_TField, Update_TField>(Expression<Func<T, Filter_TField>> filter_field, Filter_TField filter_value, Expression<Func<T, Update_TField>> update_field, Update_TField update_value, FindOneAndUpdateOptions<T> option = null)
```

**Read**

```
- async Task<long> SelectCountAsync<TField>(Expression<Func<T, TField>> field, TField value)
- async Task<long> SelectCountAsync(FilterDefinition<T> filter)
- async Task<List<T>> SelectAsync(FilterDefinition<T> filter, ProjectionDefinition<T> project = null, SortDefinition<T> orderby = null, int? skip = null, int? limit = null)
- async Task<List<T>> SelectAllAsync(SortDefinition<T> orderby = null)
- async Task<List<T>> SelectAllAsync(ProjectionDefinition<T> project, SortDefinition<T> orderby = null)
- async Task<List<T>> SelectAsync(FilterDefinition<T> filter, SortDefinition<T> orderby, int? skip = null, int? limit = null)
- async Task<T> SelectOneAsync(FilterDefinition<T> filter, ProjectionDefinition<T> project = null)
```

**Read with cursor**

```
- async Task<IAsyncCursor<T>> SelectCursorAsync(FilterDefinition<T> filter, ProjectionDefinition<T> project = null, SortDefinition<T> orderby = null, int? skip = null, int? limit = null)
- async Task<IAsyncCursor<T>> SelectAllCursorAsync(SortDefinition<T> orderby = null)
- async Task<IAsyncCursor<T>> SelectAllCursorAsync(ProjectionDefinition<T> project, SortDefinition<T> orderby = null)
- async Task<IAsyncCursor<T>> SelectCursorAsync(FilterDefinition<T> filter, SortDefinition<T> orderby, int? skip = null, int? limit = null)
```









