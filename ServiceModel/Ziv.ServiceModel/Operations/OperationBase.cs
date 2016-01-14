using System;
using System.Threading;
using Ziv.ServiceModel.Operations.OperationsManager;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Operations
{
    public abstract class OperationBase<TResult> : IOperation<TResult>
    {
        private IOperationsManager _operationsManager;
        private readonly string _readableOperationId;
        private Guid _operationId;
        private bool _isCompletelyCanceled;

        public OperationBase(IOperationsManager operationsManager, string readableOperationId)
        {
            _operationsManager = operationsManager;
            _readableOperationId = readableOperationId ?? this.GetType().Name;
            Logger.LogEvent(string.Format("Operation '{0}' instance constructed.", _readableOperationId), this, ImportanceLevels.gUnimportant);
        }

        public OperationBase(IOperationsManager operationsManager)
            : this(operationsManager, null) { }

        public OperationStatus RunSync()
        {
            Logger.LogEvent(string.Format("Operation '{0}' invoked sync.", _readableOperationId), this, ImportanceLevels.gUnimportant);
            RegisterOperationInManager(true);
            DoWork();
            var completedOperationStatus = _operationsManager.GetOperationStatus(_operationId);
            Logger.LogEvent(string.Format("Operation '{0} ({1})' sync invocation completed.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
            return completedOperationStatus;
        }

        public OperationEnqueuedToken RunAsync()
        {
            Logger.LogEvent(string.Format("Operation '{0}' invoked async.", _readableOperationId), this, ImportanceLevels.gUnimportant);
            RegisterOperationInManager(false);
            // consider: replace using of AutoResetEvent with the more conventional & API-enhanced Task.
            //var resetHandle = new AutoResetEvent(false);
            ThreadPool.QueueUserWorkItem(o => DoWorkAsyncronously(/*resetHandle*/));
            Logger.LogEvent(string.Format("Operation '{0} ({1})' work queued.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
            return new OperationEnqueuedToken(_operationId/*, resetHandle*/);
        }

        private void DoWorkAsyncronously(/*EventWaitHandle eventWaitHandle*/)
        {
            Logger.LogEvent(string.Format("Operation '{0} ({1})' work started asyncronously.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
            try
            {
                DoWork();
            }
            finally
            {
                //Logger.LogEvent(string.Format("Operation '{0} ({1})' setting handler after completion.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
                //eventWaitHandle.Set();
            }
        }

        private void DoWork()
        {
            Logger.LogEvent(string.Format("Operation '{0} ({1})' work started.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
            TResult result;
            try
            {
                Logger.LogEvent(string.Format("Operation '{0} ({1})' invoking Run method.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
                result = Run();
            }
            catch (Exception ex)
            {
                Logger.LogEvent(string.Format("Operation '{0} ({1})' failed with exception.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
                _operationsManager.SetOperationFailed(_operationId, ex);
                return;
            }
            if (_isCompletelyCanceled)
            {
                // If operation has been canceled completely, don't report its result:
                Logger.LogEvent(string.Format("Operation '{0} ({1})' work finished with cancelation.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
                return;
            }
            Logger.LogEvent(string.Format("Operation '{0} ({1})' completed successfully.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
            _operationsManager.SetOperationResult(_operationId, result);
        }

        private void RegisterOperationInManager(bool isInvokedSynchronically)
        {
            Logger.LogEvent(string.Format("Enqueueing operation '{0}' {1}.",
                _readableOperationId,
                isInvokedSynchronically ? "sync" : "async"), this, ImportanceLevels.gUnimportant);
            _operationId = _operationsManager.RegisterOperation(_readableOperationId, isInvokedSynchronically);
            Logger.LogEvent(string.Format("Enqueued operation '{0}' with guid '{1}'.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
        }

        protected void ReportProgress(int progressPercent)
        {
            Logger.LogEvent(string.Format("Operation '{0} ({1})' reports {2}% progress.", _readableOperationId, _operationId, progressPercent), this, ImportanceLevels.gUnimportant);
            _operationsManager.SetOperationProgress(_operationId, progressPercent);
        }

        protected bool IsCancelationPending()
        {
            Logger.LogEvent(string.Format("Operation '{0} ({1})' inquires cancelation state.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
            return _operationsManager.GetIsOperationCancelationPending(_operationId);
        }

        protected void ReportCancelationCompleted()
        {
            Logger.LogEvent(string.Format("Operation '{0} ({1})' reports cancelation completion.", _readableOperationId, _operationId), this, ImportanceLevels.gUnimportant);
            _isCompletelyCanceled = true;
            _operationsManager.SetOperationCanceled(_operationId);
        }

        protected abstract TResult Run();

        protected virtual void LogOperationEvent(string eventDescription, ImportanceLevels importnceLevel)
        {
            Logger.LogEvent(string.Format("Operation '{0} ({1})': {2}", _readableOperationId, _operationId, eventDescription), this, importnceLevel);
        }
    }
}
