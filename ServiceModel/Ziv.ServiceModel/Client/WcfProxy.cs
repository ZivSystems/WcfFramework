using System;
using System.ServiceModel;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Client
{
    /// <summary>
    /// This proxy creates a channel and properly aborts it in case of a communication inssue.
    /// </summary>
    /// <typeparam name="TContract">The service contract interface which this class instance is proxy for.</typeparam>
    public class WcfProxy<TContract> : IProxy<TContract>
    {
        private readonly IWcfChannelFactoryProvider _channelFactoryProvider;
        private string _endpointName;

        public WcfProxy(IWcfChannelFactoryProvider channelFactoryProvider, string endpointName = null)
        {
            Logger.LogEvent(string.Format("Constructing WcfProxyFactory of type {0}.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            _channelFactoryProvider = channelFactoryProvider;
            _endpointName = string.IsNullOrWhiteSpace(endpointName)
                            // When an enpoint name is not provided we assume only one candidate is present 
                            // in the configuration and let the app use it by sepcifiying "*" for the enpoint name
                                ? "*"
                                : endpointName;
        }

        public virtual TResult Execute<TResult>(Func<TContract, TResult> operation)
        {
            Logger.LogEvent(string.Format("Executing WCF operation on contract of type {0}.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            TResult result = default(TResult);
            var channelFactory = _channelFactoryProvider.GetChannelFactory<TContract>(_endpointName);
            Logger.LogEvent("Executing WCF operation - ChannelFactoryProvider has been retrieved.", this, ImportanceLevels.gUnimportant);
            var channel = channelFactory.CreateChannel();
            Logger.LogEvent("Executing WCF operation - channel has been created.", this, ImportanceLevels.gUnimportant);
            var innerChannel = (ICommunicationObject)channel;
            try
            {
                result = operation(channel);
                Logger.LogEvent("WCF operation execution has been completed.", this, ImportanceLevels.gUnimportant);
                innerChannel.Close();
            }
            //catch (CommunicationException)
            //{
            //    innerChannel.Abort();
            //}
            //catch (TimeoutException)
            //{
            //    innerChannel.Abort();
            //}
            catch (Exception)
            {
                Logger.LogEvent("WCF operation execution has failed.", this, ImportanceLevels.dMedium);
                innerChannel.Abort();
                throw;
            }
            return result;
        }

        public void Execute(Action<TContract> operation)
        {
            Execute(new Func<TContract, object>(contract => { operation(contract); return null; }));
        }

        public void Dispose()
        {
            // Resources are disposed during execute.
        }
    }
}