using System.Security.Principal;

namespace Ziv.ServiceModel.Identity
{
    public interface IClientIdentity
    {
        IIdentity Identity { get; }
    }
}
