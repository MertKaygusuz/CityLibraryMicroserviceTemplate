using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserServiceApi.Entities;

namespace UserServiceApi.Configurations
{
    internal class RolesConfiguration : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            builder.HasKey(r => r.RoleId);
            builder.HasIndex(r => r.RoleName)
                .IsUnique();
            builder.Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(20);

            //builder.HasMany(m => m.Members)
            //    .WithMany(r => r.Roles)
            //    .UsingEntity<MemberRoles>(
            //        x => x.HasOne(mr => mr.Member).WithMany(m => m.MemberRoles),
            //         x => x.HasOne(mr => mr.Role).WithMany(r => r.MemberRoles)
            //    );
        }
    }
}
