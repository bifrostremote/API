using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequireHierarchyAttribute : Attribute
    {
        public bool RequireEqual;
        public string HierarchySearchParameter;
        public HierarchySearchType SearchType;

        public RequireHierarchyAttribute(string searchParameter, bool requireEqual, HierarchySearchType searchType)
        {
            RequireEqual = requireEqual;
            HierarchySearchParameter = searchParameter;
            SearchType = searchType;
        }

        public enum HierarchySearchType
        {
            Usergroup,
            User
        }
    }
}
