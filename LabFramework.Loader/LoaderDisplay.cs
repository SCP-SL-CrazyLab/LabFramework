using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabFramework.Core;
using LabFramework.Core.Logging;

namespace LabFramework.Loader
{
    /// <summary>
    /// Manages the display of plugin status and framework information
    /// </summary>
    public class LoaderDisplay
    {
        private readonly ILoggingService _logger;
        private readonly List<PluginLoadInfo> _loadedPlugins = new();
        private readonly List<PluginError> _errors = new();
        
        public LoaderDisplay(ILoggingService logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Display the startup banner with framework information
        /// </summary>
        public void ShowStartupBanner()
        {
            var banner = GenerateStartupBanner();
            Console.WriteLine(banner);
            _logger.LogInformation("LabFramework startup banner displayed");
        }

        /// <summary>
        /// Display plugin loading status
        /// </summary>
        public void ShowPluginStatus()
        {
            var statusDisplay = GeneratePluginStatusDisplay();
            Console.WriteLine(statusDisplay);
            _logger.LogInformation("Plugin status displayed");
        }

        /// <summary>
        /// Add a successfully loaded plugin to the display
        /// </summary>
        public void AddLoadedPlugin(IPlugin plugin, TimeSpan loadTime)
        {
            var info = new PluginLoadInfo
            {
                Plugin = plugin,
                LoadTime = loadTime,
                Status = PluginStatus.Loaded,
                LoadedAt = DateTime.UtcNow
            };
            
            _loadedPlugins.Add(info);
            
            var message = $"✓ {plugin.Name} v{plugin.Version} loaded successfully ({loadTime.TotalMilliseconds:F0}ms)";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  {message}");
            Console.ResetColor();
            
            _logger.LogInformation($"Plugin loaded: {plugin.Name} v{plugin.Version} in {loadTime.TotalMilliseconds:F0}ms");
        }

        /// <summary>
        /// Add a failed plugin load to the display
        /// </summary>
        public void AddFailedPlugin(string pluginName, Exception error)
        {
            var pluginError = new PluginError
            {
                PluginName = pluginName,
                Error = error,
                ErrorTime = DateTime.UtcNow,
                ErrorType = GetErrorType(error)
            };
            
            _errors.Add(pluginError);
            
            var message = $"✗ {pluginName} failed to load: {error.Message}";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  {message}");
            Console.ResetColor();
            
            _logger.LogError($"Plugin load failed: {pluginName}", error);
        }

        /// <summary>
        /// Display loading summary
        /// </summary>
        public void ShowLoadingSummary()
        {
            var summary = GenerateLoadingSummary();
            Console.WriteLine(summary);
            _logger.LogInformation($"Plugin loading completed: {_loadedPlugins.Count} loaded, {_errors.Count} failed");
        }

        /// <summary>
        /// Display detailed error information
        /// </summary>
        public void ShowDetailedErrors()
        {
            if (!_errors.Any())
                return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("                        ERROR DETAILS                          ");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.ResetColor();

            foreach (var error in _errors)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Plugin: {error.PluginName}");
                Console.WriteLine($"Error Type: {error.ErrorType}");
                Console.WriteLine($"Time: {error.ErrorTime:yyyy-MM-dd HH:mm:ss}");
                Console.ResetColor();
                
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Message: {error.Error.Message}");
                
                if (error.Error.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {error.Error.InnerException.Message}");
                }
                
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(error.Error.StackTrace);
                Console.ResetColor();
                Console.WriteLine(new string('-', 60));
            }
        }

        /// <summary>
        /// Get current plugin statistics
        /// </summary>
        public PluginStatistics GetStatistics()
        {
            return new PluginStatistics
            {
                TotalPlugins = _loadedPlugins.Count + _errors.Count,
                LoadedPlugins = _loadedPlugins.Count,
                FailedPlugins = _errors.Count,
                AverageLoadTime = _loadedPlugins.Any() ? _loadedPlugins.Average(p => p.LoadTime.TotalMilliseconds) : 0,
                TotalLoadTime = _loadedPlugins.Sum(p => p.LoadTime.TotalMilliseconds),
                LoadedPluginsList = _loadedPlugins.ToList(),
                ErrorsList = _errors.ToList()
            };
        }

        private string GenerateStartupBanner()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine();
            sb.AppendLine("╔══════════════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                                                                              ║");
            sb.AppendLine("║    ██╗      █████╗ ██████╗ ███████╗██████╗  █████╗ ███╗   ███╗███████╗     ║");
            sb.AppendLine("║    ██║     ██╔══██╗██╔══██╗██╔════╝██╔══██╗██╔══██╗████╗ ████║██╔════╝     ║");
            sb.AppendLine("║    ██║     ███████║██████╔╝█████╗  ██████╔╝███████║██╔████╔██║█████╗       ║");
            sb.AppendLine("║    ██║     ██╔══██║██╔══██╗██╔══╝  ██╔══██╗██╔══██║██║╚██╔╝██║██╔══╝       ║");
            sb.AppendLine("║    ███████╗██║  ██║██████╔╝██║     ██║  ██║██║  ██║██║ ╚═╝ ██║███████╗     ║");
            sb.AppendLine("║    ╚══════╝╚═╝  ╚═╝╚═════╝ ╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝     ║");
            sb.AppendLine("║                                                                              ║");
            sb.AppendLine("║                    Advanced Plugin Framework for SCP:SL                     ║");
            sb.AppendLine("║                                                                              ║");
            sb.AppendLine($"║    Version: 1.0.0                          Build: {DateTime.UtcNow:yyyyMMdd}                    ║");
            sb.AppendLine("║    Author: Rakun Loader              License: MIT                     ║");
            sb.AppendLine("║                                                                              ║");
            sb.AppendLine("║    🚀 High Performance  🔧 Easy Development  🛡️ Advanced Permissions       ║");
            sb.AppendLine("║    🎮 Custom Items      ⚙️ Flexible Config   📚 Comprehensive Docs         ║");
            sb.AppendLine("║                                                                              ║");
            sb.AppendLine("║    Powered by LabAPI • Better than EXILED • Built for the future          ║");
            sb.AppendLine("║                                                                              ║");
            sb.AppendLine("╚══════════════════════════════════════════════════════════════════════════════╝");
            sb.AppendLine();
            
