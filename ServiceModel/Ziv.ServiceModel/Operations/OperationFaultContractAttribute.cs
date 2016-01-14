using System;

namespace Ziv.ServiceModel.Operations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class OperationFaultContractAttribute : Attribute
    {
        public Type DetailType { get; set; }

        public OperationFaultContractAttribute(Type detailType)
        {
            DetailType = detailType;
        }
    }
}
