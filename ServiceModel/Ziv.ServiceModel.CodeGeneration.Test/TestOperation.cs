using System;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Ziv.ServiceModel.CodeGeneration.Test
{
    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME)]
    class TestOperation : OperationBase<Result>
    {
        public TestOperation(Parameters parameters, IOperationsManager operationsManager)
            : base(operationsManager, "displayName")
        {
        }

        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME)]
    class TestWithMultipleParametersOperation : OperationBase<Result>
    {
        public TestWithMultipleParametersOperation(Parameters parameters, Parameters parameters2, IOperationsManager operationsManager)
            : base(operationsManager, "displayName")
        {
        }

        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME)]
    class TestWithNoParametersOperation : OperationBase<Result>
    {
        public TestWithNoParametersOperation(IOperationsManager operationsManager)
            : base(operationsManager, "displayName")
        {
        }

        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    //class TestWithNoReturnValueOperation : OperationBase<void>
    //{
    //    public TestWithNoReturnValueOperation(IOperationsManager operationsManager, string displayName) : base(operationsManager, displayName)
    //    {
    //    }
    //}

    class TestWithNoAttributeOperation : OperationBase<Result>
    {
        public TestWithNoAttributeOperation(IOperationsManager operationsManager, string displayName)
            : base(operationsManager, displayName)
        {
        }

        protected override Result Run()
        {
            throw new NotImplementedException();
        }

    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME)]
    class TestBoolReturnValueOperation : OperationBase<bool>
    {
        public TestBoolReturnValueOperation(IOperationsManager operationsManager, string displayName) : base(operationsManager, displayName)
        {
        }

        protected override bool Run()
        {
            throw new NotImplementedException();
        }
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME,Generate = OperationGeneration.Async)]
    class TestOperationGenreateAysnc : OperationBase<Result>
    {
        public TestOperationGenreateAysnc(IOperationsManager operationsManager, string displayName) : base(operationsManager, displayName)
        {
        }

        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME, Generate = OperationGeneration.Sync)]
    class TestOperationGenreateSync : OperationBase<Result>
    {
        public TestOperationGenreateSync(IOperationsManager operationsManager, string displayName) : base(operationsManager, displayName)
        {
        }

        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME, Generate = OperationGeneration.Sync)]
    [OperationFaultContract(typeof(FaultDetail))]
    class TestOperationWithFaultAttribute : OperationBase<Result>
    {
        public TestOperationWithFaultAttribute(IOperationsManager operationsManager, string displayName)
            : base(operationsManager, displayName)
        {
        }

        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME, Generate = OperationGeneration.Sync)]
    [OperationFaultContract(typeof(FaultDetail))]
    abstract class TestOperationWithFaultAttributeBase<T> : OperationBase<T>
    {
        public TestOperationWithFaultAttributeBase(IOperationsManager operationsManager, string displayName)
            : base(operationsManager, displayName)
        {
        }

        protected abstract override T Run();
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME, Generate = OperationGeneration.Sync)]
    class TestOperationWithFaultAttributeInherited : TestOperationWithFaultAttributeBase<Result>
    {
        public TestOperationWithFaultAttributeInherited(IOperationsManager operationsManager, string displayName)
            : base(operationsManager, displayName)
        {
        }

        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME)]
    class TestOperationWithBoolParameter : OperationBase<Result>
    {
        public TestOperationWithBoolParameter(bool input, IOperationsManager operationsManager)
            : base(operationsManager, "displayName")
        {
        }
        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME)]
    class TestOperationWithStringParameter : OperationBase<Result>
    {
        public TestOperationWithStringParameter(string input, IOperationsManager operationsManager)
            : base(operationsManager, "displayName")
        {
        }
        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    [Operation(OperaionsGenrationTools_TestFixture.SERVICE_SORT_NAME)]
    class TestOperationWithStringAndObjectParameters : OperationBase<Result>
    {
        public TestOperationWithStringAndObjectParameters(Parameters parameters, string input, IOperationsManager operationsManager)
            : base(operationsManager, "displayName")
        {
        }
        protected override Result Run()
        {
            throw new NotImplementedException();
        }
    }

    internal class FaultDetail
    {
    }

    class Result
    { }

    class Parameters
    {

    }
}
