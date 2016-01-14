namespace Ziv.ServiceModel.Operations.OperationsManager
{
    internal enum OperationInternalState
    {
        Started,
        CompletedSucessfully,
        Failed,
        /// <summary>
        /// Operation has been signaled to cancel but has not yet halted.
        /// </summary>
        CancelationPending,
        Canceled,
    }
}
