using System;
using System.Collections.Generic;

#nullable disable

namespace BifrostApi.Models
{
    public partial class GroupPermission
    {
        public Guid GroupUid { get; set; }
        public Guid PermissionPropertyUid { get; set; }

        public virtual UserGroup GroupNavigation { get; set; }
        public virtual PermissionProperty PermissionPropertyNavigation { get; set; }
    }
}
