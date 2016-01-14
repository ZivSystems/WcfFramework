using System.Security.Principal;
using System.ServiceModel;

namespace Ziv.ServiceModel.Identity
{
    public class WcfClientIdentity : ClientIdentity
    {
        private IIdentity _identity;
        private WindowsIdentity _windowsIdentity;

        // Gets IIdentity instance directly - intended for client-side usage.
        public WcfClientIdentity(IIdentity identity)
        {
            _identity = identity;
            _windowsIdentity = identity as WindowsIdentity;
        }

        // Gets IIdentity by ServiceSecurityContext - intended for server-side usage.
        public WcfClientIdentity(ServiceSecurityContext serviceSecurityContext)
        {
            _identity = serviceSecurityContext.PrimaryIdentity;
            _windowsIdentity = serviceSecurityContext.WindowsIdentity;
        }

        public override IIdentity Identity
        {
            get { return _identity; }
        }

        public WindowsIdentity WindowsIdentity
        {
            get { return _windowsIdentity; }
        }
    }
}
