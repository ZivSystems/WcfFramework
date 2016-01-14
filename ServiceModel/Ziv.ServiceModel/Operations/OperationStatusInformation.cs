using System.Runtime.Serialization;

namespace Ziv.ServiceModel.Operations
{
    [DataContract]
    public class OperationStatusInformation
    {
        public OperationStatusInformation(OperationState state, int progress = 0)
        {
            State = state;
            Progress = progress;
        }
        [DataMember]
        public OperationState State { get; internal set; }
        [DataMember]
        public int Progress { get; internal set; }
    }
}
