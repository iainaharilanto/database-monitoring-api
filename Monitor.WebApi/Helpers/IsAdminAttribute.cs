namespace Monitor.WebApi.Helpers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Monitor.WebApi.Entities;
    using Monitor.WebApi.Models;
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IsAdminAttribute : IsAuthorizedAttribute
    {
        public override void OnAuthorization(AuthorizationFilterContext context)
        {
            base.OnAuthorization(context);
            if (context.Result == null)
            {
                var activeProject = (ProjectRole)context.HttpContext.Items["ActiveProject"];
                if (activeProject.Role != Role.Admin)
                {
                    // not logged in
                    context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
                }
            }
        }
    }
}
