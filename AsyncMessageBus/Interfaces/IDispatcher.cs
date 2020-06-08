using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncMessageBus.Interfaces
{
    public interface IDispatcher
    {
        bool InvokeRequired { get; }
        
        Task InvokeAsync(Action callback);
        void Invoke(Action callback);
    }
}