            return sb.ToString();
        }

        private string GeneratePluginStatusDisplay()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("┌─────────────────────────────────────────────────────────────────────────────┐");
            sb.AppendLine("│                            PLUGIN LOADING STATUS                           │");
            sb.AppendLine("├─────────────────────────────────────────────────────────────────────────────┤");
            
            if (!_loadedPlugins.Any() && !_errors.Any())
            {
                sb.AppendLine("│  No plugins found to load                                                  │");
            }
            else
            {
                sb.AppendLine($"│  Total Plugins: {_loadedPlugins.Count + _errors.Count,-3} │ Loaded: {_loadedPlugins.Count,-3} │ Failed: {_errors.Count,-3} │ Success Rate: {GetSuccessRate():F1}%  │");
            }
            
            sb.AppendLine("└─────────────────────────────────────────────────────────────────────────────┘");
            sb.AppendLine();
            
            return sb.ToString();
        }

        private string GenerateLoadingSummary()
        {
            var sb = new StringBuilder();
            var stats = GetStatistics();
            
            sb.AppendLine();
            sb.AppendLine("┌─────────────────────────────────────────────────────────────────────────────┐");
            sb.AppendLine("│                            LOADING SUMMARY                                  │");
            sb.AppendLine("├─────────────────────────────────────────────────────────────────────────────┤");
            sb.AppendLine($"│  Plugins Loaded: {stats.LoadedPlugins,-10} │ Failed: {stats.FailedPlugins,-10} │ Total: {stats.TotalPlugins,-10}        │");
            sb.AppendLine($"│  Average Load Time: {stats.AverageLoadTime:F1} ms    │ Total Time: {stats.TotalLoadTime:F1} ms           │");
            sb.AppendLine($"│  Success Rate: {GetSuccessRate():F1}%              │ Framework Status: READY            │");
            sb.AppendLine("└─────────────────────────────────────────────────────────────────────────────┘");
            
            if (_loadedPlugins.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Loaded Plugins:");
                foreach (var plugin in _loadedPlugins.OrderBy(p => p.Plugin.Name))
                {
                    sb.AppendLine($"  • {plugin.Plugin.Name} v{plugin.Plugin.Version} by {plugin.Plugin.Author}");
                }
            }
            
            if (_errors.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Failed Plugins:");
                foreach (var error in _errors.OrderBy(e => e.PluginName))
                {
                    sb.AppendLine($"  • {error.PluginName} - {error.ErrorType}: {error.Error.Message}");
                }
                sb.AppendLine();
                sb.AppendLine("Use 'labframework errors' command for detailed error information.");
            }
            
            sb.AppendLine();
            
            return sb.ToString();
        }

        private double GetSuccessRate()
        {
            var total = _loadedPlugins.Count + _errors.Count;
            return total == 0 ? 100.0 : (_loadedPlugins.Count * 100.0) / total;
        }

        private ErrorType GetErrorType(Exception error)
        {
            return error switch
            {
                FileNotFoundException => ErrorType.FileNotFound,
                TypeLoadException => ErrorType.TypeLoad,
                BadImageFormatException => ErrorType.BadImageFormat,
                UnauthorizedAccessException => ErrorType.UnauthorizedAccess,
                _ => ErrorType.Unknown
            };
        }
    }

    /// <summary>
    /// Information about a loaded plugin
    /// </summary>
    public class PluginLoadInfo
    {
        public  IPlugin Plugin { get; set; }
        public TimeSpan LoadTime { get; set; }
        public PluginStatus Status { get; set; }
        public DateTime LoadedAt { get; set; }
    }

    /// <summary>
    /// Information about a plugin loading error
    /// </summary>
    public class PluginError
    {
        public  string PluginName { get; set; }
        public  Exception Error { get; set; }
        public DateTime ErrorTime { get; set; }
        public ErrorType ErrorType { get; set; }
    }
    

    /// <summary>
    /// Plugin loading statistics
    /// </summary>
    public class PluginStatistics
    {
        public int TotalPlugins { get; set; }
        public int LoadedPlugins { get; set; }
        public int FailedPlugins { get; set; }
        public double AverageLoadTime { get; set; }
        public double TotalLoadTime { get; set; }
        public List<PluginLoadInfo> LoadedPluginsList { get; set; } = new();
        public List<PluginError> ErrorsList { get; set; } = new();
    }

    /// <summary>
    /// Plugin status enumeration
    /// </summary>
    public enum PluginStatus
    {
        Loading,
        Loaded,
        Failed,
        Unloaded
    }

    /// <summary>
    /// Error type enumeration
    /// </summary>
    public enum ErrorType
    {
        Unknown,
        FileNotFound,
        TypeLoad,
        ReflectionTypeLoad,
        BadImageFormat,
        UnauthorizedAccess,
        DependencyMissing,
        ConfigurationError
    }
    
}

