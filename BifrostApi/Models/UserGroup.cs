using System;
using System.Collections.Generic;
using System.Collections;

#nullable disable

namespace BifrostApi.Models
{
    public partial class UserGroup
    {
        public UserGroup()
        {
            InverseParentNavigation = new HashSet<UserGroup>();
            Users = new HashSet<User>();
        }

        public Guid Uid { get; set; }
        public string Name { get; set; }
        public Guid? Parent { get; set; }
        public BitArray Deleted { get; set; }

        public virtual UserGroup ParentNavigation { get; set; }
        public virtual ICollection<UserGroup> InverseParentNavigation { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
