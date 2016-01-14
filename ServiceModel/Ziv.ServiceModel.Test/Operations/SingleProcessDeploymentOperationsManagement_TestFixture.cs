using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Ziv.ServiceModel.Test.Operations
{
    [TestClass]
    public class SingleProcessDeploymentOperationsManagement_TestFixture : IOperationsManager_TestFixture
    {
        private static readonly TimeSpan OperationCollectionIntervalPlusExecutionSafeTimeSpan = SingleProcessDeploymentOperationsManager.OperationsCollectionTimerIntervalMilliseconds + TimeSpan.FromMilliseconds(20);
        private static readonly TimeSpan ShortTimeSpanBeforeTimeOut = TimeSpan.FromMilliseconds(5);

        private SingleProcessDeploymentOperationsManager _operationsManager = new SingleProcessDeploymentOperationsManager();
        protected override IOperationsManager OperationsManager
        {
            get { return _operationsManager; }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetOperationStatus_OnOperationRunTimeout_ThrowsOperationNotFound_Test()
        {
            var operationId = RegisterNewOperation();
            Thread.Sleep(SingleProcessDeploymentOperationsManager.DefaultOperationRunTimeout + OperationCollectionIntervalPlusExecutionSafeTimeSpan);
            
            OperationsManager.GetOperationStatus(operationId);
        }

        [TestMethod]
        public void GetOperationStatus_OnOperationRunAlmostTimedout_ReturnsStatus_Test()
        {
            var operationId = RegisterNewOperation();
            Thread.Sleep(SingleProcessDeploymentOperationsManager.DefaultOperationRunTimeout - ShortTimeSpanBeforeTimeOut);
            
            var operationStatus = OperationsManager.GetOperationStatus(operationId);
            
            Assert.AreEqual(OperationState.Started, operationStatus.Info.State);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetOperationStatus_OnFinalStatusRetrievalPendingTimeout_ThrowsOperationNotFound_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationResult(operationId, SuccessfulResult);
            Thread.Sleep(SingleProcessDeploymentOperationsManager.DefaultFinalStatusRetrievalPendingTimeout + OperationCollectionIntervalPlusExecutionSafeTimeSpan);
            
            OperationsManager.GetOperationStatus(operationId);
        }

        [TestMethod]
        public void GetOperationStatus_OnFinalStatusRetrievalPendingAlmostTimedout_ReturnsStatus_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationResult(operationId, SuccessfulResult);
            Thread.Sleep(SingleProcessDeploymentOperationsManager.DefaultFinalStatusRetrievalPendingTimeout -ShortTimeSpanBeforeTimeOut);
            
            var operationStatus = OperationsManager.GetOperationStatus(operationId);
            
            Assert.AreEqual(OperationState.CompletedSucessfully, operationStatus.Info.State);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetOperationStatus_OnStorageAfterFinalStatusRetrievedTimeout_ThrowsOperationNotFound_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationResult(operationId, SuccessfulResult);
            OperationsManager.GetOperationStatus(operationId);
            Thread.Sleep(SingleProcessDeploymentOperationsManager.DefaultStorageAfterFinalStatusRetrievedByClientTimeout + OperationCollectionIntervalPlusExecutionSafeTimeSpan);
            
            OperationsManager.GetOperationStatus(operationId);
        }

        [TestMethod]
        public void GetOperationStatus_OnStorageAfterFinalStatusRetrievedAlmostTimedout_ReturnsStatus_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationResult(operationId, SuccessfulResult);
            OperationsManager.GetOperationStatus(operationId);
            Thread.Sleep(SingleProcessDeploymentOperationsManager.DefaultStorageAfterFinalStatusRetrievedByClientTimeout - ShortTimeSpanBeforeTimeOut);
            
            var operationStatus = OperationsManager.GetOperationStatus(operationId);
            
            Assert.AreEqual(OperationState.CompletedSucessfully, operationStatus.Info.State);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetOperationStatus_OnCancelationAndStorageAfterFinalStatusRetrievedTimeout_ThrowsOperationNotFound_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCancelFlag(operationId);
            OperationsManager.SetOperationCanceled(operationId);
            Thread.Sleep(SingleProcessDeploymentOperationsManager.DefaultStorageAfterFinalStatusRetrievedByClientTimeout + OperationCollectionIntervalPlusExecutionSafeTimeSpan);
            
            OperationsManager.GetOperationStatus(operationId);
        }

        [TestMethod]
        public void GetOperationStatus_OnCancelationAndStorageAfterFinalStatusRetrievedAlmostTimedout_ReturnsStatus_Test()
        {
            var operationId = RegisterNewOperation();
            OperationsManager.SetOperationCancelFlag(operationId);
            OperationsManager.SetOperationCanceled(operationId);
            Thread.Sleep(SingleProcessDeploymentOperationsManager.DefaultStorageAfterFinalStatusRetrievedByClientTimeout -ShortTimeSpanBeforeTimeOut);
            
            var operationStatus = OperationsManager.GetOperationStatus(operationId);
            
            Assert.AreEqual(OperationState.Canceled, operationStatus.Info.State);
        }
    }
}
