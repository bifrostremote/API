using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BifrostApi.BusinessLogic
{
    public class Permissions
    {
        public bool CanExecute(string endpoint)
        {
            return true;
        }
    }
}
