namespace CityLibrary.Shared.DbBase.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

public interface IBaseRepo<T> where T : TableBase
    {
        IMongoQueryable<T> GetData(Expression<Func<T, bool>> predicate = null, bool getWithDeleted = false);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool getWithDeleted = false);
        Task<List<T>> GetAllAsync(FilterDefinition<T> predicate, bool getWithDeleted = false);
        Task<T> GetAsync(FilterDefinition<T> predicate, bool getWithDeleted = false);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, bool getWithDeleted = false);
        Task<T> GetByIdAsync(string id, bool getWithDeleted = false);
        Task<T> AddAsync(T entity, IClientSessionHandle session = null);
        Task<bool> AddRangeAsync(IEnumerable<T> entities, IClientSessionHandle session = null);
        Task<T> ReplaceAsync(string idForReplacement, T entity,
            FindOneAndReplaceOptions<T> options = null,
            IClientSessionHandle session = null);
        Task<T> ReplaceAsync(T entity, Expression<Func<T, bool>> predicate, 
            FindOneAndReplaceOptions<T> options = null,
            IClientSessionHandle session = null);
        Task<T> ReplaceAsync(T entity, FilterDefinition<T> filter,
            FindOneAndReplaceOptions<T> options = null,
            IClientSessionHandle session = null);

        Task<UpdateResult> UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, UpdateOptions options = null, IClientSessionHandle session = null);

        Task<UpdateResult> UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, UpdateOptions options = null, IClientSessionHandle session = null);

        Task<DeleteResult> DeleteAsync(T entity, IClientSessionHandle session = null);
        Task<DeleteResult> DeleteAsync(string id, IClientSessionHandle session = null);
        Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> predicate, IClientSessionHandle session = null);
        Task<DeleteResult> DeleteAsync(FilterDefinition<T> filter, IClientSessionHandle session = null);

        Task<UpdateResult> DeleteSoftlyAsync(T entity, IClientSessionHandle session = null);
        Task<UpdateResult> DeleteSoftlyAsync(string id, IClientSessionHandle session = null);
        Task<UpdateResult> DeleteSoftlyAsync(Expression<Func<T, bool>> predicate, IClientSessionHandle session = null);
        Task<UpdateResult> DeleteSoftlyAsync(FilterDefinition<T> filter, IClientSessionHandle session = null);

        /// <summary>
        /// Get the number of records without deleted
        /// </summary>
        /// <returns></returns>
        long GetNumberOfRecords(bool getWithDeleted = false);

        /// <summary>
        /// Get the number of records without deleted
        /// </summary>
        /// <returns></returns>
        Task<long> GetNumberOfRecordsAsync(bool getWithDeleted = false);

        /// <summary>
        /// Get the number of records with LINQ expression
        /// </summary>
        /// <returns></returns>
        long GetNumberOfRecords(Expression<Func<T, bool>> predicate, bool getWithDeleted = false);

        /// <summary>
        /// Get the number of records with LINQ expression
        /// </summary>
        /// <returns></returns>
        Task<long> GetNumberOfRecordsAsync(Expression<Func<T, bool>> predicate, bool getWithDeleted = false);

        IFindFluent<T, T> Query(FilterDefinition<T> filter, bool getWithDeleted = false);

        Task<bool> DoesEntityExistAsync(string id, bool getWithDeleted = false);

        Task<bool> DoesEntityExistAsync(Expression<Func<T, bool>> predicate);

        Task<bool> DoesEntityExistAsync(FilterDefinition<T> filter);

        IMongoClient GetMongoClient();

        IAggregateFluent<T> AggregationOfRepository(AggregateOptions options = null);
    }