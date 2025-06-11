using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LabFramework.Core.Events;
using LabFramework.Core.Logging;
using LabFramework.Core.Configuration;
using LabFramework.Core.DependencyInjection;

namespace LabFramework.Core
{
    /// <summary>
    /// Main framework class that coordinates all services
    /// </summary>
    public class LabFrameworkCore
    {
        private static LabFrameworkCore _instance;
        private static readonly object _lock = new();
        
        public static LabFrameworkCore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new LabFrameworkCore();
                    }
                }
                return _instance;
            }
        }
        
        public IServiceContainer ServiceContainer { get; private set; }
        public IEventBus EventBus { get; private set; }
        public ILoggingService Logger { get; private set; }
        public IConfigurationService Configuration { get; private set; }
        
        private readonly List<IPlugin> _loadedPlugins = new();
        private bool _isInitialized = false;
        
        private LabFrameworkCore()
        {
            ServiceContainer = new ServiceContainer();
            EventBus = new EventBus();
            Logger = new ConsoleLoggingService();
            Configuration = new JsonConfigurationService();
            
            RegisterCoreServices();
        }
        
        /// <summary>
        /// Initialize the framework
        /// </summary>
        /// <returns>Task for async initialization</returns>
        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;
            
            Logger.LogInformation("Initializing LabFramework...");
            
            // Load configuration
            await Configuration.LoadFromFileAsync("config.json");
            
            // Initialize core services
            Logger.LogInformation("Core services initialized");
            
            // Load plugins
            await LoadPluginsAsync();
            
            _isInitialized = true;
            Logger.LogInformation("LabFramework initialization completed");
        }
        
        /// <summary>
        /// Shutdown the framework
        /// </summary>
        /// <returns>Task for async shutdown</returns>
        public async Task ShutdownAsync()
        {
            if (!_isInitialized)
                return;
            
            Logger.LogInformation("Shutting down LabFramework...");
            
            // Unload plugins
            await UnloadPluginsAsync();
            
            // Save configuration
            await Configuration.SaveToFileAsync("config.json");
            
            _isInitialized = false;
            Logger.LogInformation("LabFramework shutdown completed");
        }
        
        /// <summary>
        /// Register a plugin
        /// </summary>
        /// <param name="plugin">Plugin to register</param>
        public async Task RegisterPluginAsync(IPlugin plugin)
        {
            if (_loadedPlugins.Contains(plugin))
                return;
            
            Logger.LogInformation($"Registering plugin: {plugin.Name}");
            
            try
            {
                 plugin.OnLoadAsync();
                _loadedPlugins.Add(plugin);
                Logger.LogInformation($"Plugin registered successfully: {plugin.Name}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to register plugin {plugin.Name}", ex);
                throw;
            }
        }
        
        /// <summary>
        /// Unregister a plugin
        /// </summary>
        /// <param name="plugin">Plugin to unregister</param>
        public async Task UnregisterPluginAsync(IPlugin plugin)
        {
            if (!_loadedPlugins.Contains(plugin))
                return;
            
            Logger.LogInformation($"Unregistering plugin: {plugin.Name}");
            
            try
            {
                 plugin.OnUnloadAsync();
                _loadedPlugins.Remove(plugin);
                Logger.LogInformation($"Plugin unregistered successfully: {plugin.Name}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to unregister plugin {plugin.Name}", ex);
            }
        }
        
        private void RegisterCoreServices()
        {
            ServiceContainer.RegisterSingleton<IServiceContainer>(ServiceContainer);
            ServiceContainer.RegisterSingleton<IEventBus>(EventBus);
            ServiceContainer.RegisterSingleton<ILoggingService>(Logger);
            ServiceContainer.RegisterSingleton<IConfigurationService>(Configuration);
        }
        
        private async Task LoadPluginsAsync()
        {
            // TODO: Implement plugin discovery and loading
            Logger.LogInformation("Plugin loading system ready");
        }
        
        private async Task UnloadPluginsAsync()
        {
            var pluginsCopy = new List<IPlugin>(_loadedPlugins);
            foreach (var plugin in pluginsCopy)
            {
                await UnregisterPluginAsync(plugin);
            }
        }
    }
    
    /// <summary>
    /// Plugin interface
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Plugin name
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Plugin version
        /// </summary>
        string Version { get; }
        
        /// <summary>
        /// Plugin author
        /// </summary>
        string Author { get; }
        
        /// <summary>
        /// Plugin description
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Called when the plugin is loaded
        /// </summary>
        /// <returns>Task for async loading</returns>
        void OnLoadAsync();
        
        /// <summary>
        /// Called when the plugin is unloaded
        /// </summary>
        /// <returns>Task for async unloading</returns>
        void OnUnloadAsync();
    }
    
    /// <summary>
    /// Base plugin class
    /// </summary>
    public abstract class BasePlugin : IPlugin
    {
        public abstract string Name { get; }
        public abstract string Version { get; }
        public abstract string Author { get; }
        public abstract string Description { get; }
        
        protected IEventBus EventBus => LabFrameworkCore.Instance.EventBus;
        protected ILoggingService Logger => LabFrameworkCore.Instance.Logger;
        protected IConfigurationService Configuration => LabFrameworkCore.Instance.Configuration;
        protected IServiceContainer ServiceContainer => LabFrameworkCore.Instance.ServiceContainer;
        
        public virtual async void OnLoadAsync()
        {
            Logger.LogInformation($"Loading plugin: {Name} v{Version} by {Author}");
            await Task.CompletedTask;
        }
        
        public virtual async void OnUnloadAsync()
        {
            Logger.LogInformation($"Unloading plugin: {Name}");
            await Task.CompletedTask;
        }
    }
}

