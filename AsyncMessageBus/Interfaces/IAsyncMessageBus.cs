using System;
using System.Threading.Tasks;

namespace AsyncMessageBus.Interfaces
{
    public interface IAsyncMessageBus
    {
        Task SendMessage<TMessage>(TMessage message)
            where TMessage : class, IMessage;

        void Subscribe<TMessage>(object receiver, Func<TMessage, Task> func)
            where TMessage : class, IMessage;
    }
}