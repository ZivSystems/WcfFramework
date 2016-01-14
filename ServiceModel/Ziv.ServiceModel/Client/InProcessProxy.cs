using System;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Client
{
    internal class InProcessProxy<TContract> : IProxy<TContract>
    {
        private readonly TContract _serviceInstance;

        public InProcessProxy(TContract serviceInstance)
        {
            _serviceInstance = serviceInstance;
            Logger.LogEvent(string.Format("InProcessFactory of contract type {0} has been constructed.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
        }

        public TResult Execute<TResult>(Func<TContract, TResult> operation)
        {
            Logger.LogEvent(string.Format("Executing in-process operation on contract of type {0}.", typeof(TContract).Name), this, ImportanceLevels.gUnimportant);
            var result = operation(_serviceInstance);
            Logger.LogEvent("In-process operation execution has been completed.", this, ImportanceLevels.gUnimportant);
            return result;
        }

        public void Execute(Action<TContract> operation)
        {
            Execute(new Func<TContract, object>(contract => { operation(contract); return null; }));
        }

        public void Dispose()
        {
            // No disposable resources are used.
        }
    }
}