using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Column("Deleted")]
        private BitArray _deleted { get; set; }

        [NotMapped]
        public bool Deleted
        {
            get
            {
                return _deleted[0];
            }
            set
            {
                _deleted[0] = value;
            }
        }

        public virtual UserGroup ParentNavigation { get; set; }
        public virtual ICollection<UserGroup> InverseParentNavigation { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
