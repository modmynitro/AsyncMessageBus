using System;
using System.Threading.Tasks;

namespace AsyncMessageBus.Interfaces
{
    public interface ISubscription
    {
        Type MessageType { get; }

        bool IsAlive { get; }

        Task SendMessage(IMessage message);

        bool HasReceiver(object receiver);
    }
}
