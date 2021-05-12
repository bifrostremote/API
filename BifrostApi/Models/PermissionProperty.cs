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
    }
}
