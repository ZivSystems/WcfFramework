using System.Security.Principal;
using System.Threading;

namespace Ziv.ServiceModel.Identity
{
    public class InProcessClientIdentity : ClientIdentity
    {
        // Identity represented by this class is determined when instance constructed, 
        // and is not affected by impersonations that occur afterwards.
        private IIdentity _identity;

        public InProcessClientIdentity()
        {
            _identity = Thread.CurrentPrincipal.Identity;
        }

        public override IIdentity Identity
        {
            get { return _identity; }
        }
    }
}
