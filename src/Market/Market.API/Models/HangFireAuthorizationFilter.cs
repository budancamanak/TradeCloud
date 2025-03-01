using Hangfire.Dashboard;

namespace Market.API.Models;

public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true; //bypass for now
        // return HttpContext.Current.User.Identity.IsAuthenticated;
        // //Can use this for NetCore
        // return context.GetHttpContext().User.Identity.IsAuthenticated; 
    }
}