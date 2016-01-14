using System;
using Sample.DTO;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Sample.Operations
{
    [Operation("AnotherTrial", Generate = OperationGeneration.Both)]
    public class DoOtherStuff : OperationBase<SomeResult>
    {
        public DoOtherStuff(IOperationsManager operationsManager)
            : base(operationsManager, "Other stuff")
        {
        }

        protected override SomeResult Run()
        {
            throw new NotImplementedException();
        }
    }
}
