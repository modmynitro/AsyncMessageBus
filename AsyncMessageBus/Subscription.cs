using AsyncMessageBus.Interfaces;

using System;
using System.Threading.Tasks;

namespace AsyncMessageBus
{
    internal class Subscription<TMessage> :
        ISubscription
        where TMessage : class, IMessage
    {
        protected WeakReference<Func<TMessage, Task>>? _deliveryFunction;

        public Subscription(object receíver, Func<TMessage, Task> callback)
        {
            if (receíver != callback.Target)
                throw new ArgumentException(nameof(callback));

            _deliveryFunction = new WeakReference<Func<TMessage, Task>>(callback);
        }

        public bool IsAlive => _deliveryFunction != null;
        public Type MessageType => typeof(TMessage);

        private Func<TMessage, Task>? DeliveryFunction
        {
            get
            {
                if (_deliveryFunction == null)
                    return null;

                if (_deliveryFunction.TryGetTarget(out Func<TMessage, Task> func))
                    return func;

                _deliveryFunction = null;
                return null;
            }
        }

        public bool HasReceiver(object receiver)
        {
            if (receiver is null)
                throw new ArgumentNullException(nameof(receiver));

            return DeliveryFunction?.Target == receiver;
        }

        public async Task SendMessage(IMessage message)
        {
            await this.SendMessage((TMessage)message);
        }

        private async Task SendMessage(TMessage message)
        {
            Func<TMessage, Task>? func = DeliveryFunction;

            if (func == null)
                return;

            await func(message);
        }
    }
}