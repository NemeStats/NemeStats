using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public class SecuredEntityWithTechnicalKey : EntityWithTechnicalKey, SecuredEntity 
    {
        public override int Id { get; set; }
        public int GamingGroupId { get; set; }
    }
}
