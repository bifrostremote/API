using BifrostApi.Models;
using BifrostApi.Models.Attributes;
using BifrostApi.Session;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.BusinessLogic
{
    public class HierarchyMiddleware
    {
        private readonly RequestDelegate _next;

        public HierarchyMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        private bifrostContext _context;
        private const string TOP_LEVEL_GROUP = "8668e99b-0bde-4c99-9269-03019cb1d822";

        public async Task Invoke(HttpContext httpContext, bifrostContext context)
        {
            // Store the context in a class field
            // The reason that this is not done through dependency injection is because our context is a scoped instance
            // and all ASP.NET middleware is effectively a singleton by being constructed at app startup.
            _context = context;

            // Get information about endpoint route.
            var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;

            // Query endpoint metadata for permission annotations if any.
            // cast the found metadata to the correct type.
            List<RequireHierarchyAttribute> requestMetadata = endpoint?.Metadata.Where(x => x.GetType() == typeof(RequireHierarchyAttribute))
                .Cast<RequireHierarchyAttribute>().ToList();

            // Get the first RequireHierarchyAttribute, we dont need to think about multiple attributes due to attribute settings
            // disallowing it.
            RequireHierarchyAttribute attribute = endpoint?.Metadata.GetMetadata<RequireHierarchyAttribute>();


            if (attribute != null)
            {
                Session.Session session = SessionHelper.GetCurrentSession(httpContext.Session);

                // Check if session is initialized and session is authenticated
                if (session != null && SessionHelper.IsSessionAuthenticated(httpContext.Session))
                {
                    Guid parameterData = Guid.Parse(httpContext.Request.Query[attribute.HierarchySearchParameter]);
                    Guid CurrentGroupUid = session.CurrentUser.UserGroupUid;

                    UserGroup currentSearchingGroup = new UserGroup();



                    // We switch between searching for users to get their group or search by the groupUid.
                    switch (attribute.SearchType)
                    {
                        case RequireHierarchyAttribute.HierarchySearchType.User:
                            currentSearchingGroup = _context.UserGroups.Include(a => a.ParentNavigation).Where(x => x.Users.Where(e => e.Id == parameterData).Count() > 0).FirstOrDefault();
                            break;

                        case RequireHierarchyAttribute.HierarchySearchType.Usergroup:
                            currentSearchingGroup = _context.UserGroups.Where(x => x.Uid == parameterData).FirstOrDefault();
                            break;
                    }

                    // No group found, defaulting to unauthorized
                    if (currentSearchingGroup == null)
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }


                    if (!SearchHierarchy(currentSearchingGroup, CurrentGroupUid, context))
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }
                else
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await _next(httpContext);
        }

        // TODO: Instead of loading a single parent, we should instead find a way to either load diminish the load that this does or profile the load that an entire load does.
        private UserGroup LoadParent(UserGroup group)
        {
            var matches = _context.UserGroups.Where(x => x.Uid == group.ParentUid).ToList();

            if (matches.Count == 0)
                return null;

            return matches.FirstOrDefault();

        }

        private bool SearchHierarchy(UserGroup groupToSearch, Guid targetParent, bifrostContext context, bool initialSearch = true)
        {
            // If we are running the first layer search, and the current group is the top level parent group
            // always allow access. 
            // In essence an early top level check.
            if (initialSearch && targetParent == Guid.Parse(TOP_LEVEL_GROUP))
                return true;
            

            if (groupToSearch.ParentUid == null)
                return false;

            if (groupToSearch.Uid == targetParent)
                return true;

            var parent = LoadParent(groupToSearch);

            return SearchHierarchy(parent, targetParent, context, false);
        }
    }



    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class HierarchyMiddlewareExtensions
    {
        public static IApplicationBuilder UseHierarchyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HierarchyMiddleware>();
        }
    }
}

