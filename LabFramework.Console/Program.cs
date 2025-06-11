using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LabFramework.Core;
using LabFramework.Loader;
using LabFramework.Commands;
using LabFramework.Core.Logging;

namespace LabFramework.Console
{
    /// <summary>
    /// Main console application for LabFramework
    /// </summary>
    class Program
    {
        private static PluginLoader? _pluginLoader;
        private static ICommandService? _commandService;
        private static bool _running = true;

        static async Task Main(string[] args)
        {
            try
            {
                // Initialize framework
                await InitializeFrameworkAsync();

                // Load plugins
                if (_pluginLoader != null)
                    await _pluginLoader.LoadAllPluginsAsync();

                // Start interactive console
                await RunInteractiveConsoleAsync();
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"Fatal error: {ex.Message}");
                System.Console.ResetColor();
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
            }
        }

        private static async Task InitializeFrameworkAsync()
        {
            // Initialize core framework
            var framework = LabFrameworkCore.Instance;
            await framework.InitializeAsync();

            // Get services
            var logger = framework.Logger;
            var serviceContainer = framework.ServiceContainer;

            // Initialize plugin loader
            _pluginLoader = new PluginLoader(logger, serviceContainer);

            // Initialize command service
            _commandService = serviceContainer.Resolve<ICommandService>();
            _commandService.RegisterCommands(new FrameworkCommands(_pluginLoader));

            logger.LogInformation("LabFramework initialized successfully");
        }

        private static async Task RunInteractiveConsoleAsync()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("LabFramework Interactive Console");
            System.Console.WriteLine("Type 'help' for available commands or 'exit' to quit");
            System.Console.WriteLine();

            while (_running)
            {
                System.Console.Write("LabFramework> ");
                var input = System.Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    await ShutdownAsync();
                    break;
                }

                await ProcessCommandAsync(input);
            }
        }

        private static async Task ProcessCommandAsync(string input)
        {
            try
            {
                if (_commandService == null)
                {
                    System.Console.WriteLine("Command service not initialized");
                    return;
                }

                var args = input.Split( (char)StringSplitOptions.RemoveEmptyEntries);
                var context = new CommandContext("console", "Console", args, input);

                var result = await _commandService.ExecuteCommandAsync(context);

                if (result.Success)
                {
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        System.Console.ForegroundColor = ConsoleColor.Green;
                        System.Console.WriteLine(result.Message);
                        System.Console.ResetColor();
                    }
                }
                else
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine($"Error: {result.Message}");
                    System.Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"Command execution error: {ex.Message}");
                System.Console.ResetColor();
            }
        }

        private static async Task ShutdownAsync()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Shutting down LabFramework...");

            try
            {
                if (_pluginLoader != null)
                    await _pluginLoader.UnloadAllPluginsAsync();
                System.Console.WriteLine("Framework shutdown completed successfully");
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"Error during shutdown: {ex.Message}");
                System.Console.ResetColor();
            }

            _running = false;
        }
    }

    /// <summary>
    /// Built-in framework commands
    /// </summary>
    public class FrameworkCommands
    {
        private readonly PluginLoader _pluginLoader;

        public FrameworkCommands(PluginLoader pluginLoader)
        {
            _pluginLoader = pluginLoader;
        }

        [Command("help", "Show available commands")]
        public CommandResult HelpCommand(CommandContext context)
        {
            var help = @"
LabFramework Console Commands:

Framework Commands:
  help                    - Show this help message
  status                  - Show framework and plugin status
  plugins                 - List all loaded plugins
  reload <plugin>         - Reload a specific plugin
  stats                   - Show detailed statistics
  clear                   - Clear the console screen
  exit/quit               - Exit the framework

Plugin Commands:
  (Plugin-specific commands will appear here when plugins are loaded)

Examples:
  status                  - Show current status
  reload MyPlugin         - Reload MyPlugin
  plugins                 - List all plugins
";
            return CommandResult.Successful(help);
        }

        [Command("status", "Show framework status")]
        public CommandResult StatusCommand(CommandContext context)
        {
            _pluginLoader.ShowStatus();
            return CommandResult.Successful("");
        }

        [Command("plugins", "List all loaded plugins")]
        public CommandResult PluginsCommand(CommandContext context)
        {
            var plugins = _pluginLoader.GetLoadedPlugins();
            
            if (!plugins.Any())
            {
                return CommandResult.Successful("No plugins currently loaded.");
            }

            var result = $"Loaded Plugins ({plugins.Count}):\n\n";
            
            foreach (var plugin in plugins.OrderBy(p => p.Name))
            {
                result += $"• {plugin.Name} v{plugin.Version}\n";
                result += $"  Author: {plugin.Author}\n";
                result += $"  Description: {plugin.Description}\n\n";
            }

            return CommandResult.Successful(result);
        }

        [Command("reload", "Reload a specific plugin")]
        public async Task<CommandResult> ReloadCommand(CommandContext context,
            [CommandParameter("plugin", "Plugin name to reload")] string pluginName)
        {
            var success = await _pluginLoader.ReloadPluginAsync(pluginName);
            
            if (success)
            {
                return CommandResult.Successful($"Plugin '{pluginName}' reloaded successfully");
            }
            else
            {
                return CommandResult.Failed($"Failed to reload plugin '{pluginName}'");
            }
        }

        [Command("stats", "Show detailed statistics")]
        public CommandResult StatsCommand(CommandContext context)
        {
            var stats = _pluginLoader.GetStatistics();
            
            var result = $@"
LabFramework Statistics:

Plugin Statistics:
  Total Plugins: {stats.TotalPlugins}
  Loaded Successfully: {stats.LoadedPlugins}
  Failed to Load: {stats.FailedPlugins}
  Success Rate: {(stats.TotalPlugins > 0 ? (stats.LoadedPlugins * 100.0 / stats.TotalPlugins):100):F1}%

Performance:
  Average Load Time: {stats.AverageLoadTime:F1} ms
  Total Load Time: {stats.TotalLoadTime:F1} ms

System Information:
  Framework Version: 1.0.0
  .NET Version: {Environment.Version}
  OS: {Environment.OSVersion}
  Working Directory: {Environment.CurrentDirectory}
  Uptime: {DateTime.UtcNow - Process.GetCurrentProcess().StartTime:hh\:mm\:ss}
";

            return CommandResult.Successful(result);
        }

        [Command("clear", "Clear the console screen")]
        public CommandResult ClearCommand(CommandContext context)
        {
            System.Console.Clear();
            return CommandResult.Successful("");
        }
    }
}

