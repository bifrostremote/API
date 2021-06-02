﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;

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
        public string UserName { get; set; }
        //public PasswordKey PasswordKey { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid UserGroupUid { get; set; }
        public bool Deleted { get; set; }
        public virtual UserGroup UserGroup { get; set; }
        public virtual ICollection<Machine> Machines { get; set; }
    }
}
