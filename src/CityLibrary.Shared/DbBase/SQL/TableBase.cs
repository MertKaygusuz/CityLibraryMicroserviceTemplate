using System.ComponentModel.DataAnnotations;

namespace CityLibrary.Shared.DbBase.SQL
{
    public abstract class TableBase : ISoftDeletable
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        [MaxLength(36)]
        public string CreatedBy { get; set; }

        [MaxLength(36)]
        public string LastUpdatedBy { get; set; }

        [MaxLength(36)]
        public string DeletedBy { get; set; }
    }
}
