using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackManagement.Domain.Entities;

namespace TrackManagement.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Username).IsRequired().HasMaxLength(100);
        builder.HasIndex(user => user.Username).IsUnique();
        builder.Property(user => user.PasswordHash).IsRequired().HasMaxLength(500);
        builder.Property(user => user.Role).IsRequired().HasMaxLength(50);
        builder.Property(user => user.FailedLoginAttempts).IsRequired();
        builder.Property(user => user.LockedUntilUtc);
    }
}
