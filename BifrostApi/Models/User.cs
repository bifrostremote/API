using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

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
        public bool Deleted { get; set; }
        public virtual UserGroup UserGroup { get; set; }
        public virtual ICollection<Machine> Machines { get; set; }
    }
}
