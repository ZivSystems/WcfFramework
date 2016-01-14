using System;

namespace Ziv.ServiceModel.Operations.OperationsManager
{
    internal class SingleProcessDeploymentOperationStatus
    {
        public static readonly TimeSpan NoTimeout = TimeSpan.MaxValue;

        public SingleProcessDeploymentOperationStatus(
            Guid operationId,
            string readableOperationId,
            OperationInternalState state = OperationInternalState.Started,
            DateTime? operationStartTime = null,
            int progress = 0,
            object result = null,
            TimeSpan? operationRunTimeout = null,
            TimeSpan? finalStatusRetrievalPendingTimeout = null,
            TimeSpan? storageAfterFinalStatusRetrievedByClientTimeout = null)
        {
            _operationId = OperationId;
            _readableOperationId = readableOperationId;
            State = state;
            OperationStartTime = operationStartTime ?? DateTime.Now;
            Progress = progress;
            Result = result;
            OperationRunTimeout = operationRunTimeout ?? NoTimeout;
            FinalStatusRetrievalPendingTimeout = finalStatusRetrievalPendingTimeout ?? NoTimeout;
            StorageAfterFinalStatusRetrievedByClientTimeout = storageAfterFinalStatusRetrievedByClientTimeout ?? NoTimeout;
        }

        private readonly Guid _operationId;

        public Guid OperationId
        {
            get { return _operationId; }
        }

        private readonly string _readableOperationId;

        public string ReadableOperationId
        {
            get { return _readableOperationId; }
        }

        public OperationInternalState State { get; set; }

        public int Progress { get; set; }

        public object Result { get; set; }

        public Exception Exception { get; set; }

        public DateTime OperationStartTime { get; set; }

        public DateTime? FinalStatusSetTime { get; set; }

        public DateTime? LastClientStatusRetrievalTime { get; set; }

        public bool IsFinalStatusRetrievedByClient { get; set; }

        public TimeSpan OperationRunTimeout { get; set; }

        public TimeSpan FinalStatusRetrievalPendingTimeout { get; set; }

        public TimeSpan StorageAfterFinalStatusRetrievedByClientTimeout { get; set; }

        public OperationStatus GetOperationStatusForClient()
        {
            OperationState? stateForClient = State.GetStateForClient();
            if (!stateForClient.HasValue)
            {
                throw new InvalidOperationException("Cannot get OperationStatus for operation which is in fault state.");
            }
            return new OperationStatus(Result, new OperationStatusInformation(stateForClient.Value, Progress));
        }
    }
}
