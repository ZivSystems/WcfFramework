using System;
using System.Threading;
using Sample.DTO;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Sample.Operations
{
    [Operation("Trials")]
    public class DoSomethingOperation : OperationBase<SomeResult>
    {
        public const string DO_SOMETHING_RESULT_TEMPLATE = "The number provided was '{0}'.";
        const int SLEEP_TIME_MILISECONDS = 1000;
        private const int PROCESS_STAGES = 10;

        private readonly SomeParameters _parameters;
        private readonly ISomeRequiredService _someRequiredService;

        public DoSomethingOperation(
            SomeParameters parameters, 
            IOperationsManager operationsManager, 
            ISomeRequiredService someRequiredService)
            : base(operationsManager, "Do Something")
        {
            _parameters = parameters;
            _someRequiredService = someRequiredService;
        }

        protected override SomeResult Run()
        {
            for (int i = 0; i < PROCESS_STAGES; i++)
            {
                Thread.Sleep(SLEEP_TIME_MILISECONDS);
                if (IsCancelationPending())
                {
                    ReportCancelationCompleted();
                    return null;
                }
                int progress = Convert.ToInt32((((double)i) / ((double)PROCESS_STAGES)) * 100);
                ReportProgress(progress);
            }
            return new SomeResult
                       {
                           Result = string.Format(DO_SOMETHING_RESULT_TEMPLATE, _parameters.Parameter)
                       };
        }
    }
}