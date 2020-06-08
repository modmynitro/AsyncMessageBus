using AsyncMessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AsyncMessageBus
{
    internal class DefaultDispatcher : IDispatcher
    {
        private DefaultDispatcher()
        {
        }

        public static IDispatcher Dispatcher { get; } = new DefaultDispatcher();

        public bool InvokeRequired => false;

        public Task InvokeAsync(Action callback) => throw new NotImplementedException();

        public void Invoke(Action callback) => throw new NotImplementedException();
    }
}
