using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace BifrostApi.Models
{
    public partial class MachineToken
    {
        public Guid Uid { get; set; }
        public string Token { get; set; }

        [Column("Active")]
        private BitArray _active { get; set; }

        [NotMapped]
        public bool Active
        {
            get
            {
                return _active[0];
            }
            set
            {
                _active[0] = value;
            }
        }
        public int CreateDate { get; set; }
        public Guid MachineId { get; set; }

        public virtual Machine Machine { get; set; }
    }
}
