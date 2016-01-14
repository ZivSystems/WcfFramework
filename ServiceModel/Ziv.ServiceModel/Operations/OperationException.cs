using System;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Operations
{
    public class OperationException : Exception
    {
        public Guid OperationId { get; set; }
        public string OperationDisplayName { get; set; }

        public OperationException(Guid operationId, string operationDisplayName, Exception exception)
            : base(CreateMessage(operationId, operationDisplayName), exception)
        {
            Logger.LogEvent(CreateMessage(operationId, operationDisplayName), this, ImportanceLevels.bVeryHigh);
            OperationId = operationId;
            OperationDisplayName = operationDisplayName;
        }

        private static string CreateMessage(Guid operationId, string operationDisplayName)
        {
            return string.Format(
                "Error has occured during service operation ({0} {1}). See inner exception for details.", operationDisplayName, operationId);
        }
    }
}