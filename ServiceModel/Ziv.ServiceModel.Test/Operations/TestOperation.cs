using System;
using System.Threading;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Ziv.ServiceModel.Test.Operations
{
    [Operation("Test",
        IsReportingProgress= IS_REPORTING_PROGRESS,
        IsSupportingCancel = IS_SUPPORTING_CANCEL,
        SuggestedPollingIntervalMilliseconds = SUGGESTED_POLLING_INTERVAL_MILLISECONDS,
        ExpectedCompletionTimeMilliseconds = EXPECTED_COMPLETION_TIME_MILLISECONDS)]
    class TestOperation : OperationBase<string>
    {
        public const string TEST_OPERATION_RESULT = "Test operation result";
        public static readonly TimeSpan OperationDuration = new TimeSpan(0, 0, 1);
        public static readonly TimeSpan SafeTimeToSetOperationResult = TimeSpan.FromMilliseconds(100);
        public static readonly Exception OperationException = new Exception("Test exception");
        public const int OPERATION_PROGRESS = 42;
        public const bool IS_REPORTING_PROGRESS = true;
        public const bool IS_SUPPORTING_CANCEL = true;
        public const int SUGGESTED_POLLING_INTERVAL_MILLISECONDS = 442;
        public const int EXPECTED_COMPLETION_TIME_MILLISECONDS = 4242;

        private bool _isFailure;

        public TestOperation(bool isFailure = false)
            : this(new OperationsManagerMock(), isFailure) { }

        public TestOperation(IOperationsManager operationsManager, bool isFailure = false)
            : base(operationsManager, "Test operation")
        {
            _isFailure = isFailure;
        }

        protected override string Run()
        {
            Thread.Sleep(OperationDuration);
            if (_isFailure)
            {
                throw OperationException;
            }
            return TEST_OPERATION_RESULT;
        }

        public new void ReportProgress(int progressPercent)
        {
            base.ReportProgress(progressPercent);
        }

        public new bool IsCancelationPending()
        {
            return base.IsCancelationPending();
        }

        public new void ReportCancelationCompleted()
        {
            base.ReportCancelationCompleted();
        }
    }
}
