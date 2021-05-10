using System;
using System.Collections.Generic;

#nullable disable

namespace BifrostApi.Models
{
    public partial class GroupPermission
    {
        public Guid Group { get; set; }
        public Guid PermissionProperty { get; set; }

        public virtual UserGroup GroupNavigation { get; set; }
        public virtual PermissionProperty PermissionPropertyNavigation { get; set; }
    }
}
