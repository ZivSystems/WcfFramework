namespace Ziv.ServiceModel.Client
{
    public interface IProxyFactory<out TContract>
    {
        IProxy<TContract> GetProxy();
    }
}
