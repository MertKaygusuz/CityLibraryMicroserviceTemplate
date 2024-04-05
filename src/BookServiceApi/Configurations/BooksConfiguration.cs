using BookServiceApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookServiceApi.Configurations
{
    internal class BooksConfiguration : IEntityTypeConfiguration<Books>
    {
        public void Configure(EntityTypeBuilder<Books> builder)
        {
            builder.HasKey(b => b.BookId);
            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(b => b.BookTitle)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(b => b.FirstPublishDate)
                .IsRequired();
            builder.Property(b => b.EditionNumber)
                .IsRequired();
            builder.Property(b => b.EditionDate)
                .IsRequired();
            builder.Property(b => b.TitleType)
                .IsRequired();
            builder.Property(b => b.CoverType)
                .IsRequired();
            builder.Property(b => b.AvailableCount)
                .IsRequired();
        }
    }
}
