using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using BookReservationReportApi.ContextRelated;
using CityLibrary.Shared.Extensions;
using CityLibrary.Shared.DbBase.Mongo;
using static CityLibrary.Shared.Extensions.TokenExtensions.AccesInfoFromToken;

namespace BookReservationReportApi.Repositories.Base
{
    public class BaseRepo<T> : IBaseRepo<T> where T : TableBase
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly IMongoClient _mongoClient;
        public BaseRepo(AppDbContext context, IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            _collection = context.Database.Collection<T>();
        }

        public virtual async Task<T> AddAsync(T entity, IClientSessionHandle session = null)
        {
            var options = new InsertOneOptions { BypassDocumentValidation = false };
            if (session is null)
                await _collection.InsertOneAsync(entity, options);
            else
                await _collection.InsertOneAsync(session, entity, options);
            return entity;
        }

        public virtual async Task<bool> AddRangeAsync(IEnumerable<T> entities, IClientSessionHandle session = null)
        {
            var options = new BulkWriteOptions { IsOrdered = false, BypassDocumentValidation = false };
            if (session is null)
                return (await _collection.BulkWriteAsync((IEnumerable<WriteModel<T>>)entities, options)).IsAcknowledged;

            return (await _collection.BulkWriteAsync(session, (IEnumerable<WriteModel<T>>)entities, options)).IsAcknowledged;
        }

        public virtual async Task<DeleteResult> DeleteAsync(T entity, IClientSessionHandle session = null)
        {
            var filter = MongoRelatedExtensions.FilterByObjectId<T>(entity.Id);
            if (session is null)
                return await _collection.DeleteOneAsync(filter);

            return await _collection.DeleteOneAsync(session, filter);
        }

        public virtual async Task<DeleteResult> DeleteAsync(string id, IClientSessionHandle session = null)
        {
            var filter = MongoRelatedExtensions.FilterByObjectId<T>(id);
            if (session is null)
                return await _collection.DeleteOneAsync(filter);

            return await _collection.DeleteOneAsync(session, filter);
        }

        public virtual async Task<DeleteResult> DeleteAsync(Expression<Func<T, bool>> predicate, IClientSessionHandle session = null)
        {
            if (session is null)
                return await _collection.DeleteManyAsync(predicate);

            return await _collection.DeleteManyAsync(session, predicate);
        }

        public virtual async Task<DeleteResult> DeleteAsync(FilterDefinition<T> filter, IClientSessionHandle session = null)
        {
            if (session is null)
                return await _collection.DeleteManyAsync(filter);

            return await _collection.DeleteManyAsync(session, filter);
        }

        public virtual async Task<UpdateResult> DeleteSoftlyAsync(T entity, IClientSessionHandle session = null)
        {
            var filter = MongoRelatedExtensions.FilterByObjectId<T>(entity.Id);
            var updateBuilder = Builders<T>.Update.Set(x => x.DeletedAt, DateTime.UtcNow)
                                                   .Set(x => x.IsDeleted, true) 
                                                   .Set(x => x.DeletedBy, GetMyUserId());

            if (session is null)
                return await _collection.UpdateOneAsync(filter, updateBuilder);

            return await _collection.UpdateOneAsync(session, filter, updateBuilder);
        }

        public virtual async Task<UpdateResult> DeleteSoftlyAsync(string id, IClientSessionHandle session = null)
        {
            var filter = MongoRelatedExtensions.FilterByObjectId<T>(id);
            var updateBuilder = Builders<T>.Update.Set(x => x.DeletedAt, DateTime.UtcNow)
                                                   .Set(x => x.IsDeleted, true) 
                                                   .Set(x => x.DeletedBy, GetMyUserId());

            if (session is null)
                return await _collection.UpdateOneAsync(filter, updateBuilder);

            return await _collection.UpdateOneAsync(session, filter, updateBuilder);
        }

        public virtual async Task<UpdateResult> DeleteSoftlyAsync(Expression<Func<T, bool>> predicate, IClientSessionHandle session = null)
        {
            var filter = Builders<T>.Filter.Where(predicate);
            var updateBuilder = Builders<T>.Update.Set(x => x.DeletedAt, DateTime.UtcNow)
                                                   .Set(x => x.IsDeleted, true) 
                                                   .Set(x => x.DeletedBy, GetMyUserId());
            
            if (session is null)
                return await _collection.UpdateManyAsync(filter, updateBuilder);

            return await _collection.UpdateManyAsync(session, filter, updateBuilder);
        }

