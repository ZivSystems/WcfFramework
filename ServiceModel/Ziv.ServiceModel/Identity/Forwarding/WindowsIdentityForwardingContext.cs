using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Identity.Forwarding
{
    public class WindowsIdentityForwardingContext: IForwardingContext
    {
        public IClientIdentity ClientIdentity { get; set; }

        public IdentityForwardingStrategyBase ApplyForwardingStrategy()
        {
            Logger.LogEvent(string.Format("Appling WindowsImpersonationForwardingStrategy with client identity of '{0}'.", ClientIdentity.Identity.Name), this, ImportanceLevels.gUnimportant);
            return new WindowsImpersonationForwardingStrategy(ClientIdentity);
        }
    }
}
