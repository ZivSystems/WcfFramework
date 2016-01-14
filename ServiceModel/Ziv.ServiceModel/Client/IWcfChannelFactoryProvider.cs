using System.ServiceModel;

namespace Ziv.ServiceModel.Client
{
    public interface IWcfChannelFactoryProvider
    {
        ChannelFactory<TContract> GetChannelFactory<TContract>(string endpointName);
    }
}