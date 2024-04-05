using BookServiceApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookServiceApi.Configurations
{
    internal class UsersConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.HasKey(m => m.UserId);

            builder.HasIndex(m => m.UserName)
                   .IsUnique();
        
            builder.Property(m => m.UserName)
                .HasMaxLength(30);

            builder.Property(m => m.FullName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(m => m.BirthDate)
                .IsRequired();
            builder.Property(m => m.Address)
                .IsRequired()
                .HasMaxLength(300);
        }
    }
}
