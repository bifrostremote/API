using System;
using System.Collections.Generic;
using System.Collections;

#nullable disable

namespace BifrostApi.Models
{
    public partial class MachineToken
    {
        public Guid Uid { get; set; }
        public string Token { get; set; }
        public BitArray Active { get; set; }
        public int CreateDate { get; set; }
        public Guid MachineId { get; set; }

        public virtual Machine Machine { get; set; }
    }
}
