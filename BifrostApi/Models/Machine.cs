using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

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

        public virtual User User { get; set; }
        public virtual ICollection<MachineToken> MachineTokens { get; set; }
    }
}
