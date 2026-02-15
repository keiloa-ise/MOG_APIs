using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOJ.Modules.UserManagments.Application.Common.Interfaces
{
    public interface ICleanupService
    {
        Task CleanExpiredSessions();
        Task CleanOldPasswordHistories(int keepLast = 5);
        Task CleanInactiveUsers(int daysInactive = 90);
        Task CleanOldAuditLogs(int daysToKeep = 30);
    }
}
