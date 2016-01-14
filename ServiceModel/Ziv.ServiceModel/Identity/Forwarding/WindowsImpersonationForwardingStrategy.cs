using System;
using System.Security.Principal;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Identity.Forwarding
{
    public class WindowsImpersonationForwardingStrategy : IdentityForwardingStrategyBase
    {
        WindowsImpersonationContext _impersonationContext;

        public WindowsImpersonationForwardingStrategy(IClientIdentity clientIdentity)
            :base(clientIdentity)
        {  }

        protected override void StartForwarding()
        {
            WindowsIdentity windowsIdentity = _clientIdentity.Identity as WindowsIdentity;
            if (windowsIdentity==null)
            {
                if (_clientIdentity is WcfClientIdentity)
                {
                    windowsIdentity = ((WcfClientIdentity)_clientIdentity).WindowsIdentity;
                }
            }
            if (windowsIdentity==null)
            {
                throw new InvalidOperationException("Impersonation can be performed with windows identity only.");
            }
            Logger.LogEvent(string.Format("Impersonating windows identity of '{0}'.", windowsIdentity.Name), this, ImportanceLevels.gUnimportant);
             _impersonationContext = windowsIdentity.Impersonate();
             Logger.LogEvent(string.Format("Impersonation of windows identity '{0}' has been started.", windowsIdentity.Name), this, ImportanceLevels.gUnimportant);
        }

        public override void StopForwarding()
        {
            Logger.LogEvent("Stopping windows impersonation.", this, ImportanceLevels.gUnimportant);
            _impersonationContext.Undo();
            Logger.LogEvent("Windows impersonation has been undid.", this, ImportanceLevels.gUnimportant);
        }
    }
}
