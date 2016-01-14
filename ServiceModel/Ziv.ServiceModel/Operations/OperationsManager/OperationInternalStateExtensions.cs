namespace Ziv.ServiceModel.Operations.OperationsManager
{
    internal static class OperationInternalStateExtensions
    {
        public static OperationState? GetStateForClient(this OperationInternalState state)
        {
            switch (state)
            {
                case OperationInternalState.Started:
                    return OperationState.Started;
                case OperationInternalState.CompletedSucessfully:
                    return OperationState.CompletedSucessfully;
                case OperationInternalState.CancelationPending:
                    return OperationState.CancelationPending;
                case OperationInternalState.Canceled:
                    return OperationState.Canceled;
                default:
                    return null;
            }
        }

        public static bool IsFinished(this OperationInternalState state)
        {
            return state == OperationInternalState.CompletedSucessfully
                || state == OperationInternalState.Failed
                || state == OperationInternalState.Canceled;
        }
    }
}
