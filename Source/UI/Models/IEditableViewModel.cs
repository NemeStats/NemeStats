using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Models.User;

namespace UI.Models
{
    public interface IEditableViewModel
    {
        bool UserCanEdit { get; set; }
    }
}
