using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models.Attributes
{
    // https://stackoverflow.com/questions/61123591/asp-net-web-api-core-route-to-action-based-on-query-string-parameter
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class QueryRouteSelectorAttribute : ActionMethodSelectorAttribute
    {
        public string ParameterSelector { get; set; }
        public bool ShouldPass { get; set; }

        public QueryRouteSelectorAttribute(string parameterSelector, bool shouldPass)
        {
            ParameterSelector = parameterSelector;
            ShouldPass = shouldPass;
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            routeContext.HttpContext.Request.Query.TryGetValue(ParameterSelector, out StringValues value);


            if (ShouldPass)
                return !StringValues.IsNullOrEmpty(value);

            return StringValues.IsNullOrEmpty(value);
        }
    }
}
