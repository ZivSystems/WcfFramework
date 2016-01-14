using System.Runtime.Serialization;

namespace Ziv.ServiceModel.Operations
{
    [DataContract]
    public class OperationStatus<TResult>
    {
        public OperationStatus(TResult result, OperationStatusInformation info)
        {
            Result = result;
            Info = info;
        }

        [DataMember]
        public OperationStatusInformation Info { get; private set; }

        [DataMember]
        public TResult Result { get; private set; }
    }

    public class OperationStatus
    {
        public OperationStatus(object result, OperationStatusInformation info)
        {
            Result = result;
            Info = info;
        }

        public OperationStatusInformation Info { get; set; }

        public object Result { get; set; }

        public OperationStatus<TResult> ToTypedStatus<TResult>()
        {
            return new OperationStatus<TResult>((TResult)Result, Info);
        }
    }
}