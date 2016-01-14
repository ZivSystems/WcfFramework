using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ziv.ServiceModel.CodeGeneration.Test
{
    [TestClass]
    public class OperaionsGenrationTools_TestFixture
    {
        public const string SERVICE_SORT_NAME = "Testing";

        [TestMethod]
        public void InvokeAllExtensionMethos_Test()
        {
            foreach (var operationType in OperationsCodeGenerationUtils.GetAllOperationTypes())
            {
                operationType.OperationAttribute();
                operationType.OperationAttributeExists();
                operationType.OperationConstructorInvocation();
                operationType.OperationContractNamespaces();
                var operationParameters = operationType.OperationParameters();
                var operationParametersInvocation = operationParameters.ToInvocation();
                operationType.OperationResultType();
                operationType.OperationServiceNamespaces();
                operationType.OperationServices();
                operationType.OperationShortName();
                operationType.OperationServiceClassName();
                operationType.OperationContractInterfaceName();
                var faultContracts = operationType.OperationFaultContracts();
            }
        }
    }
}
