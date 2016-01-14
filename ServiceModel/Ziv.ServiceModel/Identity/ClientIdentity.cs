using System;
using System.Security.Principal;

namespace Ziv.ServiceModel.Identity
{
    public abstract class ClientIdentity : IClientIdentity
    {
        public abstract IIdentity Identity { get; }

        public static ClientIdentity Create(IIdentity identity)
        {
            if (identity is WindowsIdentity)
            {
                return new WcfWindowsUserClientIdentity((WindowsIdentity)identity);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
