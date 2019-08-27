using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lib.Core.Mongodb.Helper
{
    public abstract class MongoHelper<T> : IDisposable
    {
        private static string _connection_string;

        private string _database_name;
        private string _collection_name;

        private IMongoClient _client;
        private IMongoDatabase _database;

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <returns></returns>
        abstract protected Task CreateIndexAsync();

        public MongoHelper(string database_name, string collection_name)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            _connection_string = config["MongoConnection"];

            _database_name = database_name;
            _collection_name = collection_name;

            try
            {
                _client = new MongoClient(_connection_string);
                _database = _client.GetDatabase(_database_name);
            }
            catch (Exception ex)
            {

                throw;
            }

            Task.Run(() => CreateIndexAsync());
        }

        #region Property

        /// <summary>
        /// 基于数据类的Collection
        /// </summary>
        protected IMongoCollection<T> DBContext
        {
            get { return _database.GetCollection<T>(_collection_name); }
        }

        /// <summary>
        /// 基于BsonDocument的Collection
        /// </summary>
        protected IMongoCollection<BsonDocument> BsonDBContext
        {
            get { return _database.GetCollection<BsonDocument>(_collection_name); }
        }

        /// <summary>
        /// Collection的数据总数
        /// </summary>
        protected long Count
        {
            get { return DBContext.CountDocuments("{}"); }
        }

        #endregion Property

        #region Create

        /// <summary>
        /// 创建一条数据
        /// </summary>
        /// <param name="data">基于数据类的数据</param>
        /// <param name="option">创建参数</param>
        /// <returns>是否成功</returns>
        protected async Task<bool> CreateAsync(T data, InsertOneOptions option = null)
        {
            try
            {
                await DBContext.InsertOneAsync(data, option);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建一条数据
        /// </summary>
        /// <param name="document">基于BsonDocument的数据</param>
        /// <param name="option">创建参数</param>
        /// <returns>是否成功</returns>
        protected async Task<bool> CreateAsync(BsonDocument document, InsertOneOptions option = null)
        {
            try
            {
                await BsonDBContext.InsertOneAsync(document, option);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建多条数据
        /// </summary>
        /// <param name="data">基于数据类的数据列表</param>
        /// <param name="option">创建参数</param>
        /// <returns>是否成功</returns>
        protected async Task<bool> CreateManyAsync(IEnumerable<T> data, InsertManyOptions option = null)
        {
            try
            {
                await DBContext.InsertManyAsync(data, option);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建多条数据
        /// </summary>
        /// <param name="documents">基于BsonDocument的数据列表</param>
        /// <param name="option">创建参数</param>
        /// <returns>是否成功</returns>
        protected async Task<bool> CreateManyAsync(IEnumerable<BsonDocument> documents, InsertManyOptions option = null)
        {
            try
            {
                await BsonDBContext.InsertManyAsync(documents, option);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion Create

        #region Update

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <typeparam name="TField">基于数据类的字段类型</typeparam>
        /// <param name="field">匹配条件字段</param>
        /// <param name="value">匹配条件值</param>
        /// <param name="update">更新内容</param>
        /// <param name="option">更新参数</param>
        /// <returns>是否成功</returns>
        protected async Task<bool> UpdateOneAsync<TField>(Expression<Func<T, TField>> field, TField value, UpdateDefinition<T> update, FindOneAndUpdateOptions<T> option = null)
        {
            try
            {
                FindOneAndUpdateOptions<T> using_option;
                if (option == null)
                {
                    using_option = new FindOneAndUpdateOptions<T>()
                    {
                        ReturnDocument = ReturnDocument.After,
                    };
                }
                else
                {
                    using_option = option;
                    using_option.ReturnDocument = ReturnDocument.After;
                }

                var filter = Builders<T>.Filter.Eq(field, value);

                var result = await DBContext.FindOneAndUpdateAsync<T>(filter, update, using_option);
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <typeparam name="TField">基于数据类的字段类型</typeparam>
        /// <param name="filter">匹配条件</param>
        /// <param name="field">要更新的字段</param>
        /// <param name="value">要更新的内容</param>
        /// <param name="option">更新参数</param>
        /// <returns>是否成功</returns>
        protected async Task<bool> UpdateOneAsync<TField>(FilterDefinition<T> filter, Expression<Func<T, TField>> field, TField value, FindOneAndUpdateOptions<T> option = null)
        {
            try
            {
                FindOneAndUpdateOptions<T> using_option;
                if (option == null)
                {
                    using_option = new FindOneAndUpdateOptions<T>()
                    {
                        ReturnDocument = ReturnDocument.After,
                    };
                }
                else
                {
                    using_option = option;
                    using_option.ReturnDocument = ReturnDocument.After;
                }

                var update = Builders<T>.Update.Set(field, value);

                var result = await DBContext.FindOneAndUpdateAsync<T>(filter, update, using_option);
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="filter">匹配条件</param>
        /// <param name="update">更新内容</param>
        /// <param name="option">更新参数</param>
        /// <returns>是否成功</returns>
        protected async Task<bool> UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T> option = null)
        {
            try
            {
                FindOneAndUpdateOptions<T> using_option;
                if (option == null)
                {
                    using_option = new FindOneAndUpdateOptions<T>()
                    {
                        ReturnDocument = ReturnDocument.After,
                    };
                }
                else
                {
                    using_option = option;
                    using_option.ReturnDocument = ReturnDocument.After;
                }

                var result = await DBContext.FindOneAndUpdateAsync<T>(filter, update, using_option);
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <typeparam name="Filter_TField">基于数据类的字段类型</typeparam>
        /// <typeparam name="Update_TField">基于数据类的字段类型</typeparam>
        /// <param name="filter_field">匹配条件字段</param>
        /// <param name="filter_value">匹配条件值</param>
        /// <param name="update_field">要更新的字段</param>
        /// <param name="update_value">要更新的值</param>
        /// <param name="option">更新参数</param>
        /// <returns>是否成功</returns>
        protected async Task<bool> UpdateOneAsync<Filter_TField, Update_TField>(Expression<Func<T, Filter_TField>> filter_field, Filter_TField filter_value, Expression<Func<T, Update_TField>> update_field, Update_TField update_value, FindOneAndUpdateOptions<T> option = null)
        {
            try
            {
                FindOneAndUpdateOptions<T> using_option;
                if (option == null)
                {
                    using_option = new FindOneAndUpdateOptions<T>()
                    {
                        ReturnDocument = ReturnDocument.After,
                    };
                }
                else
                {
                    using_option = option;
                    using_option.ReturnDocument = ReturnDocument.After;
                }

                var filter = Builders<T>.Filter.Eq(filter_field, filter_value);
                var update = Builders<T>.Update.Set(update_field, update_value);

                var result = await DBContext.FindOneAndUpdateAsync<T>(filter, update, using_option);
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion Update

        #region Read

        /// <summary>
        /// 根据某字段的值统计数量
        /// </summary>
        /// <typeparam name="TField">基于数据类的字段类型</typeparam>
        /// <param name="field">字段名</param>
        /// <param name="value">字段值</param>
        /// <returns>数量</returns>
        protected async Task<long> SelectCountAsync<TField>(Expression<Func<T, TField>> field, TField value)
        {
            var filter = Builders<T>.Filter.Eq(field, value);
            var project = Builders<T>.Projection.Include("_id");
            var result = await DBContext.Find(filter).Project(project).CountDocumentsAsync();

            return result;
        }

        /// <summary>
        /// 根据匹配条件统计数量
        /// </summary>
        /// <param name="filter">匹配条件</param>
        /// <returns>数量</returns>
        protected async Task<long> SelectCountAsync(FilterDefinition<T> filter)
        {
            var project = Builders<T>.Projection.Include("_id");
            var result = await DBContext.Find(filter).Project(project).CountDocumentsAsync();

            return result;
        }

        /// <summary>
        /// 全参数的数据查询
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="project">查询字段，可忽略不写</param>
        /// <param name="orderby">排序字段，可忽略不写</param>
        /// <param name="skip">跳过数量，可忽略不写</param>
        /// <param name="limit">获取数量，可忽略不写</param>
        /// <returns>返回数据列表</returns>
        protected async Task<List<T>> SelectAsync(FilterDefinition<T> filter, ProjectionDefinition<T> project = null, SortDefinition<T> orderby = null, int? skip = null, int? limit = null)
        {
            var result = DBContext.Find<T>(filter);
            if (project != null)
                result = result.Project<T>(project);
            if (orderby != null)
                result = result.Sort(orderby);
            if (skip != null)
                result = result.Skip(skip);
            if (limit != null)
                result = result.Limit(limit);

            return await result.ToListAsync();
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="orderby">排序条件，可忽略不写</param>
        /// <returns>返回数据列表</returns>
        protected async Task<List<T>> SelectAllAsync(SortDefinition<T> orderby = null)
        {
            var filter = Builders<T>.Filter.Empty;
            return await SelectAsync(filter, null, orderby);
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <param name="project">查询字段</param>
        /// <param name="orderby">排序字段，可忽略不写</param>
        /// <returns>返回数据列表</returns>
        protected async Task<List<T>> SelectAllAsync(ProjectionDefinition<T> project, SortDefinition<T> orderby = null)
        {
            var filter = Builders<T>.Filter.Empty;
            return await SelectAsync(filter, project, orderby);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="skip">跳过数量</param>
        /// <param name="limit">获取数量</param>
        /// <returns>返回数据列表</returns>
        protected async Task<List<T>> SelectAsync(FilterDefinition<T> filter, SortDefinition<T> orderby, int? skip = null, int? limit = null)
        {
            return await SelectAsync(filter, null, orderby, skip, limit);
        }

        /// <summary>
        /// 查询一条数据。这儿做了检查，如果按照条件会得到多于一条数据，会报错。
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="project">查询字段，可忽略不写</param>
        /// <returns>返回数据</returns>
        protected async Task<T> SelectOneAsync(FilterDefinition<T> filter, ProjectionDefinition<T> project = null)
        {
            var result = await SelectAsync(filter, project);
            if (result.Count > 1)
                throw new Exception("查询条件不正确，返回的数据超过一条");
            else if (result.Count == 1)
                return result[0];
            else
                return default(T);
        }

        #endregion Read

        #region ReadCursor

        /// <summary>
        /// 查询数据并返回数据的cursor
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="project">查询字段，可忽略不写</param>
        /// <param name="orderby">排序字段，可忽略不写</param>
        /// <param name="skip">跳过数量，可忽略不写</param>
        /// <param name="limit">获取数量，可忽略不写</param>
        /// <returns>返回数据cursor</returns>
        protected async Task<IAsyncCursor<T>> SelectCursorAsync(FilterDefinition<T> filter, ProjectionDefinition<T> project = null, SortDefinition<T> orderby = null, int? skip = null, int? limit = null)
        {
            var result = DBContext.Find<T>(filter);
            if (project != null)
                result = result.Project<T>(project);
            if (orderby != null)
                result = result.Sort(orderby);
            if (skip != null)
                result = result.Skip(skip);
            if (limit != null)
                result = result.Limit(limit);

            return await result.ToCursorAsync();
        }

        /// <summary>
        /// 查询对应条件的所有数据，返回cursor
        /// </summary>
        /// <param name="orderby">排序字段，可忽略不写</param>
        /// <returns>返回数据cursor</returns>
        protected async Task<IAsyncCursor<T>> SelectAllCursorAsync(SortDefinition<T> orderby = null)
        {
            var filter = Builders<T>.Filter.Empty;
            return await SelectCursorAsync(filter, null, orderby);
        }

        /// <summary>
        /// 查询对应条件的所有数据，返回cursor
        /// </summary>
        /// <param name="project">查询字段</param>
        /// <param name="orderby">排序字段，可忽略不写</param>
        /// <returns>返回数据cursor</returns>
        protected async Task<IAsyncCursor<T>> SelectAllCursorAsync(ProjectionDefinition<T> project, SortDefinition<T> orderby = null)
        {
            var filter = Builders<T>.Filter.Empty;
            return await SelectCursorAsync(filter, project, orderby);
        }

        /// <summary>
        /// 查询数据并返回数据的cursor
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="skip">跳过数量</param>
        /// <param name="limit">获取数量</param>
        /// <returns>返回数据cursor</returns>
        protected async Task<IAsyncCursor<T>> SelectCursorAsync(FilterDefinition<T> filter, SortDefinition<T> orderby, int? skip = null, int? limit = null)
        {
            return await SelectCursorAsync(filter, null, orderby, skip, limit);
        }

        #endregion ReadCursor

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
