using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BifrostApi.Models;

namespace BifrostApi.Session
{
    public partial class Session
    {
        public bool isAuthenticated;
        public User CurrentUser;
        public List<PermissionProperty> permissions;
    }
}
