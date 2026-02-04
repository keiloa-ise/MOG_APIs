using Microsoft.EntityFrameworkCore;
using MOJ.Modules.UserManagments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<AppUser> AppUsers { get; }
        DbSet<Role> Roles { get; }
        DbSet<UserRoleChangeLog> UserRoleChangeLogs { get; }
        DbContext DbContext { get; } 
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // إضافة لـ transaction support
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default);
    }
}
