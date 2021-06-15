using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models.DTO
{
    [Keyless]
    public class MachineHierarchyDTO
    {
        public Guid user_uid { get; set; }
        public string username { get; set; }
        public Guid user_group_id { get; set; }
        public Guid machine_uid { get; set; } 
        public string machine_name { get; set; }
        public string machine_ip { get; set; }
        public string machine_lastonline { get; set; }
    }
}
