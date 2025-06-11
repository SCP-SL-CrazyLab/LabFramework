using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LabFramework.Core.DependencyInjection
{
    /// <summary>
    /// Service lifetime enumeration
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// A new instance is created every time
        /// </summary>
        Transient,
        
        /// <summary>
        /// A single instance is created and reused
        /// </summary>
        Singleton,
        
        /// <summary>
        /// A single instance per scope
        /// </summary>
        Scoped
    }
    
    /// <summary>
    /// Service descriptor for dependency injection
    /// </summary>
    public class ServiceDescriptor
    {
        public Type ServiceType { get; set; }
        public Type ImplementationType { get; set; }
        public object Instance { get; set; }
        public Func<IServiceProvider, object> Factory { get; set; }
        public ServiceLifetime Lifetime { get; set; }
    }
    
    /// <summary>
    /// Simple dependency injection container
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Register a service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Implementation type</typeparam>
        /// <param name="lifetime">Service lifetime</param>
        void Register<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TImplementation : class, TService;
        
        /// <summary>
        /// Register a service with a factory
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="factory">Factory function</param>
        /// <param name="lifetime">Service lifetime</param>
        void Register<TService>(Func<IServiceProvider, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Transient);
        
        /// <summary>
        /// Register a singleton instance
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="instance">Service instance</param>
        void RegisterSingleton<TService>(TService instance);
        
        /// <summary>
        /// Resolve a service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Service instance</returns>
        TService Resolve<TService>();
        
        /// <summary>
        /// Resolve a service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Service instance</returns>
        object Resolve(Type serviceType);
        
        /// <summary>
        /// Try to resolve a service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="service">Resolved service</param>
        /// <returns>True if service was resolved</returns>
        bool TryResolve<TService>(out TService service);
    }
    
    /// <summary>
    /// Simple dependency injection container implementation
    /// </summary>
    public class ServiceContainer : IServiceContainer, IServiceProvider
    {
        private readonly ConcurrentDictionary<Type, ServiceDescriptor> _services = new();
        private readonly ConcurrentDictionary<Type, object> _singletonInstances = new();
        
        public void Register<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TImplementation : class, TService
        {
            _services[typeof(TService)] = new ServiceDescriptor
            {
                ServiceType = typeof(TService),
                ImplementationType = typeof(TImplementation),
                Lifetime = lifetime
            };
        }
        
        public void Register<TService>(Func<IServiceProvider, TService> factory, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            _services[typeof(TService)] = new ServiceDescriptor
            {
                ServiceType = typeof(TService),
                Factory = provider => factory(provider),
                Lifetime = lifetime
            };
        }
        
        public void RegisterSingleton<TService>(TService instance)
        {
            _services[typeof(TService)] = new ServiceDescriptor
            {
                ServiceType = typeof(TService),
                Instance = instance,
                Lifetime = ServiceLifetime.Singleton
            };
            _singletonInstances[typeof(TService)] = instance;
        }
        
        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }
        
        public object Resolve(Type serviceType)
        {
            if (!_services.TryGetValue(serviceType, out var descriptor))
            {
                throw new InvalidOperationException($"Service of type {serviceType.Name} is not registered.");
            }
            
            return CreateInstance(descriptor);
        }
        
        public bool TryResolve<TService>(out TService service)
        {
            try
            {
                service = Resolve<TService>();
                return true;
            }
            catch
            {
                service = default;
                return false;
            }
        }
        
        public object GetService(Type serviceType)
        {
            return TryResolve(serviceType, out var service) ? service : null;
        }
        
        private bool TryResolve(Type serviceType, out object service)
        {
            try
            {
                service = Resolve(serviceType);
                return true;
            }
            catch
            {
                service = null;
                return false;
            }
        }
        
        private object CreateInstance(ServiceDescriptor descriptor)
        {
            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                if (_singletonInstances.TryGetValue(descriptor.ServiceType, out var existingInstance))
                {
                    return existingInstance;
                }
            }
            
            object instance;
            
            if (descriptor.Instance != null)
            {
                instance = descriptor.Instance;
            }
            else if (descriptor.Factory != null)
            {
                instance = descriptor.Factory(this);
            }
            else if (descriptor.ImplementationType != null)
            {
                instance = CreateInstance(descriptor.ImplementationType);
            }
            else
            {
                throw new InvalidOperationException($"Cannot create instance of {descriptor.ServiceType.Name}");
            }
            
            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                _singletonInstances[descriptor.ServiceType] = instance;
            }
            
            return instance;
        }
        
        private object CreateInstance(Type type)
        {
            var constructors = type.GetConstructors();
            var constructor = constructors[0]; // Use first constructor for simplicity
            
            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = Resolve(parameters[i].ParameterType);
            }
            
            return Activator.CreateInstance(type, args);
        }
    }
}

