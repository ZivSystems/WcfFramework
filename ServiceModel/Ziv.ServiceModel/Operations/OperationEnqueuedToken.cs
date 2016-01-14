using System;

namespace Ziv.ServiceModel.Operations
{
    public class OperationEnqueuedToken
    {
        public OperationEnqueuedToken(Guid guid/*, AutoResetEvent handler*/)
        {
            OperationId = guid;
            //Handler = handler;
        }

        public Guid OperationId { get; private set; }
        //public AutoResetEvent Handler { get; private set; }
    }
}