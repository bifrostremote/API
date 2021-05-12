using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BifrostApi.Models
{
    public partial class User
    {
        public User()
        {
            Machines = new HashSet<Machine>();
        }

        public Guid Uid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Passwordsalt { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid UserGroupId { get; set; }

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

        public virtual UserGroup UserGroup { get; set; }
        public virtual ICollection<Machine> Machines { get; set; }
    }
}
