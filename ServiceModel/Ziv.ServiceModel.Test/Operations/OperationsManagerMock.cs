using System;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Ziv.ServiceModel.Test.Operations
{
    class OperationsManagerMock : IOperationsManager
    {
        private Guid _singleOperationId;

        private void ValidateOperationId(Guid operationId)
        {
            if (operationId != _singleOperationId)
            {
                throw new ArgumentOutOfRangeException("operationId", "Passed operationId is different from the one-and-only operation id that managed by this mock at a time.");
            }
        }

        private OperationStatus _operationStatus;

        private Exception _operationException;

        public Guid RegisterOperation(string displayName, bool isInvokedSynchronically)
        {
            _singleOperationId = Guid.NewGuid();
            _operationStatus = new OperationStatus(null, new OperationStatusInformation(OperationState.Started));
            return _singleOperationId;
        }

        public OperationStatus GetOperationStatus(Guid operationId)
        {
            ValidateOperationId(operationId);
            if (_operationException!=null)
            {
                throw _operationException;
            }
            else
            {
                // Don't return reference to the on-going object. Instead, clone it to a static-state new object.
                return new OperationStatus(
                    _operationStatus.Result, 
                    new OperationStatusInformation(_operationStatus.Info.State, _operationStatus.Info.Progress));
            }
        }

        public bool GetIsOperationCancelationPending(Guid operationId)
        {
            ValidateOperationId(operationId);
            return _operationStatus.Info.State == OperationState.CancelationPending;
        }

        public void SetOperationCancelFlag(Guid operationId)
        {
            ValidateOperationId(operationId);
            _operationStatus.Info.State = OperationState.CancelationPending;
        }

        public void SetOperationResult(Guid operationId, object result)
        {
            ValidateOperationId(operationId);
            _operationStatus.Info.State = OperationState.CompletedSucessfully;
            _operationStatus.Result = result;
        }

        public void SetOperationProgress(Guid operationId, int progressPercent)
        {
            ValidateOperationId(operationId);
            _operationStatus.Info.Progress = progressPercent;
        }

        public void SetOperationCanceled(Guid operationId)
        {
            ValidateOperationId(operationId);
            _operationStatus.Info.State = OperationState.Canceled;
        }

        public void SetOperationFailed(Guid operationId, Exception ex)
        {
            ValidateOperationId(operationId);
            _operationException = ex;
        }
    }
}
