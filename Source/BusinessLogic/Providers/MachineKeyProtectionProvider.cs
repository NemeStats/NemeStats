using BusinessLogic.Logic.Security;
using Microsoft.Owin.Security.DataProtection;

namespace BusinessLogic.Providers
{
    public class MachineKeyProtectionProvider : IDataProtectionProvider
    {
        public IDataProtector Create(params string[] purposes)
        {
            return new MachineKeyDataProtector(purposes);
        }
    }
}
