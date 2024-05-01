using BookReservationReportApi.ContextRelated;
using BookReservationReportApi.Entities;
using CityLibrary.Shared.Extensions;
using CityLibrary.Shared.SharedEnums;
using MongoDB.Bson;

namespace BookReservationReportApi.IntegrationTests.Helpers;

public static class DbHelpers
{
    public static void InitDbForTests(AppDbContext context)
    {
        var activeBookReservationCollection = context.Database.Collection<ActiveBookReservation>();
        var bookReservationHistoryCollection = context.Database.Collection<BookReservationHistory>();
        activeBookReservationCollection.InsertMany(GetActiveBookReservationsForTest());
        bookReservationHistoryCollection.InsertMany(GetBookReservationHistoriesForTest());
    }

    public static async Task ReinitDbForTests(AppDbContext context)
    {
        var activeBookReservationCollection = context.Database.Collection<ActiveBookReservation>();
        var bookReservationHistoryCollection = context.Database.Collection<BookReservationHistory>();
        await activeBookReservationCollection.DeleteManyAsync(new BsonDocument());
        await bookReservationHistoryCollection.DeleteManyAsync(new BsonDocument());
        InitDbForTests(context);
    }
    private static IEnumerable<ActiveBookReservation> GetActiveBookReservationsForTest() 
    {
        return
        [
            new ActiveBookReservation() 
            {
                UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                BookId = 1,
                User = new() 
                {
                    UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                    UserName = "User2",
                    FullName = "Kaya",
                    BirthDate = DateTime.UtcNow.AddYears(-40),
                    Address = "Kaya's Address"
                },
                Book = new()
                {
                    BookId = 1,
                    BookTitle = "Ailenin, Devletin ve Özel Mülkiyetin Kökeni",
                    Author = "Friedrich Engels",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-138),
                    EditionNumber = 4,
                    EditionDate = DateTime.UtcNow.AddYears(-120),
                    TitleType = BookTitleTypes.Science,
                    CoverType = BookCoverTypes.HardCover
                },
                DeliveryDateToUser = DateTime.UtcNow.AddDays(-4)
            },
            new ActiveBookReservation() 
            {
                UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                BookId = 2,
                User = new() 
                {
                    UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                    UserName = "User2",
                    FullName = "Kaya",
                    BirthDate = DateTime.UtcNow.AddYears(-40),
                    Address = "Kaya's Address",
                },
                Book = new()
                {
                    BookId = 2,
                    BookTitle = "Beyoğlu Rapsodisi",
                    Author = "Ahmet Ümit",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-19),
                    EditionNumber = 4,
                    EditionDate = DateTime.UtcNow.AddYears(-5),
                    TitleType = BookTitleTypes.Literature,
                    CoverType = BookCoverTypes.HardCover
                },
                DeliveryDateToUser = DateTime.UtcNow.AddDays(-2)
            },
            new ActiveBookReservation() 
            {
                UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
                BookId = 2,
                User = new() 
                {
                    UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
                    UserName = "User1",
                    FullName = "Orhan",
                    BirthDate = DateTime.UtcNow.AddYears(-30),
                    Address = "Orhan's Address"
                },
                Book = new()
                {
                    BookId = 2,
                    BookTitle = "Beyoğlu Rapsodisi",
                    Author = "Ahmet Ümit",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-19),
                    EditionNumber = 4,
                    EditionDate = DateTime.UtcNow.AddYears(-5),
                    TitleType = BookTitleTypes.Literature,
                    CoverType = BookCoverTypes.HardCover
                },
                DeliveryDateToUser = DateTime.UtcNow.AddDays(-6)
            }
        ];
    }

    private static IEnumerable<BookReservationHistory> GetBookReservationHistoriesForTest() 
    {
        return
        [
            new BookReservationHistory() 
            {
                UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3",
                BookId = 1,
                User = new() 
                {
                    UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3",
                    UserName = "User3",
                    FullName = "Kadriye",
                    BirthDate = DateTime.UtcNow.AddYears(-20),
                    Address = "Kadriye's Address"
                },
                Book = new()
                {
                    BookId = 1,
                    BookTitle = "Ailenin, Devletin ve Özel Mülkiyetin Kökeni",
                    Author = "Friedrich Engels",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-138),
                    EditionNumber = 4,
                    EditionDate = DateTime.UtcNow.AddYears(-120),
                    TitleType = BookTitleTypes.Science,
                    CoverType = BookCoverTypes.HardCover
                },
                DeliveryDateToUser = DateTime.UtcNow.AddDays(-40),
                RecievedDate = DateTime.UtcNow.AddDays(-20)
            },
            new BookReservationHistory() 
            {
                UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3",
                BookId = 2,
                User = new() 
                {
                    UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3",
                    UserName = "User3",
                    FullName = "Kadriye",
                    BirthDate = DateTime.UtcNow.AddYears(-20),
                    Address = "Kadriye's Address"
                },
                Book = new()
                {
                    BookId = 2,
                    BookTitle = "Beyoğlu Rapsodisi",
                    Author = "Ahmet Ümit",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-19),
                    EditionNumber = 4,
                    EditionDate = DateTime.UtcNow.AddYears(-5),
                    TitleType = BookTitleTypes.Literature,
                    CoverType = BookCoverTypes.HardCover
                },
                DeliveryDateToUser = DateTime.UtcNow.AddDays(-12),
                RecievedDate = DateTime.UtcNow.AddDays(-3)
            },
            new BookReservationHistory() 
            {
                UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
                BookId = 5,
                User = new() 
                {
                    UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
                    UserName = "User1",
                    FullName = "Orhan",
                    BirthDate = DateTime.UtcNow.AddYears(-30),
                    Address = "Orhan's Address",
                },
                Book = new()
                {
                    BookId = 5,
                    BookTitle = "Thomas' Calculus",
                    Author = "George Brinton Thomas",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-70),
                    EditionNumber = 13,
                    EditionDate = DateTime.UtcNow.AddYears(-5),
                    TitleType = BookTitleTypes.Math,
                    CoverType = BookCoverTypes.HardCover
                },
                DeliveryDateToUser = DateTime.UtcNow.AddDays(-22),
                RecievedDate = DateTime.UtcNow.AddDays(-13)
            },
            new BookReservationHistory() 
            {
                UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                BookId = 5,
                User = new() 
                {
                    UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                    UserName = "User2",
                    FullName = "Kaya",
                    BirthDate = DateTime.UtcNow.AddYears(-40),
                    Address = "Kaya's Address"
                },
                Book = new()
                {
                    BookId = 5,
                    BookTitle = "Thomas' Calculus",
                    Author = "George Brinton Thomas",
                    FirstPublishDate = DateTime.UtcNow.AddYears(-70),
                    EditionNumber = 13,
                    EditionDate = DateTime.UtcNow.AddYears(-5),
                    TitleType = BookTitleTypes.Math,
                    CoverType = BookCoverTypes.HardCover
                },
                DeliveryDateToUser = DateTime.UtcNow.AddDays(-120),
                RecievedDate = DateTime.UtcNow.AddDays(-100)
            }
        ];
    }
}
