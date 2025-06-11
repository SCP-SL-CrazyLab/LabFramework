using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using LabFramework.Core;
using LabFramework.Core.Logging;
using LabFramework.Core.DependencyInjection;

namespace LabFramework.Loader
{
    /// <summary>
    /// Enhanced plugin loader with display capabilities
    /// </summary>
    public class PluginLoader
    {
        private readonly ILoggingService _logger;
        private readonly IServiceContainer _serviceContainer;
        private readonly LoaderDisplay _display;
        private readonly List<IPlugin> _loadedPlugins = new();
        private readonly string _pluginsDirectory;

        public PluginLoader(ILoggingService logger, IServiceContainer serviceContainer, string pluginsDirectory = "plugins")
        {
            _logger = logger;
            _serviceContainer = serviceContainer;
            _display = new LoaderDisplay(logger);
            _pluginsDirectory = pluginsDirectory;
        }

        /// <summary>
        /// Load all plugins with enhanced display
        /// </summary>
        public async Task LoadAllPluginsAsync()
        {
            _display.ShowStartupBanner();
            _display.ShowPluginStatus();

            Console.WriteLine("Loading plugins...");
            Console.WriteLine();

            if (!Directory.Exists(_pluginsDirectory))
            {
                Directory.CreateDirectory(_pluginsDirectory);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  Created plugins directory: {_pluginsDirectory}");
                Console.ResetColor();
                _display.ShowLoadingSummary();
                return;
            }

            var pluginFiles = Directory.GetFiles(_pluginsDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            
            if (!pluginFiles.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  No plugin files found in plugins directory");
                Console.ResetColor();
                _display.ShowLoadingSummary();
                return;
            }

            Console.WriteLine($"Found {pluginFiles.Length} plugin file(s):");
            Console.WriteLine();

            foreach (var pluginFile in pluginFiles)
            {
                await LoadPluginFromFileAsync(pluginFile);
            }

            _display.ShowLoadingSummary();
            _display.ShowDetailedErrors();

            _logger.LogInformation($"Plugin loading completed: {_loadedPlugins.Count} plugins loaded successfully");
        }

        /// <summary>
        /// Load a single plugin from file
        /// </summary>
        private async Task LoadPluginFromFileAsync(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Console.Write($"  Loading {fileName}... ");

                // Load assembly
                var assembly = Assembly.LoadFrom(filePath);
                
                // Find plugin types
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    .ToList();

                if (!pluginTypes.Any())
                {
                    throw new InvalidOperationException("No plugin classes found in assembly");
                }

                if (pluginTypes.Count > 1)
                {
                    _logger.LogWarning($"Multiple plugin classes found in {fileName}, using first one");
                }

                var pluginType = pluginTypes.First();
                
                // Create plugin instance
                var plugin = (IPlugin?)Activator.CreateInstance(pluginType);
                
                if (plugin == null)
                {
                    throw new InvalidOperationException("Failed to create plugin instance");
                }

                // Validate plugin
                ValidatePlugin(plugin);

                // Load plugin
                 plugin.OnLoadAsync();
                
                stopwatch.Stop();
                
                // Register with framework
                await LabFrameworkCore.Instance.RegisterPluginAsync(plugin);
                _loadedPlugins.Add(plugin);

                _display.AddLoadedPlugin(plugin, stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _display.AddFailedPlugin(fileName, ex);
            }
        }

        /// <summary>
        /// Validate plugin before loading
        /// </summary>
        private void ValidatePlugin(IPlugin plugin)
        {
            if (string.IsNullOrWhiteSpace(plugin.Name))
                throw new InvalidOperationException("Plugin name cannot be empty");

            if (string.IsNullOrWhiteSpace(plugin.Version))
                throw new InvalidOperationException("Plugin version cannot be empty");

            if (string.IsNullOrWhiteSpace(plugin.Author))
                throw new InvalidOperationException("Plugin author cannot be empty");

            // Check for duplicate names
            if (_loadedPlugins.Any(p => p.Name.Equals(plugin.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Plugin with name '{plugin.Name}' is already loaded");
        }

        /// <summary>
        /// Unload all plugins
        /// </summary>
        public async Task UnloadAllPluginsAsync()
        {
            Console.WriteLine();
            Console.WriteLine("Unloading plugins...");
            Console.WriteLine();

            var pluginsCopy = new List<IPlugin>(_loadedPlugins);
            
            foreach (var plugin in pluginsCopy)
            {
                await UnloadPluginAsync(plugin);
            }

            Console.WriteLine($"Unloaded {pluginsCopy.Count} plugin(s)");
            _logger.LogInformation($"All plugins unloaded successfully");
        }

        /// <summary>
        /// Unload a specific plugin
        /// </summary>
        private async Task UnloadPluginAsync(IPlugin plugin)
        {
            try
            {
                Console.Write($"  Unloading {plugin.Name}... ");
                
                 plugin.OnUnloadAsync();
                await LabFrameworkCore.Instance.UnregisterPluginAsync(plugin);
                _loadedPlugins.Remove(plugin);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ {ex.Message}");
                Console.ResetColor();
                
                _logger.LogError($"Failed to unload plugin {plugin.Name}", ex);
            }
        }

        /// <summary>
        /// Get loader statistics
        /// </summary>
        public PluginStatistics GetStatistics()
        {
            return _display.GetStatistics();
        }

        /// <summary>
        /// Get list of loaded plugins
        /// </summary>
        public IReadOnlyList<IPlugin> GetLoadedPlugins()
        {
            return _loadedPlugins.AsReadOnly();
        }

        /// <summary>
        /// Reload a specific plugin
        /// </summary>
        public async Task<bool> ReloadPluginAsync(string pluginName)
        {
            var plugin = _loadedPlugins.FirstOrDefault(p => 
                p.Name.Equals(pluginName, StringComparison.OrdinalIgnoreCase));

            if (plugin == null)
            {
                _logger.LogWarning($"Plugin '{pluginName}' not found for reload");
                return false;
            }

            try
            {
                Console.WriteLine($"Reloading plugin: {plugin.Name}");
                
                // Unload
                await UnloadPluginAsync(plugin);
                
                // Find and reload the plugin file
                var pluginFiles = Directory.GetFiles(_pluginsDirectory, "*.dll", SearchOption.TopDirectoryOnly);
                var pluginFile = pluginFiles.FirstOrDefault(f => 
                    Path.GetFileNameWithoutExtension(f).Equals(pluginName, StringComparison.OrdinalIgnoreCase));

                if (pluginFile != null)
                {
                    await LoadPluginFromFileAsync(pluginFile);
                    return true;
                }
                else
                {
                    _logger.LogError($"Plugin file for '{pluginName}' not found");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to reload plugin '{pluginName}'", ex);
                return false;
            }
        }

        /// <summary>
        /// Show current plugin status
        /// </summary>
        public void ShowStatus()
        {
            _display.ShowPluginStatus();
            
            if (_loadedPlugins.Any())
            {
                Console.WriteLine("Currently loaded plugins:");
                foreach (var plugin in _loadedPlugins.OrderBy(p => p.Name))
                {
                    Console.WriteLine($"  • {plugin.Name} v{plugin.Version} by {plugin.Author}");
                    Console.WriteLine($"    Description: {plugin.Description}");
                }
            }
            else
            {
                Console.WriteLine("No plugins currently loaded.");
            }
        }
    }
}

