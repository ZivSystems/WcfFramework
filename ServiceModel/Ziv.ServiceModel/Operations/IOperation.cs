namespace Ziv.ServiceModel.Operations
{
    public interface IOperation
    {
        OperationEnqueuedToken RunAsync();
        OperationStatus RunSync();
    }

    public interface IOperation<TResult> : IOperation
    {

    }
}