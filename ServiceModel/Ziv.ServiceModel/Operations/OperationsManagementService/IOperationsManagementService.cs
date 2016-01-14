using System;

namespace Ziv.ServiceModel.Operations.OperationsManagementService
{
    public interface IOperationsManagementService
    {
        void SetOperationCancelFlag(Guid operationId);

        Guid[] GetOperationsCompleted();

        // for feuture use for polling multiple requests for progress and results:
        // OperationStatus[] GetPorationsResults(Guid[] ProcessesIds)
    }
}
