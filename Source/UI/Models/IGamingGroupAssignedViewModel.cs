using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models
{
    public interface IGamingGroupAssignedViewModel
    {
        string GamingGroupName { get; set; }
        int GamingGroupId { get; set; }
    }
}
