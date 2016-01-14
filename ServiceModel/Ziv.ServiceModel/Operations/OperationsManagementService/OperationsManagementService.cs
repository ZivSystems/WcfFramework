using System;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Ziv.ServiceModel.Operations.OperationsManagementService
{
    public class OperationsManagementService : IOperationsManagementService
    {
        private readonly IOperationsManager _operationsManager;

        public OperationsManagementService(IOperationsManager operationsManager)
        {
            _operationsManager = operationsManager;
        }

        public Guid[] GetOperationsCompleted()
        {
            throw new NotImplementedException();
        }

        public void SetOperationCancelFlag(Guid operationId)
        {
            _operationsManager.SetOperationCancelFlag(operationId);
        }
    }
}
