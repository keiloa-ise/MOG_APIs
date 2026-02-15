using Hangfire.Dashboard;

namespace APIs.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // ›Ì »Ì∆… «· ÿÊÌ—° «”„Õ »«·œŒÊ· „Õ·Ì« ›ﬁÿ
            if (httpContext.Request.Host.Host == "localhost")
                return true;

            // ›Ì «·≈‰ «Ã°  Õﬁﬁ „‰ √‰ «·„” Œœ„ „”Ã· œŒÊ·Â Ê·œÌÂ ’·«ÕÌ… Admin
            return httpContext.User.Identity?.IsAuthenticated == true &&
                   httpContext.User.IsInRole("Admin"); // √Ê SuperAdmin
        }
    }
}