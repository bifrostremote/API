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

            // Query metadata for permission annotations if any.
            var requestMetadata = endpoint?.Metadata.GetMetadata<RequiredPermissionAttribute>();
            var test = endpoint?.Metadata.Where(x => x.GetType() == typeof(RequiredPermissionAttribute)).ToList();

            if (requestMetadata != null)
            {
                var session = SessionHelper.GetCurrentSession(httpContext.Session);

                if (session != null && SessionHelper.IsSessionAuthenticated(httpContext.Session))
                {
                    var currentUserGroup = session.CurrentUser.UserGroup;

                    var currentPermissions = dbContext.GroupPermissions.Where(x => x.Group == currentUserGroup.Uid).ToList();

                    foreach (var permission in requestMetadata.RequiredPermission)
                    {
                        //if (currentPermissions.Where(x => x.PermissionPropertyNavigation.Name != permission) != 0)
                        //{

                        //}
                    }
                } else
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                await _next(httpContext);
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
