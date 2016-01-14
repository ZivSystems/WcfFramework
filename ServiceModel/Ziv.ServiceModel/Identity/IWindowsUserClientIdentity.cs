using System.Security.Principal;

namespace Ziv.ServiceModel.Identity
{
    internal interface IWindowsUserClientIdentity
    {
        WindowsIdentity WindowsIdentity { get; }
    }
}
