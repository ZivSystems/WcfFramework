using System;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Ziv.ServiceModel.Test.Operations
{
    [TestClass]
    public abstract class IOperationsManager_TestFixture
    {
        protected const string OPERATION_DISPLAY_NAME = "Test Display Name";
        protected const string EXCEPTION_MESSAGE = "Test Exception Message";
        protected static readonly object SuccessfulResult = new Object();
        protected static readonly Exception GeneralFailureException = new Exception(EXCEPTION_MESSAGE);
        protected static readonly FaultException<int> WcfFaultException = new FaultException<int>(42, EXCEPTION_MESSAGE);

        protected abstract IOperationsManager OperationsManager { get; }

        [TestMethod]
        public void RegisterSyncOperation_ReturnsNotEmptyGuid_Test()
        {
            var operationId = OperationsManager.RegisterOperation(OPERATION_DISPLAY_NAME, true);

            Assert.AreNotEqual(new Guid(), operationId);
        }

        [TestMethod]
        public void RegisterAsyncOperation_ReturnsNotEmptyGuid_Test()
        {
            var operationId = OperationsManager.RegisterOperation(OPERATION_DISPLAY_NAME, false);

            Assert.AreNotEqual(new Guid(), operationId);
        }

        [TestMethod]
        public void GetOperationStatus_OnRunningOperation_ReturnsStarted_Test()
        {
            var operationId = RegisterNewOperation();

            var operationStatus = OperationsManager.GetOperationStatus(operationId);

            Assert.AreEqual(OperationState.Started, operationStatus.Info.State);
            Assert.IsNull(operationStatus.Result);
        }

        [TestMethod]
        public void GetOperationStatus_OnSuccessfulOperation_ReturnsSuccessfulResult_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationResult(operationId, SuccessfulResult);

            var operationStatus = OperationsManager.GetOperationStatus(operationId);

            Assert.AreEqual(OperationState.CompletedSucessfully, operationStatus.Info.State);
            Assert.AreEqual(SuccessfulResult, operationStatus.Result);
        }

        [TestMethod]
        public void GetOperationStatus_OnOperationFailed_ThrowsWithSpecifiedExceptionDetails_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationFailed(operationId, GeneralFailureException);

            OperationException exception = null;
            try
            {
                var operationStatus = OperationsManager.GetOperationStatus(operationId);
            }
            catch (OperationException ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Assert.AreEqual(operationId, exception.OperationId);
            Assert.AreEqual(EXCEPTION_MESSAGE, exception.InnerException.Message);
        }

        [TestMethod]
        public void GetOperationStatus_OnOperationDecoratedWithOperationFaultFailed_ThrowsStronglyTypedFault_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationFailed(operationId, WcfFaultException);

            FaultException exception = null;
            try
            {
                OperationsManager.GetOperationStatus(operationId);
            }
            catch (FaultException ex)
            {
                exception = ex;
            }

            Assert.AreEqual(WcfFaultException, exception);
        }

        [TestMethod]
        public void GetOperationStatus_OnProgressSet_ReturnsProgress_Test()
        {
            var operationId = RegisterNewOperation();
            var progress = new Random().Next(0, 100);
            OperationsManager.SetOperationProgress(operationId, progress);

            var operationStatus = OperationsManager.GetOperationStatus(operationId);

            Assert.AreEqual(progress, operationStatus.Info.Progress);
        }

        [TestMethod]
        public void GetOperationStatus_OnOperationCancelFlagSet_ReturnsCancelationPending_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCancelFlag(operationId);

            var operationStatus = OperationsManager.GetOperationStatus(operationId);

            Assert.AreEqual(OperationState.CancelationPending, operationStatus.Info.State);
        }

        [TestMethod]
        public void GetOperationStatus_OnOperationCancelFlagSetAfterSuccessfulCompletion_ReturnsSuccessfulResult_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationResult(operationId, SuccessfulResult);
            OperationsManager.SetOperationCancelFlag(operationId);

            var operationStatus = OperationsManager.GetOperationStatus(operationId);

            Assert.AreEqual(OperationState.CompletedSucessfully, operationStatus.Info.State);
            Assert.AreEqual(SuccessfulResult, operationStatus.Result);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationException))]
        public void GetOperationStatus_OnOperationCancelFlagSetAfterOperationFailed_Throws_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationFailed(operationId, GeneralFailureException);
            OperationsManager.SetOperationCancelFlag(operationId);

            var operationStaus = OperationsManager.GetOperationStatus(operationId);
        }

        [TestMethod]
        public void GetOperationStatus_OnOperationCancelFlagSetAfterCancelation_ReturnsCanceled_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCanceled(operationId);
            OperationsManager.SetOperationCancelFlag(operationId);

            var operationStaus = OperationsManager.GetOperationStatus(operationId);

            Assert.AreEqual(OperationState.Canceled, operationStaus.Info.State);
        }

        [TestMethod]
        public void GetOperationStatus_OnOperationCanceled_ReturnsCanceled_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCanceled(operationId);

            var operationStatus = OperationsManager.GetOperationStatus(operationId);

            Assert.AreEqual(OperationState.Canceled, operationStatus.Info.State);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetOperationStatus_OnFakeGuid_ThrowsOperationNotFound_Test()
        {
            var fakeGuid = Guid.NewGuid();

            OperationsManager.GetOperationStatus(fakeGuid);
        }

        [TestMethod]
        public void SetOperationCancelFlag_OperationStatusIsCancelationPending_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCancelFlag(operationId);

            var operationStatus = OperationsManager.GetOperationStatus(operationId);

            Assert.AreEqual(OperationState.CancelationPending, operationStatus.Info.State);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetOperationCancelFlag_OnFakeGuid_ThrowsOperationNotFound_Test()
        {
            var fakeGuid = Guid.NewGuid();

            OperationsManager.SetOperationCancelFlag(fakeGuid);
        }

        [TestMethod]
        public void GetIsOperationCancelationPending_OnRunningOperationCancelationNotRequested_ReturnsFalse_Test()
        {
            var operationId = RegisterNewOperation();
            
            var isOperationFlaggedToCancel = OperationsManager.GetIsOperationCancelationPending(operationId);
            
            Assert.AreEqual(false, isOperationFlaggedToCancel);
        }

        [TestMethod]
        public void GetIsOperationCancelationPending_OnRunningOperationCancelationRequested_ReturnsTrue_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCancelFlag(operationId);
            
            var isOperationFlaggedToCancel = OperationsManager.GetIsOperationCancelationPending(operationId);
            
            Assert.AreEqual(true, isOperationFlaggedToCancel);
        }

        [TestMethod]
        public void GetIsOperationCancelationPending_OnCanceledOperationCancelationRequested_ReturnsFalse_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCancelFlag(operationId);
            OperationsManager.SetOperationCanceled(operationId);
            
            var isOperationFlaggedToCancel = OperationsManager.GetIsOperationCancelationPending(operationId);
            
            Assert.AreEqual(false, isOperationFlaggedToCancel);
        }

        [TestMethod]
        public void GetIsOperationCancelationPending_OnCanceledOperationCancelationNotRequested_ReturnsFalse_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCanceled(operationId);
            
            var isOperationFlaggedToCancel = OperationsManager.GetIsOperationCancelationPending(operationId);
            
            Assert.AreEqual(false, isOperationFlaggedToCancel);
        }

        [TestMethod]
        public void GetIsOperationCancelationPending_OnCompletedSuccessfulOperationCancelationNotRequested_ReturnsFalse_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationResult(operationId, SuccessfulResult);
            
            var isOperationFlaggedToCancel = OperationsManager.GetIsOperationCancelationPending(operationId);
            
            Assert.AreEqual(false, isOperationFlaggedToCancel);
        }

        [TestMethod]
        public void GetIsOperationCancelationPending_OnCompletedSuccessfulOperationCancelationRequested_ReturnsFalse_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCancelFlag(operationId);
            OperationsManager.SetOperationResult(operationId, SuccessfulResult);
            
            var isOperationFlaggedToCancel = OperationsManager.GetIsOperationCancelationPending(operationId);
            
            Assert.AreEqual(false, isOperationFlaggedToCancel);
        }

        [TestMethod]
        public void GetIsOperationCancelationPending_OnFailedOperationCancelationNotRequested_ReturnsFalse_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationFailed(operationId, GeneralFailureException);
            
            var isOperationFlaggedToCancel = OperationsManager.GetIsOperationCancelationPending(operationId);
            
            Assert.AreEqual(false, isOperationFlaggedToCancel);
        }

        [TestMethod]
        public void GetIsOperationCancelationPending_OnFailedOperationCancelationRequested_ReturnsFalse_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCancelFlag(operationId);
            OperationsManager.SetOperationFailed(operationId, GeneralFailureException);
            
            var isOperationFlaggedToCancel = OperationsManager.GetIsOperationCancelationPending(operationId);
            
            Assert.AreEqual(false, isOperationFlaggedToCancel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetIsOperationCancelationPending_OnFakeGuid_ThrowsOperationNotFound_Test()
        {
            var fakeGuid = Guid.NewGuid();

            var isOperationFlaggedTocancel = OperationsManager.GetIsOperationCancelationPending(fakeGuid);
        }

        [TestMethod]
        public void SetOperationCanceled_OperationStatusIsCanceled_Test()
        {
            var operationId = RegisterNewOperation();
            
            OperationsManager.SetOperationCanceled(operationId);
            
            var operationStatus = OperationsManager.GetOperationStatus(operationId);
            Assert.AreEqual(OperationState.Canceled, operationStatus.Info.State);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetOperationCanceled_OnFakeGuid_ThrowsOperationNotFound_Test()
        {
            var fakeGuid = Guid.NewGuid();
            
            OperationsManager.SetOperationCanceled(fakeGuid);
        }

        [TestMethod]
        public void SetOperationProgress_OperationStatusContainsNewProgress_Test()
        {
            var operationId = RegisterNewOperation();
            var progress = new Random().Next(0, 100);
            
            OperationsManager.SetOperationProgress(operationId, progress);
            
            var operationStatus = OperationsManager.GetOperationStatus(operationId);
            Assert.AreEqual(progress, operationStatus.Info.Progress);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetOperationProgress_OnFakeGuid_ThrowsOperationNotFound_Test()
        {
            var fakeGuid = Guid.NewGuid();
            var progress = new Random().Next(0, 100);
            
            OperationsManager.SetOperationProgress(fakeGuid, progress);
        }

        [TestMethod]
        public void SetOperationResult_OperationStatusIsCompletedSuccessfullyAndContainsResult_Test()
        {
            var operationId = RegisterNewOperation();
            
            OperationsManager.SetOperationResult(operationId, SuccessfulResult);
            
            var operationStatus = OperationsManager.GetOperationStatus(operationId);
            Assert.AreEqual(OperationState.CompletedSucessfully, operationStatus.Info.State);
            Assert.AreEqual(SuccessfulResult, operationStatus.Result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetOperationResult_OnFakeGuid_ThrowsOperationNotFound_Test()
        {
            var fakeGuid = Guid.NewGuid();
            
            OperationsManager.SetOperationResult(fakeGuid, SuccessfulResult);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationException))]
        public void SetOperationFailed_OnSetNotFaultException_GettingOperationStatusThrowsOperationException_Test()
        {
            var operationId = RegisterNewOperation();
            
            OperationsManager.SetOperationFailed(operationId, GeneralFailureException);
            
            var operationStatus = OperationsManager.GetOperationStatus(operationId);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException), AllowDerivedTypes = true)]
        public void SetOperationFailed_OnSetFaultException_GettingOperationStatusThrowsTheSameFaultException_Test()
        {
            var operationId = RegisterNewOperation();
            
            OperationsManager.SetOperationFailed(operationId, WcfFaultException);
            
            var operationStatus = OperationsManager.GetOperationStatus(operationId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetOperationFailed_OnFakeGuid_ThrowsOperationNotFound_Test()
        {
            var fakeGuid = Guid.NewGuid();
            
            OperationsManager.SetOperationFailed(fakeGuid, GeneralFailureException);
        }

        protected Guid RegisterNewOperation()
        {
            return OperationsManager.RegisterOperation(OPERATION_DISPLAY_NAME, true);
        }
    }
}
