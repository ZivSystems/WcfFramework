using System;

namespace Ziv.ServiceModel.Client
{
    public interface IProxy<out TContract> : IDisposable
    {
        TResult Execute<TResult>(Func<TContract, TResult> operation);
        void Execute(Action<TContract> operation);
    }
}
