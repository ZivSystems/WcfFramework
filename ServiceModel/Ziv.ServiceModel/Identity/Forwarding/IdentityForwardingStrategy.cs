using System;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Identity.Forwarding
{
    public abstract class IdentityForwardingStrategyBase : IDisposable
    {
        protected IClientIdentity _clientIdentity;

        protected IdentityForwardingStrategyBase(IClientIdentity clientIdentity)
        {
            Logger.LogEvent(string.Format("Consructing IdentityForwardingStrategy with client identity '{0}'.", clientIdentity.Identity.Name), this, ImportanceLevels.gUnimportant);
            if (clientIdentity==null)
            {
                throw new ArgumentNullException("clientIdentity");
            }
            _clientIdentity = clientIdentity;
            StartForwarding();
            Logger.LogEvent(string.Format("Forwarding client identity '{0}' has been started.", clientIdentity.Identity.Name), this, ImportanceLevels.gUnimportant);
        }

        protected abstract void StartForwarding();

        public abstract void StopForwarding();

        public void Dispose()
        {
            Logger.LogEvent(string.Format("Disposing IdentityForwardingStrategy of client identity '{0}'.", _clientIdentity.Identity.Name), this, ImportanceLevels.gUnimportant);
            StopForwarding();
            Logger.LogEvent(string.Format("IdentityForwardingStrategy of client identity '{0}' has been stopped.", _clientIdentity.Identity.Name), this, ImportanceLevels.gUnimportant);
        }
    }
}
