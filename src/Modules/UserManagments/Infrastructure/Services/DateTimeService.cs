using MOJ.Modules.UserManagments.Application.Common.Interfaces;

namespace MOJ.Modules.UserManagments.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
