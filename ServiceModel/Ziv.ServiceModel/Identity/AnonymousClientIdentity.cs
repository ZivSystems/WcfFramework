using System.Security.Principal;

namespace Ziv.ServiceModel.Identity
{
    public sealed class AnonymousClientIdentity : ClientIdentity
    {
        IIdentity _anonymousIdentity;        

        public override IIdentity Identity
        {
            get { return _anonymousIdentity; }
        }

        private AnonymousClientIdentity()
        {
            _anonymousIdentity = new GenericIdentity(string.Empty); // Pass an empty string as identity name cause IsAuthenticated property to return false.
        }

        private static AnonymousClientIdentity _instance = new AnonymousClientIdentity();
        public static AnonymousClientIdentity Instance
        {
            get { return _instance; }
        }
    }
}
