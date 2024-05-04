using Microsoft.EntityFrameworkCore;
using BookServiceApi.ContextRelated;
using BookServiceApi.Entities;
using CityLibrary.Shared.SharedEnums;

namespace BookServiceApi.IntegrationTests.Helpers
{
    public static class DbHelpers
    {
        public static void InitDbForTests(AppDbContext db)
        {
            db.Books.AddRange(GetBooksForTest());
            db.SaveChanges();
        }

        public static async Task ReinitDbForTests(AppDbContext db)
        {
            await db.Books.ExecuteDeleteAsync();
            db.SaveChanges();
            InitDbForTests(db);
        }

        private static IEnumerable<Book> GetBooksForTest() 
        {
            return
            [
                new Book
                {
                    BookId = 1,
                    BookTitle = "Ailenin, Devletin ve Özel Mülkiyetin Kökeni",
                    Author = "Friedrich Engels",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-138),
                    EditionNumber = 4,
                    EditionDate = DateTime.UtcNow.AddYears(-120),
                    TitleType = BookTitleTypes.Science,
                    CoverType = BookCoverTypes.HardCover,
                    AvailableCount = 3,
                    ReservedCount = 0
                },
                new Book
                {
                    BookId = 2,
                    BookTitle = "Beyoğlu Rapsodisi",
                    Author = "Ahmet Ümit",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-19),
                    EditionNumber = 4,
                    EditionDate = DateTime.UtcNow.AddYears(-5),
                    TitleType = BookTitleTypes.Literature,
                    CoverType = BookCoverTypes.HardCover,
                    AvailableCount = 4,
                    ReservedCount = 0
                },
                new Book
                {
                    BookId = 3,
                    BookTitle = "Beyoğlu Rapsodisi",
                    Author = "Ahmet Ümit",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-19),
                    EditionNumber = 3,
                    EditionDate = DateTime.UtcNow.AddYears(-10),
                    TitleType = BookTitleTypes.Literature,
                    CoverType = BookCoverTypes.HardCover,
                    AvailableCount = 3,
                    ReservedCount = 0
                },
                new Book
                {
                    BookId = 4,
                    BookTitle = "Thomas' Calculus",
                    Author = "George Brinton Thomas",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-70),
                    EditionNumber = 13,
                    EditionDate = DateTime.UtcNow.AddYears(-5),
                    TitleType = BookTitleTypes.Math,
                    CoverType = BookCoverTypes.SoftCover,
                    AvailableCount = 500,
                    ReservedCount = 0
                },
                new Book
                {
                    BookId = 5,
                    BookTitle = "Thomas' Calculus",
                    Author = "George Brinton Thomas",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-70),
                    EditionNumber = 13,
                    EditionDate = DateTime.UtcNow.AddYears(-5),
                    TitleType = BookTitleTypes.Math,
                    CoverType = BookCoverTypes.HardCover,
                    AvailableCount = 50,
                    ReservedCount = 0
                }
            ];
        }

    }
}