using BifrostApi.Models;
using BifrostApi.Models.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BifrostApi.Session;
using Microsoft.EntityFrameworkCore;

namespace BifrostApi.BusinessLogic
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, bifrostContext dbContext)
        {
            

            // Get information about endpoint route.
            var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;

            // Query endpoint metadata for permission annotations if any.
            // cast the found metadata to the correct type.
            List<RequiredPermissionAttribute> requestMetadata = endpoint?.Metadata.Where(x => x.GetType() == typeof(RequiredPermissionAttribute))
                .Cast<RequiredPermissionAttribute>().ToList();

            if (requestMetadata != null && requestMetadata.Count != 0)
            {
                var session = SessionHelper.GetCurrentSession(httpContext.Session);     

                // Check if session is initialized and session is authenticated
                if (session != null && SessionHelper.IsSessionAuthenticated(httpContext.Session))
                {
                    var currentUserGroup = session.CurrentUser.UserGroup;

                    var currentPermissions = dbContext.GroupPermissions.Include(e => e.PermissionPropertyNavigation).Where(x => x.GroupUid == currentUserGroup.Uid).ToList();

                    foreach (var permission in requestMetadata)
                    {
                        // Check for matching permissions to the current checking permission
                        var matchedPermission = currentPermissions.Where(x => x.PermissionPropertyNavigation.Name == permission.RequiredPermission).Count();

                        // if no matching permission was found, session is unauthorized and we return HTTP 401
                        if (matchedPermission == 0)
                        {
                            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }
                    }
                } else
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class PermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PermissionMiddleware>();
        }
    }
}
