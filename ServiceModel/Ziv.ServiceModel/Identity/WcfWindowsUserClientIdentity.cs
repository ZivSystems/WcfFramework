using System.Security.Principal;

namespace Ziv.ServiceModel.Identity
{
    public class WcfWindowsUserClientIdentity : WcfClientIdentity, IWindowsUserClientIdentity
    {
        public WcfWindowsUserClientIdentity(WindowsIdentity identity)
            : base(identity) { }
    }
}
