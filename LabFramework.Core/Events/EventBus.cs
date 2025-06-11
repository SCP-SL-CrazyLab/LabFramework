using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LabFramework.Core.Events
{
    /// <summary>
    /// Represents an event that can be handled by the framework
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Unique identifier for this event
        /// </summary>
        string EventId { get; }
        
        /// <summary>
        /// Timestamp when the event was created
        /// </summary>
        DateTime Timestamp { get; }
        
        /// <summary>
        /// Whether this event can be cancelled
        /// </summary>
        bool IsCancellable { get; }
        
        /// <summary>
        /// Whether this event has been cancelled
        /// </summary>
        bool IsCancelled { get; set; }
    }
    
    /// <summary>
    /// Base implementation of IEvent
    /// </summary>
    public abstract class BaseEvent : IEvent
    {
        public string EventId { get; }
        public DateTime Timestamp { get; }
        public virtual bool IsCancellable => false;
        public bool IsCancelled { get; set; }
        
        protected BaseEvent()
        {
            EventId = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }
    }
    
    /// <summary>
    /// Event handler delegate
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    /// <param name="eventArgs">Event arguments</param>
    /// <returns>Task for async handling</returns>
    public delegate Task EventHandler<in T>(T eventArgs) where T : IEvent;
    
    /// <summary>
    /// Event bus interface for publishing and subscribing to events
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Subscribe to an event type
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        void Subscribe<T>(EventHandler<T> handler) where T : IEvent;
        
        /// <summary>
        /// Unsubscribe from an event type
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="handler">Event handler</param>
        void Unsubscribe<T>(EventHandler<T> handler) where T : IEvent;
        
        /// <summary>
        /// Publish an event
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="eventArgs">Event arguments</param>
        /// <returns>Task for async publishing</returns>
        Task PublishAsync<T>(T eventArgs) where T : IEvent;
        
        /// <summary>
        /// Publish an event synchronously
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="eventArgs">Event arguments</param>
        void Publish<T>(T eventArgs) where T : IEvent;
    }
    
    /// <summary>
    /// High-performance event bus implementation
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();
        private readonly object _lock = new();
        
        public void Subscribe<T>(EventHandler<T> handler) where T : IEvent
        {
            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_handlers.ContainsKey(eventType))
                {
                    _handlers[eventType] = new List<Delegate>();
                }
                _handlers[eventType].Add(handler);
            }
        }
        
        public void Unsubscribe<T>(EventHandler<T> handler) where T : IEvent
        {
            lock (_lock)
            {
                var eventType = typeof(T);
                if (_handlers.ContainsKey(eventType))
                {
                    _handlers[eventType].Remove(handler);
                    if (_handlers[eventType].Count == 0)
                    {
                        _handlers.Remove(eventType);
                    }
                }
            }
        }
        
        public async Task PublishAsync<T>(T eventArgs) where T : IEvent
        {
            List<Delegate> handlers;
            lock (_lock)
            {
                var eventType = typeof(T);
                if (!_handlers.ContainsKey(eventType))
                    return;
                
                handlers = new List<Delegate>(_handlers[eventType]);
            }
            
            var tasks = new List<Task>();
            foreach (var handler in handlers)
            {
                if (handler is EventHandler<T> typedHandler)
                {
                    tasks.Add(typedHandler(eventArgs));
                }
            }
            
            await Task.WhenAll(tasks);
        }
        
        public void Publish<T>(T eventArgs) where T : IEvent
        {
            PublishAsync(eventArgs).GetAwaiter().GetResult();
        }
    }
}

