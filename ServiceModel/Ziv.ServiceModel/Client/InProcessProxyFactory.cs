using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Client
{
    public class InProcessProxyFactory<TContract> : IProxyFactory<TContract>
    {
        private readonly TContract _serviceInstance;

        public InProcessProxyFactory(TContract serviceInstance)
        {
            _serviceInstance = serviceInstance;
            Logger.LogEvent(string.Format("InProcessProxyFactory of type {0} has been constructed.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
        }

        public IProxy<TContract> GetProxy()
        {
            Logger.LogEvent(string.Format("Creating InProcessProxy of type {0}.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            return new InProcessProxy<TContract>(_serviceInstance);
        }
    }
}