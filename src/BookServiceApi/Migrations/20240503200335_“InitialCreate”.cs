using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookServiceApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Author = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BookTitle = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstPublishDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EditionNumber = table.Column<short>(type: "smallint", nullable: false),
                    EditionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TitleType = table.Column<int>(type: "int", nullable: false),
                    CoverType = table.Column<int>(type: "int", nullable: false),
                    AvailableCount = table.Column<short>(type: "smallint", nullable: false),
                    ReservedCount = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastUpdatedBy = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedBy = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BirthDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Address = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastUpdatedBy = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedBy = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Author", "AvailableCount", "BookTitle", "CoverType", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "EditionDate", "EditionNumber", "FirstPublishDate", "LastUpdatedAt", "LastUpdatedBy", "ReservedCount", "TitleType" },
                values: new object[,]
                {
                    { 1, "Friedrich Engels", (short)3, "Ailenin, Devletin ve Özel Mülkiyetin Kökeni", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(1904, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4640), (short)4, new DateTime(1886, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4630), null, null, (short)0, 3 },
                    { 2, "Ahmet Ümit", (short)4, "Beyoğlu Rapsodisi", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(2019, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4640), (short)4, new DateTime(2005, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4640), null, null, (short)0, 1 },
                    { 3, "Ahmet Ümit", (short)3, "Beyoğlu Rapsodisi", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(2014, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4640), (short)3, new DateTime(2005, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4640), null, null, (short)0, 1 },
                    { 4, "George Brinton Thomas", (short)500, "Thomas' Calculus", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(2019, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4640), (short)13, new DateTime(1954, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4640), null, null, (short)0, 2 },
                    { 5, "George Brinton Thomas", (short)50, "Thomas' Calculus", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, new DateTime(2019, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4650), (short)13, new DateTime(1954, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4650), null, null, (short)0, 2 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "BirthDate", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "FullName", "LastUpdatedAt", "LastUpdatedBy", "UserName" },
                values: new object[,]
                {
                    { "1146ae0a-cdf3-4822-a691-98f5da9c3f9e", "Kaya's Address", new DateTime(1984, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4760), new DateTime(2024, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4760), null, null, null, "Kaya", null, null, "User2" },
                    { "739d9fdf-f824-40d8-b909-4586bdc283d3", "Kadriye's Address", new DateTime(2004, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4760), new DateTime(2024, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4760), null, null, null, "Kadriye", null, null, "User3" },
                    { "75a4749d-1090-4ade-894e-2612adcd0c1c", "Orhan's Address", new DateTime(1994, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4760), new DateTime(2024, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4760), null, null, null, "Orhan", null, null, "User1" },
                    { "d964dfdf-7cdc-4a7a-a951-04b540bac28d", "Admin's Address", new DateTime(1994, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4750), new DateTime(2024, 5, 3, 20, 3, 35, 148, DateTimeKind.Utc).AddTicks(4750), null, null, null, "Admin", null, null, "Admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
