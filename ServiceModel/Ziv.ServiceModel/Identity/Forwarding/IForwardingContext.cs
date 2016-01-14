namespace Ziv.ServiceModel.Identity.Forwarding
{
    public interface IForwardingContext
    {
        IClientIdentity ClientIdentity { get; set; }
        IdentityForwardingStrategyBase ApplyForwardingStrategy();
    }
}
