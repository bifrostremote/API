using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BifrostApi.Models
{
    public partial class PermissionProperty
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }

        public bool Deleted { get; set; }
    }
}
