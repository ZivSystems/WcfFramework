using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;
using Ziv.ServiceModel.Test.Operations;

namespace Ziv.ServiceModel.Test
{
    [TestClass]
    public class ServiceBase_TestFixture
    {
        private class TestService : ServiceBase
        {
            IOperationsManager _operationsManager;

            public TestService()
                : this(new OperationsManagerMock()) { }

            public TestService(IOperationsManager operationsManager)
                : base(operationsManager) 
            {
                _operationsManager = operationsManager;
            }

            public OperationStatus DoOperation(bool isFailure = false)
            {
                return base.DoOperation(new TestOperation(_operationsManager, isFailure));
            }

            public OperationStartInformation DoOperationAsync(bool isFailure = false)
            {
                return base.DoOperationAsync(new TestOperation(_operationsManager, isFailure));
            }

            public new OperationStatus GetOperationStatus(Guid operationId)
            {
                return base.GetOperationStatus(operationId);
            }

            public new void CancelOperation(Guid operationId)
            {
                base.CancelOperation(operationId);
            }
        }

        [TestMethod]
        public void DoOperation_OnSuccessfulOperation_BlocksExecutionAndReturnsResult_Test() 
        {
            var service = new TestService();
            
            var startTime = DateTime.Now;
            var operationStatus = service.DoOperation();
            var endTime = DateTime.Now;
            
            Assert.IsTrue(endTime - startTime >= TestOperation.OperationDuration);
            Assert.AreEqual(OperationState.CompletedSucessfully, operationStatus.Info.State);
            Assert.AreEqual(TestOperation.TEST_OPERATION_RESULT, operationStatus.Result);
        }

