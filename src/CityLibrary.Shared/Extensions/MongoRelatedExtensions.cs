using System.ComponentModel.DataAnnotations.Schema;
using CityLibrary.Shared.DbBase.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using static CityLibrary.Shared.Extensions.TokenExtensions.AccesInfoFromToken;

namespace CityLibrary.Shared.Extensions;

public static class MongoRelatedExtensions
{
    private static string GetDocumentName(Type type)
    {
        return type.GetCustomAttributes(false)
            .OfType<TableAttribute>()
            .Select(x => x.Name)
            .Single();
    }

    /// <summary>
    /// Provides dynamic type to GetCollection method.
    /// Don't forget to define "Table" attribute on DbEntity classes. Otherwise, it crashes.
    /// </summary>
    public static IMongoCollection<T> Collection<T>(this IMongoDatabase db) where T : TableBase
    {
        return db.GetCollection<T>(GetDocumentName(typeof(T)));
    }

    public static UpdateDefinition<T> OnUpdate<T>(this UpdateDefinition<T> updateQuery, bool isUpsert = false) where T : TableBase
    {
        var myUserId = GetMyUserId();

        var result = updateQuery.Set(x => x.UpdatedAt, DateTime.UtcNow)
            .Set(x => x.UpdatedBy, myUserId);

        if (isUpsert)
        {
            result = result.Set(x => x.CreatedAt, DateTime.UtcNow)
                .Set(x => x.CreatedBy, myUserId);
        }

        return result;
    }

    public static FilterDefinition<T> FilterByObjectId<T>(this IConvertible S)
    { 
        return Builders<T>.Filter.Eq("_id", S is string ? ObjectId.Parse(S as string) : S);
    }

}