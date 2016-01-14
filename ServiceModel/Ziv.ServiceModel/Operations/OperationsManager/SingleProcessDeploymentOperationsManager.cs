using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Operations.OperationsManager
{
    public class SingleProcessDeploymentOperationsManager : IOperationsManager
    {
        public static readonly TimeSpan OperationsCollectionTimerIntervalMilliseconds = new TimeSpan(0, 1, 0);
        public static readonly TimeSpan DefaultOperationRunTimeout = new TimeSpan(3, 0, 0, 0);
        public static readonly TimeSpan DefaultFinalStatusRetrievalPendingTimeout = new TimeSpan(0, 30, 0);
        public static readonly TimeSpan DefaultStorageAfterFinalStatusRetrievedByClientTimeout = new TimeSpan(0, 5, 0);
        
        private readonly IDictionary<Guid, SingleProcessDeploymentOperationStatus> _operations =
            new ConcurrentDictionary<Guid, SingleProcessDeploymentOperationStatus>();

        private readonly Timer _operationsCollectionTimer = new Timer();

        public SingleProcessDeploymentOperationsManager()
        {
            Logger.LogEvent("Constructing instance of SingleProcessDeploymentOperationsManager.", this, ImportanceLevels.gUnimportant);
            SetOperationsCollectionTimer();
        }

        private void SetOperationsCollectionTimer()
        {
            Logger.LogEvent("Initializing operations collection timer.", this, ImportanceLevels.gUnimportant);
            _operationsCollectionTimer.Interval = OperationsCollectionTimerIntervalMilliseconds.TotalMilliseconds;
            _operationsCollectionTimer.AutoReset = true;
            _operationsCollectionTimer.Elapsed += new ElapsedEventHandler(OperationsCollectionTimer_Elapsed);
            _operationsCollectionTimer.Start();
            Logger.LogEvent("Operation collection timer has been started.", this, ImportanceLevels.gUnimportant);
        }

        private void OperationsCollectionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<Guid> operationsToCollect = from operationPair in _operations
                                                    let operation = operationPair.Value
                                                    let isFinished = operation.State.IsFinished()
                                                    let now = DateTime.Now
                                                    let isFinishedRertivedTimedout=
                                                                isFinished
                                                                && operation.IsFinalStatusRetrievedByClient
                                                                && now - operation.LastClientStatusRetrievalTime > operation.StorageAfterFinalStatusRetrievedByClientTimeout
                                                    let isFinishedNotRertivedTimedout =
                                                                isFinished
                                                                && !operation.IsFinalStatusRetrievedByClient
                                                                && now - operation.FinalStatusSetTime > operation.FinalStatusRetrievalPendingTimeout
                                                    let isNotFinishedTimedout =
                                                                !isFinished
                                                                && now - operation.OperationStartTime > operation.OperationRunTimeout
                                                    where isFinishedRertivedTimedout || isFinishedNotRertivedTimedout || isNotFinishedTimedout
                                                    select operationPair.Key;
            foreach (var operationId in operationsToCollect.ToArray())
            {
                _operations.Remove(operationId);
                Logger.LogEvent(string.Format("Operation '{0}' has been removed from operations manager.", operationId), this, ImportanceLevels.gUnimportant);
            }
        }

        public Guid RegisterOperation(string readableOperationID, bool isInvokedSynchronically)
        {
            var operationId = Guid.NewGuid();
            var info = new SingleProcessDeploymentOperationStatus(operationId, readableOperationID
                , operationRunTimeout: DefaultOperationRunTimeout
                , finalStatusRetrievalPendingTimeout: DefaultFinalStatusRetrievalPendingTimeout
                , storageAfterFinalStatusRetrievedByClientTimeout: DefaultStorageAfterFinalStatusRetrievedByClientTimeout);
            _operations.Add(operationId, info);
            Logger.LogEvent(string.Format("Operation '{0}' registered by operation manager with the ID '{1}'.", readableOperationID, operationId), this, ImportanceLevels.gUnimportant);
            return operationId;
        }

        public OperationStatus GetOperationStatus(Guid operationId)
        {
            Logger.LogEvent(string.Format("Operation status requested for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
            SingleProcessDeploymentOperationStatus operation;
            if (_operations.TryGetValue(operationId, out operation))
            {
                // Lock is necessary here: since operation's state changing is not an atomic operation
                // (so it subject to lock by itself), read it without lock may retrieve partial or corrupted state.
                lock (operation)
                {
                    operation.LastClientStatusRetrievalTime = DateTime.Now;
                    if (operation.State.IsFinished())
                    {
                        MarkOperationAsFinalStatusRetrievedByClient(operation);
                    }
                    if (operation.State == OperationInternalState.Failed)
                    {
                        if (operation.Exception is FaultException)
                        {
                            Logger.LogEvent(string.Format("Throwing fault exception for failed operation '{0} ({1})'.", operation.ReadableOperationId, operationId), this, ImportanceLevels.gUnimportant);
                            throw operation.Exception;
                        }
                        else
                        {
                            Logger.LogEvent(string.Format("Throwing operation exception for failed operation '{0} ({1})'.", operation.ReadableOperationId, operationId), this, ImportanceLevels.gUnimportant);
                            throw new OperationException(operationId, operation.ReadableOperationId, operation.Exception);
                        }
                    }
                    Logger.LogEvent(string.Format("Returning operation result for operation '{0} ({1})'.", operation.ReadableOperationId, operationId), this, ImportanceLevels.gUnimportant);
                    return operation.GetOperationStatusForClient();
                }
            }
            else
            {
                throw GetClientRequestedOperationNotFoundException(operationId);
            }
        }

        public void SetOperationCancelFlag(Guid operationId)
        {
            Logger.LogEvent(string.Format("Set cancel flag requested for for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
            SingleProcessDeploymentOperationStatus operation;
            if (_operations.TryGetValue(operationId, out operation))
            {
                lock (operation)
                {
                    if (!operation.State.IsFinished())
                    {
                        operation.State = OperationInternalState.CancelationPending;
                        Logger.LogEvent(string.Format("Cancel flag set for operation '{0} ({1})'.", operation.ReadableOperationId, operationId), this, ImportanceLevels.gUnimportant);
                    }
                    else
                    {
                        Logger.LogEvent(string.Format("Cancel flag not set for operation '{0} ({1})', because the operation is already finished.", operation.ReadableOperationId, operationId), this, ImportanceLevels.eLow);
                    }
                    // In case of cancelation request we assume that this is the last client call for this operation, so we can mark it as 'final status has been retrieved'.
                    // When the operation will be canceled completely, it may be collected according to the 'final status has been retrieved' timeout (without waiting for 'final status retrieval pending' timeout).
                    operation.LastClientStatusRetrievalTime = DateTime.Now;
                    MarkOperationAsFinalStatusRetrievedByClient(operation);
                }
            }
            else
            {
                throw GetClientRequestedOperationNotFoundException(operationId);
            }
        }

        public bool GetIsOperationCancelationPending(Guid operationId)
        {
            Logger.LogEvent(string.Format("Is flagged to be canceled inquery issued for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
            SingleProcessDeploymentOperationStatus operation;
            if (_operations.TryGetValue(operationId, out operation))
            {
                return operation.State == OperationInternalState.CancelationPending;
            }
            else
            {
                throw GetServerCallOperationNotFoundException(operationId);
            }
        }

        public void SetOperationCanceled(Guid operationId)
        {
            Logger.LogEvent(string.Format("Set canceled requested for for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
            SingleProcessDeploymentOperationStatus operation;
            if (_operations.TryGetValue(operationId, out operation))
            {
                lock (operation)
                {
                    operation.State = OperationInternalState.Canceled;
                    operation.FinalStatusSetTime = DateTime.Now;
                }
                Logger.LogEvent(string.Format("Canceled set for operation '{0} ({1})' .", operation.ReadableOperationId, operationId), this, ImportanceLevels.dMedium);
            }
            else
            {
                throw GetServerCallOperationNotFoundException(operationId);
            }
        }

        public void SetOperationProgress(Guid operationId, int progressPercent)
        {
            Logger.LogEvent(string.Format("Set progress reuested for for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
            SingleProcessDeploymentOperationStatus operation;
            if (_operations.TryGetValue(operationId, out operation))
            {
                lock (operation)
                {
                    operation.Progress = progressPercent;
                }
                Logger.LogEvent(string.Format("Progrerss of operation '{0} ({1})' set to {2}%.", operation.ReadableOperationId, operationId, progressPercent), this, ImportanceLevels.gUnimportant);
            }
            else
            {
                throw GetServerCallOperationNotFoundException(operationId);
            }
        }

        public void SetOperationResult(Guid operationId, object result)
        {
            Logger.LogEvent(string.Format("Set result requested for for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
            SingleProcessDeploymentOperationStatus operation;
            if (_operations.TryGetValue(operationId, out operation))
            {
                lock (operation)
                {
                    operation.State = OperationInternalState.CompletedSucessfully;
                    operation.Result = result;
                    operation.FinalStatusSetTime = DateTime.Now;
                }
                string resultString;
                if (result == null)
                {
                    resultString = "null";
                }
                else
                {
                    try
                    {
                        resultString = result.ToString();
                    }
                    catch (Exception)
                    {
                        Logger.LogEvent(
                            string.Format("Parsing result of operation '{0} ({1})' failed.", operation.ReadableOperationId,
                                          operationId), this, ImportanceLevels.dMedium);
                        resultString = "{Result string not parsed}";
                    }
                }
                Logger.LogEvent(string.Format("Result of operation '{0} ({1})' set to '{2}'.", operation.ReadableOperationId, operationId, resultString), this, ImportanceLevels.gUnimportant);
            }
            else
            {
                throw GetServerCallOperationNotFoundException(operationId);
            }
        }

        public void SetOperationFailed(Guid operationId, Exception ex)
        {
            Logger.LogEvent(string.Format("Set failed requested for for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
            SingleProcessDeploymentOperationStatus operation;
            if (_operations.TryGetValue(operationId, out operation))
            {
                lock (operation)
                {
                    operation.State = OperationInternalState.Failed;
                    operation.Exception = ex;
                    operation.FinalStatusSetTime = DateTime.Now;
                }
                Logger.LogEvent(string.Format("Operation failure set '{0} ({1})'.", operation.ReadableOperationId, operationId), this, ImportanceLevels.dMedium);
            }
            else
            {
                throw GetServerCallOperationNotFoundException(operationId);
            }
        }

        private void MarkOperationAsFinalStatusRetrievedByClient(SingleProcessDeploymentOperationStatus operation)
        {
            operation.IsFinalStatusRetrievedByClient = true;
            Logger.LogEvent(string.Format("Operation's final status has been retrieved by client '{0} ({1})'.", operation.ReadableOperationId, operation.OperationId), this, ImportanceLevels.dMedium);
        }

        private ArgumentOutOfRangeException GetClientRequestedOperationNotFoundException(Guid operationId)
        {
            string message = string.Format("Operation '{0}' not found.", operationId);
            Logger.LogEvent(message, this, ImportanceLevels.dMedium);
            return new ArgumentOutOfRangeException("operationId", operationId, message);
        }

        private ArgumentOutOfRangeException GetServerCallOperationNotFoundException(Guid operationId)
        {
            string message = string.Format(
                "Operation ID '{0}', supplied by server call, is not found in the server operations manager."
                + "\nThis exception most likely indicates a bug in operation code (for example, changing operation progress after cancelation)."
                , operationId);
            Logger.LogEvent(message, this, ImportanceLevels.aMostImportant);
            return new ArgumentOutOfRangeException("operationId", operationId, message);
        }
    }
}