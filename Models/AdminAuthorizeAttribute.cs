using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;
        var role = httpContext.Session.GetString("UserRole"); 

        if (string.IsNullOrEmpty(role) || role != "Admin")
        {
            context.Result = new UnauthorizedObjectResult(new { Message = "Access Denied: Admin Only" });
        }
    }
}
