using AsyncMessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncMessageBus
{
    public class AsyncMessageBus : IAsyncMessageBus
    {
        private readonly IDispatcher _dispatcher;
        private readonly List<ISubscription> _subscriptions = new List<ISubscription>();

        public AsyncMessageBus()
        {
            _dispatcher = DefaultDispatcher.Dispatcher;
        }

        public AsyncMessageBus(IDispatcher dispatcher)
        {
            if (dispatcher is null)
                throw new ArgumentNullException(nameof(dispatcher));

            _dispatcher = dispatcher; 
        }

        public void Subscribe<TMessage>(object receiver, Func<TMessage, Task> callback)
            where TMessage : class, IMessage
        {
            if (receiver is null)
                throw new ArgumentNullException(nameof(receiver));

            if (callback is null)
                throw new ArgumentNullException(nameof(callback));
            
            if (_dispatcher.InvokeRequired)
            {
                _dispatcher.Invoke(() => SubscribeInternal(receiver, callback));
            }
            else
            {
                SubscribeInternal(receiver, callback);
            }
        }

        private void SubscribeInternal<TMessage>(object receiver, Func<TMessage, Task> callback)
            where TMessage : class, IMessage
        {
            var subscription = new Subscription<TMessage>(receiver, callback);

            _subscriptions.Add(subscription);
        }

        public void UnSubscribe<TMessage>(object receiver)
            where TMessage : class, IMessage
        {
            if (receiver is null)
                throw new ArgumentNullException(nameof(receiver));

            if (_dispatcher.InvokeRequired)
            {
                _dispatcher.Invoke(() => UnSubscribeInternal<TMessage>(receiver));
            }
            else
            {
                UnSubscribeInternal<TMessage>(receiver);
            }
        }

        private bool UnSubscribeInternal<TMessage>(object receiver)
            where TMessage : class, IMessage
        {
            foreach(var subscription in _subscriptions)
            {
                if (subscription.MessageType == typeof(TMessage) &&
                    subscription.HasReceiver(receiver))
                {
                    _subscriptions.Remove(subscription);
                    return true;
                }
            }

            return false;
        }

        public async Task SendMessage<TMessage>(TMessage message)
            where TMessage : class, IMessage
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (_dispatcher.InvokeRequired)
            {
                await _dispatcher.InvokeAsync(async () => await SendMessageInternal(message));
            }
            else
            {
                await SendMessageInternal(message);
            }
        }

        public async Task SendMessageInternal<TMessage>(TMessage message)
            where TMessage : class, IMessage
        {
            List<Task> taskList = new List<Task>();

            foreach(ISubscription subscription in _subscriptions.Where(s => s.MessageType == typeof(TMessage)))
            {
                taskList.Add(subscription.SendMessage(message));
            }

            _subscriptions.RemoveAll(s => !s.IsAlive);

            await Task.WhenAll(taskList);
        }
    }
}
