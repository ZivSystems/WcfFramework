using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ziv.ServiceModel.Operations;

namespace Ziv.ServiceModel.Test.Operations
{
    [TestClass]
    public class OperationBase_TestFixture
    {
        [TestMethod]
        public void RunSync_OnSuccessfulOperation_BlocksExecutionAndReturnsResult_Test() 
        {
            var operation = new TestOperation();
            
            var startTime = DateTime.Now;
            var operationStatus = operation.RunSync();
            var endTime = DateTime.Now;
            
            Assert.IsTrue(endTime - startTime >= TestOperation.OperationDuration);
            Assert.AreEqual(OperationState.CompletedSucessfully, operationStatus.Info.State);
            Assert.AreEqual(TestOperation.TEST_OPERATION_RESULT, operationStatus.Result);
        }

        [TestMethod]
        public void RunSync_OnFailureOperation_BlocksExecutionAndThrow_Test()
        {
            var operation = new TestOperation(isFailure: true);
            
            var startTime = DateTime.Now;
            Exception exception = null;
            try
            {
                operation.RunSync();
            }
            catch(Exception ex) 
            {
                exception = ex;
            }
            var endTime = DateTime.Now;
            
            Assert.IsTrue(endTime - startTime >= TestOperation.OperationDuration);
            Assert.IsNotNull(exception);
            Assert.AreEqual(TestOperation.OperationException, exception);
        }

        [TestMethod]
        public void RunAsync_OnSuccessfulOperation_NotBlockExecutionAndReturnsOperationGuidAndOperationsManagerStatusContainsResult_Test() 
        {
            var operationsManager = new OperationsManagerMock();
            var operation = new TestOperation(operationsManager);
            
            var startTime = DateTime.Now;
            var enqueuedToken = operation.RunAsync();
            var endTime = DateTime.Now;
            
            var beforeWaitOperationStatus = operationsManager.GetOperationStatus(enqueuedToken.OperationId);
            Thread.Sleep(TestOperation.OperationDuration + TestOperation.SafeTimeToSetOperationResult);
            var afterWaitOperationStatus = operationsManager.GetOperationStatus(enqueuedToken.OperationId);
            Assert.IsTrue(endTime - startTime < TestOperation.OperationDuration);
            Assert.AreEqual(OperationState.Started, beforeWaitOperationStatus.Info.State);
            Assert.IsNull(beforeWaitOperationStatus.Result);
            Assert.AreEqual(OperationState.CompletedSucessfully, afterWaitOperationStatus.Info.State);
            Assert.AreEqual(TestOperation.TEST_OPERATION_RESULT, afterWaitOperationStatus.Result);
        }
        
        [TestMethod]
        public void RunAsync_OnFailureOperation_NotBlockExecutionAndReturnsOperationGuidAndOperationsManagerStatusThrows_Test() 
        {
            var operationsManager = new OperationsManagerMock();
            var operation = new TestOperation(operationsManager, isFailure: true);
            
            var startTime = DateTime.Now;
            var enqueuedToken = operation.RunAsync();
            var endTime = DateTime.Now;
            
            var beforeWaitOperationStatus = operationsManager.GetOperationStatus(enqueuedToken.OperationId);
            Thread.Sleep(TestOperation.OperationDuration + TestOperation.SafeTimeToSetOperationResult);
            Exception exception = null;
            try
            {
                operationsManager.GetOperationStatus(enqueuedToken.OperationId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.IsTrue(endTime - startTime < TestOperation.OperationDuration);
            Assert.AreEqual(OperationState.Started, beforeWaitOperationStatus.Info.State);
            Assert.IsNull(beforeWaitOperationStatus.Result);
            Assert.AreEqual(TestOperation.OperationException, exception);
        }

        [TestMethod]
        public void RunAsync_OnCanceledOperation_NotBlockExecutionAndReturnsOperationGuidAndOperationsManagerResultIsEmpty_Test() 
        {
            var operationsManager = new OperationsManagerMock();
            var operation = new TestOperation(operationsManager);
            
            var startTime = DateTime.Now;
            var enqueuedToken = operation.RunAsync();
            var endTime = DateTime.Now;
            
            var beforeWaitOperationStatus = operationsManager.GetOperationStatus(enqueuedToken.OperationId);
            operation.ReportCancelationCompleted();
            Thread.Sleep(TestOperation.OperationDuration + TestOperation.SafeTimeToSetOperationResult);
            var afterWaitOperationStatus = operationsManager.GetOperationStatus(enqueuedToken.OperationId);
            Assert.IsTrue(endTime - startTime < TestOperation.OperationDuration);
            Assert.AreEqual(OperationState.Started, beforeWaitOperationStatus.Info.State);
            Assert.IsNull(beforeWaitOperationStatus.Result);
            Assert.AreEqual(OperationState.Canceled, afterWaitOperationStatus.Info.State);
            Assert.IsNull(afterWaitOperationStatus.Result);
        }

        [TestMethod]
        public void ReportProgress_OperationsManagerStatusContainsReportedProgress_Test() 
        {
            var operationsManager = new OperationsManagerMock();
            var operation = new TestOperation(operationsManager);            
            var enqueuedToken = operation.RunAsync();
            
            operation.ReportProgress(TestOperation.OPERATION_PROGRESS);
            
            var operationStatus = operationsManager.GetOperationStatus(enqueuedToken.OperationId);
            Assert.AreEqual(TestOperation.OPERATION_PROGRESS, operationStatus.Info.Progress);
        }
        
        [TestMethod]
        public void IsCancelationPending_OnNoOperationCancelationPendingInOperationManager_ReturnsFalse_Test()
        {
            var operation = new TestOperation();
            operation.RunAsync();
            
            var isCancelationPending = operation.IsCancelationPending();
            
            Assert.AreEqual(false, isCancelationPending);
        }

        [TestMethod]
        public void IsCancelationPending_OnOperationCancelationPendingInOperationManager_ReturnsTrue_Test() 
        {
            var operationsManager = new OperationsManagerMock();
            var operation = new TestOperation(operationsManager);
            var enqueuedToken = operation.RunAsync();
            operationsManager.SetOperationCancelFlag(enqueuedToken.OperationId);
            
            var isCancelationPending = operation.IsCancelationPending();
            
            Assert.AreEqual(true, isCancelationPending);
        }

        [TestMethod]
        public void ReportCancelationCompleted_OperationsManagerStatusIsCanceled_Test()
        {
            var operationsManager = new OperationsManagerMock();
            var operation = new TestOperation(operationsManager);
            var enqueuedToken = operation.RunAsync();
            
            operation.ReportCancelationCompleted();
            
            var operationStatus = operationsManager.GetOperationStatus(enqueuedToken.OperationId);
            Assert.AreEqual(OperationState.Canceled, operationStatus.Info.State);
        }
    }
}
