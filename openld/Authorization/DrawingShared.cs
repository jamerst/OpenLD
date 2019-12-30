using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

using openld.Services;

namespace openld.Authorization {
    public sealed class DrawingShared : Attribute, IAuthorizationFilter {
        private AccessContext context;
        public DrawingShared(AccessContext context) {
            this.context = context;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext) {
            switch (context) {
                case AccessContext.Drawing:
                    if (filterContext.RouteData.Values.ContainsKey("drawingId") && filterContext.RouteData.Values["drawingId"] is string) {
                        string drawingId = (string) filterContext.RouteData.Values["drawingId"];
                        var user = filterContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                        var drawingService = filterContext.HttpContext.RequestServices.GetService<IDrawingService>();

                        if (!drawingService.isSharedWith(drawingId, user) && !drawingService.isOwner(drawingId, user)) {
                            filterContext.Result = new UnauthorizedResult();
                        }
                    } else {
                        filterContext.Result = new NotFoundResult();
                    }
                    return;
                default:
                    filterContext.Result = new ForbidResult();
                    return;
            }
        }
    }
}