using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace CityLibrary.Shared.DbBase.Mongo;
using static CityLibrary.Shared.Extensions.TokenExtensions.AccesInfoFromToken;

public class TableBase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string CreatedBy { get; set; } = GetMyUserId();

    public string UpdatedBy { get; set; }

    public string DeletedBy { get; set; }

    public bool IsDeleted { get; set; }
}