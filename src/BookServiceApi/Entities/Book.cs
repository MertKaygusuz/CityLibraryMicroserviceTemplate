using CityLibrary.Shared.DbBase.SQL;
using CityLibrary.Shared.SharedEnums;

namespace BookServiceApi.Entities
{
    public class Book : TableBase
    {
        public int BookId { get; set; }

        public string Author { get; set; }

        public string BookTitle { get; set; }

        public DateTime FirstPublishDate { get; set; }

        public short EditionNumber { get; set; }

        public DateTime EditionDate { get; set; }

        public BookTitleTypes TitleType { get; set; }

        public BookCoverTypes CoverType  { get; set; }

        public short AvailableCount { get; set; }

        public short ReservedCount { get; set; }
    }
}