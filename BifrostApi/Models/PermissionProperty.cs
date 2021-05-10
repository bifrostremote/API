using System;
using System.Collections.Generic;
using System.Collections;

#nullable disable

namespace BifrostApi.Models
{
    public partial class PermissionProperty
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public BitArray Deleted { get; set; }
    }
}
