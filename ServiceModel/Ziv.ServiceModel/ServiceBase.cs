using System;
using System.Linq;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel
{
    public abstract class ServiceBase
    {
        private readonly IOperationsManager _baseOperationsManager;

        public ServiceBase(IOperationsManager operationsManager)
        {
            _baseOperationsManager = operationsManager;
            Logger.LogEvent(string.Format("New instance of {0} has been constructed.", this.GetType().Name), this, ImportanceLevels.gUnimportant);
        }

        protected virtual OperationStatus DoOperation(IOperation operation)
        {
            Logger.LogEvent(string.Format("Doing operation of type {0} syncronously.", operation.GetType().Name), this, ImportanceLevels.gUnimportant);
            var status = operation.RunSync();
            Logger.LogEvent(string.Format("Operation of type {0} is done.", operation.GetType().Name), this, ImportanceLevels.gUnimportant);
            return status;
        }

        protected OperationStartInformation DoOperationAsync(IOperation operation)
        {
            Logger.LogEvent(string.Format("Doing operation of type {0} asyncronously.", operation.GetType().Name), this, ImportanceLevels.gUnimportant);
            var operationAttribute = (OperationAttribute)operation.GetType().GetCustomAttributes(typeof(OperationAttribute), true).SingleOrDefault();
            var operationStart = operation.RunAsync();
            var operationStartInformation = new OperationStartInformation { OperationId = operationStart.OperationId };
            if (operationAttribute != null)
            {
                operationStartInformation.IsReportingProgress = operationAttribute.IsReportingProgress;
                operationStartInformation.IsSupportingCancel = operationAttribute.IsSupportingCancel;
                operationStartInformation.ExpectedCompletionTimeMilliseconds = operationAttribute.ExpectedCompletionTimeMilliseconds;
                operationStartInformation.SuggestedPollingIntervalMilliseconds = operationAttribute.SuggestedPollingIntervalMilliseconds;
            }
            Logger.LogEvent(
                string.Format(
                    "Operation of type {0} has started. OperationStartInformation: {1}.",
                    operation.GetType().Name,
                    new { 
                        operationStartInformation.OperationId, 
                        operationStartInformation.IsReportingProgress, 
                        operationStartInformation.IsSupportingCancel, 
                        operationStartInformation.ExpectedCompletionTimeMilliseconds, 
                        operationStartInformation.SuggestedPollingIntervalMilliseconds 
                    }),
                this,
                ImportanceLevels.gUnimportant);
            return operationStartInformation;
        }

        protected virtual OperationStatus GetOperationStatus(Guid operationId)
        {
            Logger.LogEvent(string.Format("Operation status is requested for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
            var operationStatus = _baseOperationsManager.GetOperationStatus(operationId);
            Logger.LogEvent(
                string.Format(
                    "Operation status has been retrieved for operation '{0}'. The retrieved operation state is {1}.",
                    operationId, 
                    operationStatus.Info.State),
                this,
                ImportanceLevels.gUnimportant);
            return operationStatus;
        }

        protected void CancelOperation(Guid operationId)
        {
            Logger.LogEvent(string.Format("Operation cancelation is requested for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
            _baseOperationsManager.SetOperationCancelFlag(operationId);
            Logger.LogEvent(string.Format("Operation cancel flag has been set for operation '{0}'.", operationId), this, ImportanceLevels.gUnimportant);
        }

    }
}