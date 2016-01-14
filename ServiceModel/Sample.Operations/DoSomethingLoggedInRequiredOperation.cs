using System.Security.Permissions;
using Sample.DTO;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Sample.Operations
{
    [Operation("Trials")]
    public class DoSomethingLoggedInRequiredOperation : DoSomethingOperation
    {
        public DoSomethingLoggedInRequiredOperation(
            SomeParameters parameters, 
            IOperationsManager operationsManager, 
            ISomeRequiredService requiredService)
            : base(parameters, operationsManager, requiredService)
        {
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        protected override SomeResult Run()
        {
            return base.Run();
        }
    }
}