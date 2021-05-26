using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RequiredPermissionAttribute : Attribute
    {
        public string RequiredPermission;

        public RequiredPermissionAttribute(string permission)
        {
            RequiredPermission = permission;
        }
    }
}
