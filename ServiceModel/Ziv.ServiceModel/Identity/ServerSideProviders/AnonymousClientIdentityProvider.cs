namespace Ziv.ServiceModel.Identity.ServerSideProviders
{
    public class AnonymousClientIdentityProvider : IClientIdentityProvider
    {
        public IClientIdentity GetClientIdentityForCurrentThread()
        {
            return AnonymousClientIdentity.Instance;
        }

        private AnonymousClientIdentityProvider()
        {

        }

        private static AnonymousClientIdentityProvider _instance = new AnonymousClientIdentityProvider();
        public static AnonymousClientIdentityProvider Instance
        {
            get { return _instance; }
        }
    }
}