        [TestMethod]
        public void DoOperation_OnFailureOperation_BlocksExecutionAndThrow_Test()
        {
            var service = new TestService();
            
            var startTime = DateTime.Now;
            Exception exception = null;
            try
            {
                service.DoOperation(isFailure: true);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            var endTime = DateTime.Now;
            
            Assert.IsTrue(endTime - startTime >= TestOperation.OperationDuration);
            Assert.IsNotNull(exception);
            Assert.AreEqual(TestOperation.OperationException, exception);
        }

        [TestMethod]
        public void DoOperationAsync_OnSuccessfulOperation_NotBlockExecutionAndReturnsOperationGuidWithAttributeInformationAndOperationsManagerStatusContainsResult_Test()
        {
            var operationsManager = new OperationsManagerMock();
            var service = new TestService(operationsManager);
            
            var startTime = DateTime.Now;
            var operationStartInformation = service.DoOperationAsync();
            var endTime = DateTime.Now;
            
            var beforeWaitOperationStatus = operationsManager.GetOperationStatus(operationStartInformation.OperationId);
            Thread.Sleep(TestOperation.OperationDuration + TestOperation.SafeTimeToSetOperationResult);
            var afterWaitOperationStatus = operationsManager.GetOperationStatus(operationStartInformation.OperationId);
            Assert.IsTrue(endTime - startTime < TestOperation.OperationDuration);
            Assert.AreEqual(TestOperation.IS_REPORTING_PROGRESS, operationStartInformation.IsReportingProgress);
            Assert.AreEqual(TestOperation.IS_SUPPORTING_CANCEL, operationStartInformation.IsSupportingCancel);
            Assert.AreEqual(TestOperation.SUGGESTED_POLLING_INTERVAL_MILLISECONDS, operationStartInformation.SuggestedPollingIntervalMilliseconds);
            Assert.AreEqual(TestOperation.EXPECTED_COMPLETION_TIME_MILLISECONDS, operationStartInformation.ExpectedCompletionTimeMilliseconds);
            Assert.AreEqual(OperationState.Started, beforeWaitOperationStatus.Info.State);
            Assert.IsNull(beforeWaitOperationStatus.Result);
            Assert.AreEqual(OperationState.CompletedSucessfully, afterWaitOperationStatus.Info.State);
            Assert.AreEqual(TestOperation.TEST_OPERATION_RESULT, afterWaitOperationStatus.Result);
        }

        [TestMethod]
        public void DoOperationAsync_OnFailureOperation_NotBlockExecutionAndReturnsOperationGuidWithAttributeInformationAndOperationsManagerStatusThrows_Test()
        {
            var operationsManager = new OperationsManagerMock();
            var service = new TestService(operationsManager);
            
            var startTime = DateTime.Now;
            var operationStartInformation = service.DoOperationAsync(isFailure: true);
            var endTime = DateTime.Now;
            
            var beforeWaitOperationStatus = operationsManager.GetOperationStatus(operationStartInformation.OperationId);
            Thread.Sleep(TestOperation.OperationDuration + TestOperation.SafeTimeToSetOperationResult);
            Exception exception = null;
            try
            {
                operationsManager.GetOperationStatus(operationStartInformation.OperationId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            Assert.IsTrue(endTime - startTime < TestOperation.OperationDuration);
            Assert.AreEqual(TestOperation.IS_REPORTING_PROGRESS, operationStartInformation.IsReportingProgress);
            Assert.AreEqual(TestOperation.IS_SUPPORTING_CANCEL, operationStartInformation.IsSupportingCancel);
            Assert.AreEqual(TestOperation.SUGGESTED_POLLING_INTERVAL_MILLISECONDS, operationStartInformation.SuggestedPollingIntervalMilliseconds);
            Assert.AreEqual(TestOperation.EXPECTED_COMPLETION_TIME_MILLISECONDS, operationStartInformation.ExpectedCompletionTimeMilliseconds);
            Assert.AreEqual(OperationState.Started, beforeWaitOperationStatus.Info.State);
            Assert.IsNull(beforeWaitOperationStatus.Result);
            Assert.AreEqual(TestOperation.OperationException, exception);
        }

        [TestMethod]
        public void DoOperationAsync_OnCancelationRequstedButNotCompletedOperation_NotBlockExecutionAndReturnsOperationGuidWithAttributeInformationAndOperationsManagerStatusIsCompletedSuuccessfuly_Test()
        {
            var operationsManager = new OperationsManagerMock();
            var service = new TestService(operationsManager);
            
            var startTime = DateTime.Now;
            var operationStartInformation = service.DoOperationAsync();
            var endTime = DateTime.Now;
            
            var beforeWaitOperationStatus = operationsManager.GetOperationStatus(operationStartInformation.OperationId);
            service.CancelOperation(operationStartInformation.OperationId);
            Thread.Sleep(TestOperation.OperationDuration + TestOperation.SafeTimeToSetOperationResult);
            var afterWaitOperationStatus = operationsManager.GetOperationStatus(operationStartInformation.OperationId);
            Assert.IsTrue(endTime - startTime < TestOperation.OperationDuration);
            Assert.AreEqual(TestOperation.IS_REPORTING_PROGRESS, operationStartInformation.IsReportingProgress);
            Assert.AreEqual(TestOperation.IS_SUPPORTING_CANCEL, operationStartInformation.IsSupportingCancel);
            Assert.AreEqual(TestOperation.SUGGESTED_POLLING_INTERVAL_MILLISECONDS, operationStartInformation.SuggestedPollingIntervalMilliseconds);
            Assert.AreEqual(TestOperation.EXPECTED_COMPLETION_TIME_MILLISECONDS, operationStartInformation.ExpectedCompletionTimeMilliseconds);
            Assert.AreEqual(OperationState.Started, beforeWaitOperationStatus.Info.State);
            Assert.IsNull(beforeWaitOperationStatus.Result);
            Assert.AreEqual(OperationState.CompletedSucessfully, afterWaitOperationStatus.Info.State);
            Assert.AreEqual(TestOperation.TEST_OPERATION_RESULT, afterWaitOperationStatus.Result);
        }

        [TestMethod]
        public void GetOperationStatus_ReturnsTheSameStatusAsTheOperationsManagerStatus_Test() 
        {
            var operationsManager = new OperationsManagerMock();
            var service = new TestService(operationsManager);            
            var operationStartInformation = service.DoOperationAsync();
            Thread.Sleep(TestOperation.OperationDuration + TestOperation.SafeTimeToSetOperationResult);
            var operationStatusFromOperationsManager = operationsManager.GetOperationStatus(operationStartInformation.OperationId);
            
            var operationStatusFromService = service.GetOperationStatus(operationStartInformation.OperationId);
            
            Assert.AreEqual(operationStatusFromOperationsManager.Result, operationStatusFromService.Result);
            Assert.AreEqual(operationStatusFromOperationsManager.Info.State, operationStatusFromService.Info.State);
            Assert.AreEqual(operationStatusFromOperationsManager.Info.Progress, operationStatusFromService.Info.Progress);
        }

        [TestMethod]
        public void GetOperationStatus_OnFailureOperation_ThrowsTheSameExceptionAsTheOperationsManagerThrows_Test() 
        {
            var operationsManager = new OperationsManagerMock();
            var service = new TestService(operationsManager);
            var operationStartInformation = service.DoOperationAsync(isFailure: true);
            Thread.Sleep(TestOperation.OperationDuration + TestOperation.SafeTimeToSetOperationResult);
            Exception exceptionFromOperationsManager = null;
            try
            {
                var operationStatusFromOperationsManager = operationsManager.GetOperationStatus(operationStartInformation.OperationId);
            }
            catch (Exception ex)
            {
                exceptionFromOperationsManager = ex;
            }

            Exception exceptionFromService = null;
            try
            {
                var operationStatusFromService = service.GetOperationStatus(operationStartInformation.OperationId);
            }
            catch (Exception ex)
            {
                exceptionFromService = ex;
            }

            Assert.IsNotNull(exceptionFromService);
            Assert.AreEqual(exceptionFromOperationsManager, exceptionFromService);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetOperationStatus_OnFakeGuid_ThrowsOperationNotFound_Test() 
        {
            var service = new TestService();
            var fakeGuid = Guid.NewGuid();

            service.GetOperationStatus(fakeGuid);
        }

        [TestMethod]
        public void CancelOperation_OperationStatusIsCancelationPendingInOperationsManager_Test() 
        {
            var operationsManager = new OperationsManagerMock();
            var service = new TestService(operationsManager);
            var operationStartInformation = service.DoOperationAsync();
            
            service.CancelOperation(operationStartInformation.OperationId);
            
            var operationStatus = operationsManager.GetOperationStatus(operationStartInformation.OperationId);
            Assert.AreEqual(OperationState.CancelationPending, operationStatus.Info.State);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CancelOperation_OnFakeGuid_ThrowOperationNotFound_Test()
        {
            var service = new TestService();
            var fakeGuid = Guid.NewGuid();

            service.CancelOperation(fakeGuid);
        }
    }
}
