using Hangfire;
using MOJ.Modules.UserManagments.Application.Common.Interfaces;

namespace APIs.Jobs
{
    public static class HangfireJobs
    {
        public static void ConfigureRecurringJobs()
        {
            // تنظيف الجلسات المنتهية يومياً
            RecurringJob.AddOrUpdate<ICleanupService>(
                "clean-expired-sessions",
                service => service.CleanExpiredSessions(),
                Cron.Daily(2));

            // تنظيف تاريخ كلمات المرور كل أسبوع
            RecurringJob.AddOrUpdate<ICleanupService>(
                "clean-old-password-histories",
                service => service.CleanOldPasswordHistories(5),
                Cron.Weekly(DayOfWeek.Sunday, 3));

            // تنظيف سجلات التدقيق كل شهر
            RecurringJob.AddOrUpdate<ICleanupService>(
                "clean-old-audit-logs",
                service => service.CleanOldAuditLogs(30),
                Cron.Monthly());
        }

        public static void EnqueueExampleJobs()
        {
            // مهمة فورية
            BackgroundJob.Enqueue(() => Console.WriteLine($"Hangfire/Fire-and-Forget at {DateTime.Now}"));

            // مهمة مؤجلة
            BackgroundJob.Schedule(
                () => Console.WriteLine($"Hangfire/Delayed job at {DateTime.Now}"),
                TimeSpan.FromDays(7));

            // مهمة متسلسلة
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Hangfire/Continuations: Step 1 - Validate Order"));
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Hangfire/Continuations: Step 2 - Process Payment"));
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Hangfire/Continuations: Step 3 - Send Confirmation"));
        }
    }
}
