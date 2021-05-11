﻿using System;
using System.Collections.Generic;
using System.Collections;

#nullable disable

namespace BifrostApi.Models
{
    public partial class Machine
    {
        public Machine()
        {
            MachineTokens = new HashSet<MachineToken>();
        }

        public Guid Uid { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public Guid UserId { get; set; }
        public int LastOnline { get; set; }
        public BitArray Deleted { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<MachineToken> MachineTokens { get; set; }
    }
}