        public virtual async Task<UpdateResult> DeleteSoftlyAsync(FilterDefinition<T> filter, IClientSessionHandle session = null)
        {
            var updateBuilder = Builders<T>.Update.Set(x => x.DeletedAt, DateTime.UtcNow)
                                                   .Set(x => x.IsDeleted, true) 
                                                   .Set(x => x.DeletedBy, GetMyUserId());

            if (session is null)
                return await _collection.UpdateManyAsync(filter, updateBuilder);

            return await _collection.UpdateManyAsync(session, filter, updateBuilder);
        }

        public virtual async Task<bool> DoesEntityExistAsync(string id, bool getWithDeleted = false)
        {
            var filter = MongoRelatedExtensions.FilterByObjectId<T>(id);
            if (!getWithDeleted)
                filter &= Builders<T>.Filter.Ne(x => x.IsDeleted, true);
            return await _collection.Find(filter).AnyAsync();
        }

        public virtual async Task<bool> DoesEntityExistAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).AnyAsync();
        }

        public virtual async Task<bool> DoesEntityExistAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).AnyAsync();
        }

        public IMongoQueryable<T> GetData(Expression<Func<T, bool>> predicate = null, bool getWithDeleted = false)
        {
            if (predicate is null)
            {
                if (getWithDeleted)
                    return _collection.AsQueryable();

                return _collection.AsQueryable().Where(x => !x.IsDeleted);
            }
            else
            {
                if (getWithDeleted)
                    return _collection.AsQueryable().Where(predicate);

                return _collection.AsQueryable().Where(predicate).Where(x => !x.IsDeleted);
            }
        }

        public virtual async Task<List<T>> GetAllAsync(FilterDefinition<T> filter = null, bool getWithDeleted = false)
        {
            if (filter is null)
            {
                if (getWithDeleted)
                    return await _collection.Find(new BsonDocument()).ToListAsync();

                return await _collection.AsQueryable().Where(x => !x.IsDeleted).ToListAsync();
            }
            else
            {
                if (getWithDeleted)
                    return await _collection.Find(filter).ToListAsync();


                filter &= Builders<T>.Filter.Ne(x => x.IsDeleted, true);

                return await _collection.Find(filter).ToListAsync();
            }
        }

        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, bool getWithDeleted = false)
        {
            if (predicate is null)
            {
                if (getWithDeleted)
                    return await _collection.Find(new BsonDocument()).ToListAsync();

                return await _collection.AsQueryable().Where(x => !x.IsDeleted).ToListAsync();
            }
            else
            {
                if (getWithDeleted)
                    return await _collection.AsQueryable().Where(predicate).ToListAsync();

                return await _collection.AsQueryable().Where(predicate).Where(x => !x.IsDeleted).ToListAsync();
            }
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool getWithDeleted = false)
        {
            if (predicate is null)
            {
                if (getWithDeleted)
                    return await _collection.Find(new BsonDocument()).SingleOrDefaultAsync();

                return await _collection.AsQueryable().Where(x => !x.IsDeleted).SingleOrDefaultAsync();
            }
            else
            {
                if (getWithDeleted)
                    return await _collection.AsQueryable().Where(predicate).SingleOrDefaultAsync();

                return await _collection.AsQueryable().Where(predicate).Where(x => !x.IsDeleted).SingleOrDefaultAsync();
            }
        }

        public virtual async Task<T> GetAsync(FilterDefinition<T> filter, bool getWithDeleted = false)
        {
            if (filter is null)
            {
                if (getWithDeleted)
                    return await _collection.Find(new BsonDocument()).SingleOrDefaultAsync();

                return await _collection.AsQueryable().Where(x => !x.IsDeleted).SingleOrDefaultAsync();
            }
            else
            {
                if (getWithDeleted)
                    return await _collection.Find(filter).SingleOrDefaultAsync();


                filter &= Builders<T>.Filter.Ne(x => x.IsDeleted, true);

                return await _collection.Find(filter).SingleOrDefaultAsync();
            }
        }

        public virtual async Task<T> GetByIdAsync(string id, bool getWithDeleted = false)
        {
            var filter = MongoRelatedExtensions.FilterByObjectId<T>(id);
            if (!getWithDeleted)
                filter &= Builders<T>.Filter.Ne(x => x.IsDeleted, true);

            return await _collection.Find(filter).SingleOrDefaultAsync();
        }

        public IMongoClient GetMongoClient()
        {
            return _mongoClient;
        }

        public virtual long GetNumberOfRecords(bool getWithDeleted = false)
        {
            if (getWithDeleted)
                return _collection.CountDocuments(new BsonDocument());

            var filter = Builders<T>.Filter.Ne(x => x.IsDeleted, true);
            return _collection.CountDocuments(filter);
        }

        public virtual long GetNumberOfRecords(Expression<Func<T, bool>> predicate, bool getWithDeleted = false)
        {
            if (getWithDeleted)
                return _collection.AsQueryable().LongCount(predicate);

            return _collection.AsQueryable().Where(x => !x.IsDeleted).LongCount(predicate);
        }

        public virtual async Task<long> GetNumberOfRecordsAsync(bool getWithDeleted = false)
        {
            if (getWithDeleted)
                return await _collection.CountDocumentsAsync(new BsonDocument());

            var filter = Builders<T>.Filter.Ne(x => x.IsDeleted, true);
            return await _collection.CountDocumentsAsync(filter);
        }

        public virtual async Task<long> GetNumberOfRecordsAsync(Expression<Func<T, bool>> predicate, bool getWithDeleted = false)
        {
            if (getWithDeleted)
                return await _collection.AsQueryable().LongCountAsync(predicate);

            return await _collection.AsQueryable().Where(x => !x.IsDeleted).LongCountAsync(predicate);
        }

        public virtual IFindFluent<T, T> Query(FilterDefinition<T> filter, bool getWithDeleted = false)
        {
            if (!getWithDeleted)
                filter &= Builders<T>.Filter.Ne(x => x.IsDeleted, true);

            return _collection.Find(filter);
        }

        /// <summary>
        /// Preserves audit data
        /// </summary>
        public virtual async Task<T> ReplaceAsync(
            string idForReplacement,
            T entity,
            FindOneAndReplaceOptions<T> options = null,
            IClientSessionHandle session = null)
        {
            var filter = MongoRelatedExtensions.FilterByObjectId<T>(idForReplacement);
            T oldRecord = await GetByIdAsync(idForReplacement);
            entity.CreatedAt = oldRecord.CreatedAt;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.DeletedAt = oldRecord.DeletedAt;
            entity.DeletedBy = oldRecord.DeletedBy;
            entity.IsDeleted = oldRecord.IsDeleted;
            entity.UpdatedBy = GetMyUserId();
            entity.CreatedBy = oldRecord.CreatedBy;
            if (session is null)
                await _collection.FindOneAndReplaceAsync(filter, entity, options);
            else
                await _collection.FindOneAndReplaceAsync(session, filter, entity, options);

            return entity;
        }

        /// <summary>
        /// Preserves audit data
        /// </summary>
        public virtual async Task<T> ReplaceAsync(
            T entity,
            Expression<Func<T, bool>> predicate,
            FindOneAndReplaceOptions<T> options = null,
            IClientSessionHandle session = null)
        {
            var filter = Builders<T>.Filter.Where(predicate);
            T oldRecord = await GetAsync(filter);
            entity.CreatedAt = oldRecord.CreatedAt;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.DeletedAt = oldRecord.DeletedAt;
            entity.DeletedBy = oldRecord.DeletedBy;
            entity.IsDeleted = oldRecord.IsDeleted;
            entity.UpdatedBy = GetMyUserId();
            entity.CreatedBy = oldRecord.CreatedBy;
            if (session is null)
                await _collection.FindOneAndReplaceAsync(filter, entity, options);
            else
                await _collection.FindOneAndReplaceAsync(session, filter, entity, options);

            return entity;
        }

        /// <summary>
        /// Does not preserve audit data
        /// </summary>
        public virtual async Task<T> ReplaceAsync(
            T entity,
            FilterDefinition<T> filter,
            FindOneAndReplaceOptions<T> options = null,
            IClientSessionHandle session = null)
        {
            T oldRecord = await GetAsync(filter);
            entity.CreatedAt = oldRecord.CreatedAt;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.DeletedAt = oldRecord.DeletedAt;
            entity.DeletedBy = oldRecord.DeletedBy;
            entity.IsDeleted = oldRecord.IsDeleted;
            entity.UpdatedBy = GetMyUserId();
            entity.CreatedBy = oldRecord.CreatedBy;
            if (session is null)
                await _collection.FindOneAndReplaceAsync(filter, entity, options);
            else
                await _collection.FindOneAndReplaceAsync(session, filter, entity, options);

            return entity;
        }

        public virtual async Task<UpdateResult> UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, UpdateOptions options = null, IClientSessionHandle session = null)
        {
            if (session is null)
                return await _collection.UpdateOneAsync(filter, updateDefinition, options);

            return await _collection.UpdateOneAsync(session, filter, updateDefinition, options);
        }

        public virtual async Task<UpdateResult> UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, UpdateOptions options = null, IClientSessionHandle session = null)
        {
            if (session is null)
                return await _collection.UpdateManyAsync(filter, updateDefinition, options);

            return await _collection.UpdateManyAsync(session, filter, updateDefinition, options);
        }

        public IAggregateFluent<T> AggregationOfRepository(AggregateOptions options = null)
        {
            return _collection.Aggregate(options);
        }
    }
}