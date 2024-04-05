using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApi.Entities;

namespace UserServiceApi.Configurations
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
            builder.Property(m => m.Password)
                .IsRequired();

            #region many-to-many configurations
            builder.HasMany(m => m.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<UserRoles>(
                    x => x.HasOne(mr => mr.Role).WithMany(r => r.UserRoles).HasForeignKey(mr => mr.RoleId),
                    x => x.HasOne(mr => mr.User).WithMany(m => m.UserRoles).HasForeignKey(mr => mr.UserId),
                    x => x.HasKey(mr => mr.Id)
                );
            #endregion
        }
    }
}
