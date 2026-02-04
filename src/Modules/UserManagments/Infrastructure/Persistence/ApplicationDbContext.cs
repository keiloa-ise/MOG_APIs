using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;
using MOJ.Modules.UserManagments.Domain.Entities;

namespace MOJ.Modules.UserManagments.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoleChangeLog> UserRoleChangeLogs { get; set; }
        public DbSet<PasswordChangeLog> PasswordChangeLogs { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        public DbContext DbContext => this;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // تنفيذ BeginTransactionAsync
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Database.BeginTransactionAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AppUser Configuration
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_AppUsers_Email");

                entity.HasIndex(u => u.Username)
                    .IsUnique()
                    .HasDatabaseName("IX_AppUsers_Username");

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()")
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.UpdatedAt)
                    .ValueGeneratedOnUpdate()
                    .IsRequired(false);

                entity.Property(u => u.IsActive)
                    .HasDefaultValue(true);

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.FullName)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(u => u.PhoneNumber)
                    .HasMaxLength(20)
                    .IsRequired(false);

                // علاقة مع Role
                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Role Configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(r => r.Name)
                    .IsUnique()
                    .HasDatabaseName("IX_Roles_Name");

                entity.Property(r => r.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()")
                    .ValueGeneratedOnAdd();

                entity.Property(r => r.UpdatedAt)
                    .ValueGeneratedOnUpdate()
                    .IsRequired(false);

                entity.Property(r => r.IsActive)
                    .HasDefaultValue(true);

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.Description)
                    .HasMaxLength(200)
                    .IsRequired(false);

                // Seed data للـ Roles
                entity.HasData(
                    new Role(1, Role.DefaultRoles.SuperAdmin, "System Super Administrator"),
                    new Role(2, Role.DefaultRoles.Admin, "System Administrator"),
                    new Role(3, Role.DefaultRoles.User, "Regular User"),
                    new Role(4, Role.DefaultRoles.Manager, "Department Manager"),
                    new Role(5, Role.DefaultRoles.Editor, "Content Editor"),
                    new Role(6, Role.DefaultRoles.Viewer, "Content Viewer")
                );
            });
            // UserRoleChangeLog Configuration
            modelBuilder.Entity<UserRoleChangeLog>(entity =>
            {
                entity.HasIndex(l => l.UserId);
                entity.HasIndex(l => l.ChangedByUserId);
                entity.HasIndex(l => l.CreatedAt);

                entity.Property(l => l.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                // علاقات مع AppUser
                entity.HasOne(l => l.User)
                    .WithMany()
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.ChangedByUser)
                    .WithMany()
                    .HasForeignKey(l => l.ChangedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // علاقات مع Role
                entity.HasOne(l => l.PreviousRole)
                    .WithMany()
                    .HasForeignKey(l => l.PreviousRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.NewRole)
                    .WithMany()
                    .HasForeignKey(l => l.NewRoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // PasswordChangeLog Configuration
            modelBuilder.Entity<PasswordChangeLog>(entity =>
            {
                entity.HasIndex(p => p.UserId);
                entity.HasIndex(p => p.CreatedAt);

                entity.Property(p => p.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.ChangedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.ChangedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PasswordHistory Configuration
            modelBuilder.Entity<PasswordHistory>(entity =>
            {
                entity.HasIndex(p => p.UserId);
                entity.HasIndex(p => p.CreatedAt);

                entity.Property(p => p.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
