using System;
using Ziv.ServiceModel.Identity.Forwarding;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Client
{
    public class WcfIdentityForwardingProxy<TContract> : WcfProxy<TContract>
    {
        private IForwardingContext _forwardingContext;

        public WcfIdentityForwardingProxy(IForwardingContext forwardingContext, IWcfChannelFactoryProvider wcfChannelFactoryProvider, string endpointName = null)
            : base(wcfChannelFactoryProvider, endpointName) 
        {
            _forwardingContext = forwardingContext;
        }

        public override TResult Execute<TResult>(Func<TContract, TResult> operation)
        {
            Logger.LogEvent(string.Format("Executing operation in proxy of type {0} with identity forwarding.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            using (_forwardingContext.ApplyForwardingStrategy())
            {
                Logger.LogEvent(string.Format("Executing operation in proxy of type {0} - forwarding strategy has been applied.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
                return base.Execute<TResult>(operation);
            }
        }
    }
}
