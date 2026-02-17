using Microsoft.EntityFrameworkCore;
using MOJ.Modules.UserManagments.Domain.Entities;

namespace MOJ.Modules.UserManagments.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<AppUser> AppUsers { get; }
        DbSet<Role> Roles { get; }
        DbSet<Department> Departments { get; } 
        DbSet<UserDepartment> UserDepartments { get; } 
        DbSet<UserRoleChangeLog> UserRoleChangeLogs { get; }
        DbSet<PasswordChangeLog> PasswordChangeLogs { get; }
        DbSet<PasswordHistory> PasswordHistories { get; }
        DbContext DbContext { get; } 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Adding transaction support
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default);
    }
}
