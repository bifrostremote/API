using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.Models
{
    public class PasswordKey
    {
        public Guid Uid { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public User UserNavigation { get; set; }
    }
}
