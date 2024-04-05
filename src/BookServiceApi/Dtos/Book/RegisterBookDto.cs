using CityLibrary.Shared.SharedEnums;

namespace BookServiceApi.Dtos.Book
{
    public class RegisterBookDto
    {
        public string Author { get; set; }

        public string BookTitle { get; set; }

        public DateTime FirstPublishDate { get; set; }

        public short EditionNumber { get; set; }

        public DateTime EditionDate { get; set; }

        public BookTitleTypes TitleType { get; set; }

        public BookCoverTypes CoverType { get; set; }

        public short AvailableCount { get; set; }
    }
}
