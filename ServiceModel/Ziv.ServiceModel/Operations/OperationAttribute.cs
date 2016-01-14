using System;

namespace Ziv.ServiceModel.Operations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OperationAttribute : Attribute
    {
        public string ServiceShortName { get; set; }

        public OperationAttribute(string serviceShortName)
        {
            ServiceShortName = serviceShortName;
            _serviceContractInterfaceName = "I" + serviceShortName + "Service";
            _serviceImplementationClassName = serviceShortName + "Service";
            Generate = OperationGeneration.Sync;
        }

        private string _serviceImplementationClassName;
        public string ServiceImplementationClassName
        {
            get { return _serviceImplementationClassName; }
            set { _serviceImplementationClassName = value; }
        }

        private string _serviceContractInterfaceName;

        public string ServiceContractInterfaceName
        {
            get { return _serviceContractInterfaceName; }
            set { _serviceContractInterfaceName = value; }
        }

        public OperationGeneration Generate { get; set; }

        public bool IsReportingProgress { get; set; }

        public bool IsSupportingCancel { get; set; }

        public int ExpectedCompletionTimeMilliseconds { get; set; }

        public int SuggestedPollingIntervalMilliseconds { get; set; }
    }

    [Flags]
    public enum OperationGeneration
    {
        None,
        Async = 1,
        Sync = 2,
        Both = Async | Sync,
    }
}
