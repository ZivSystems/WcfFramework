using System;

namespace Ziv.ServiceModel.Operations.OperationsManager
{
    /// <summary>
    /// This managers provides central managment of all running operations in a given services deployment. 
    /// Note results must be globally accurate of all operation across server processes and across servers if required.
    /// </summary>
    public interface IOperationsManager
    {
        Guid RegisterOperation(string displayName, bool isInvokedSynchronically);

        OperationStatus GetOperationStatus(Guid operationId);

        bool GetIsOperationCancelationPending(Guid operationId);

        void SetOperationCancelFlag(Guid operationId);

        void SetOperationResult(Guid operationId, object result);

        void SetOperationProgress(Guid operationId, int progressPercent);

        void SetOperationCanceled(Guid operationId);

        void SetOperationFailed(Guid operationId, Exception ex);
    }
}