# LabFramework

A modern, high-performance plugin framework for SCP: Secret Laboratory that integrates seamlessly with LabAPI.

## Features

- **High Performance**: Optimized event system and minimal overhead
- **Modular Architecture**: Clean separation of concerns with independent modules
- **Developer Friendly**: Intuitive APIs, comprehensive documentation, and helpful debugging tools
- **LabAPI Integration**: Deep integration with the official LabAPI framework
- **Advanced Command System**: Powerful command registration with automatic parameter parsing
- **Dependency Injection**: Built-in DI container for loose coupling and testability
- **Flexible Configuration**: JSON-based configuration with runtime updates
- **Comprehensive Logging**: Multi-level logging with customizable outputs
- **Plugin Management**: Dynamic plugin loading and unloading
- **Event-Driven**: Robust event system for extensibility

## Architecture

LabFramework follows a layered architecture:

### Core Layer (`LabFramework.Core`)
- Event Bus: High-performance async event system
- Logging Service: Flexible logging with multiple outputs
- Configuration Service: JSON-based configuration management
- Dependency Injection: Lightweight DI container
- Plugin System: Base classes and interfaces for plugins

### LabAPI Integration Layer (`LabFramework.LabAPI`)
- Player Wrapper: Simplified player interaction
- Item Wrapper: Enhanced item management
- Game Events: Framework-specific event definitions
- API Abstractions: Clean abstractions over LabAPI

### Feature Modules
- **Commands** (`LabFramework.Commands`): Advanced command system
- **Permissions** (`LabFramework.Permissions`): Hierarchical permission management
- **Custom Items** (`LabFramework.CustomItems`): Custom item creation and management
- **Custom Roles** (`LabFramework.CustomRoles`): Custom role system
- **Updater** (`LabFramework.Updater`): Automatic update system

## Quick Start

### 1. Installation

1. Download the latest release from GitHub
2. Extract to your SCP:SL server directory
3. Ensure LabAPI is installed and working
4. Start your server

### 2. Creating Your First Plugin

```csharp
using System.Threading.Tasks;
using LabFramework.Core;
using LabFramework.Commands;

public class MyPlugin : BasePlugin
{
    public override string Name => "My Plugin";
    public override string Version => "1.0.0";
    public override string Author => "Your Name";
    public override string Description => "My awesome plugin";
    
    public override async Task OnLoadAsync()
    {
        await base.OnLoadAsync();
        
        // Register commands
        var commandService = ServiceContainer.Resolve<ICommandService>();
        commandService.RegisterCommands(this);
        
        Logger.LogInformation("My plugin loaded!");
    }
    
    [Command("hello", "Says hello")]
    public CommandResult HelloCommand(CommandContext context)
    {
        return CommandResult.Successful("Hello from my plugin!");
    }
}
```

### 3. Configuration

Create a `config.json` file in your server directory:

```json
{
    "LogLevel": "Information",
    "EnableAutoUpdates": true,
    "PluginDirectory": "plugins",
    "CustomSettings": {
        "MyPlugin": {
            "EnableFeature": true,
            "MaxUsers": 100
        }
    }
}
```

## Command System

The command system supports:

- **Automatic Parameter Parsing**: Convert string arguments to typed parameters
- **Optional Parameters**: Default values for optional parameters
- **Permission Checking**: Role-based command access
- **Aliases**: Multiple names for the same command
- **Context Restrictions**: Console-only or player-only commands
- **Async Support**: Full async/await support

### Example Commands

```csharp
[Command("teleport", "Teleport to coordinates", permission: "admin.teleport")]
public CommandResult TeleportCommand(CommandContext context,
    [CommandParameter("x", "X coordinate")] float x,
    [CommandParameter("y", "Y coordinate")] float y,
    [CommandParameter("z", "Z coordinate")] float z)
{
    // Implementation here
    return CommandResult.Successful($"Teleported to {x}, {y}, {z}");
}

[Command("give", "Give item to player")]
public async Task<CommandResult> GiveItemCommand(CommandContext context,
    [CommandParameter("player", "Target player")] string playerName,
    [CommandParameter("item", "Item type")] string itemType,
    [CommandParameter("amount", "Amount to give", isOptional: true, defaultValue: 1)] int amount)
{
    // Async implementation here
    await SomeAsyncOperation();
    return CommandResult.Successful($"Gave {amount} {itemType} to {playerName}");
}
```

## Event System

Subscribe to events using the event bus:

```csharp
public override async Task OnLoadAsync()
{
    await base.OnLoadAsync();
    
    // Subscribe to player events
    EventBus.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);
    EventBus.Subscribe<PlayerLeftEvent>(OnPlayerLeft);
}

private async Task OnPlayerJoined(PlayerJoinedEvent eventArgs)
{
    Logger.LogInformation($"Player {eventArgs.PlayerName} joined the server");
    
    // Send welcome message
    var player = GetPlayer(eventArgs.PlayerId);
    player?.SendMessage("Welcome to the server!");
}
```

## Building from Source

### Prerequisites

- .NET 8.0 SDK or later
- LabAPI reference (for integration)

### Build Steps

```bash
# Clone the repository
git clone https://github.com/CrazyLab/LabFramework.git
cd LabFramework

# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Run tests (if available)
dotnet test
```

## Project Structure

```
LabFramework/
â”œâ”€â”€ src/
â”‚   â”œâ”€â”€ LabFramework.Core/           # Core framework components
â”‚   â”œâ”€â”€ LabFramework.LabAPI/         # LabAPI integration layer
â”‚   â”œâ”€â”€ LabFramework.Commands/       # Command system
â”‚   â”œâ”€â”€ LabFramework.Permissions/    # Permission management
â”‚   â”œâ”€â”€ LabFramework.CustomItems/    # Custom item system
â”‚   â”œâ”€â”€ LabFramework.CustomRoles/    # Custom role system
â”‚   â”œâ”€â”€ LabFramework.Updater/        # Auto-update system
â”‚   â””â”€â”€ LabFramework.PluginAPI/      # Plugin development API
â”œâ”€â”€ examples/                        # Example plugins
â”œâ”€â”€ tests/                          # Unit tests
â”œâ”€â”€ docs/                           # Documentation
â””â”€â”€ LabFramework.sln                # Solution file
```

## Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Development Setup

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- **Documentation**: [Wiki](https://github.com/CrazyLab/LabFramework/wiki)
- **Issues**: [GitHub Issues](https://github.com/CrazyLab/LabFramework/issues)
- **Discord**: [Join our Discord](https://discord.gg/mBEvmSKJPc)

## Comparison with EXILED

| Feature | LabFramework | EXILED |
|---------|-------------|---------|
| Performance | â­â­â­â­â­ | â­â­â­ |
| LabAPI Integration | â­â­â­â­â­ | â­â­ |
| Command System | â­â­â­â­â­ | â­â­â­ |
| Documentation | â­â­â­â­â­ | â­â­â­ |
| Developer Experience | â­â­â­â­â­ | â­â­â­ |
| Modularity | â­â­â­â­â­ | â­â­â­â­ |
| Testing Support | â­â­â­â­â­ | â­â­ |

## Roadmap

- [ ] Complete LabAPI integration
- [ ] Advanced permission system
- [ ] Custom item editor
- [ ] Web-based administration panel
- [ ] Plugin marketplace
- [ ] Performance monitoring tools
- [ ] Multi-language support

---

**LabFramework** - The next generation SCP:SL plugin framework
# --------------------------------------------------------------------------------------------------
# LabFramework API Documentation

## Table of Contents

1. [Core Components](#core-components)
2. [Event System](#event-system)
3. [Command System](#command-system)
4. [Configuration System](#configuration-system)
5. [Logging System](#logging-system)
6. [Dependency Injection](#dependency-injection)
7. [LabAPI Integration](#labapi-integration)
8. [Plugin Development](#plugin-development)

## Core Components

### LabFrameworkCore

The main framework class that coordinates all services.

```csharp
public class LabFrameworkCore
{
    public static LabFrameworkCore Instance { get; }
    public IServiceContainer ServiceContainer { get; }
    public IEventBus EventBus { get; }
    public ILoggingService Logger { get; }
    public IConfigurationService Configuration { get; }
    
    public Task InitializeAsync();
    public Task ShutdownAsync();
    public Task RegisterPluginAsync(IPlugin plugin);
    public Task UnregisterPluginAsync(IPlugin plugin);
}
```

### IPlugin Interface

Base interface for all plugins.

```csharp
public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    string Author { get; }
    string Description { get; }
    
    Task OnLoadAsync();
    Task OnUnloadAsync();
}
```

### BasePlugin Class

Base implementation of IPlugin with common functionality.

```csharp
public abstract class BasePlugin : IPlugin
{
    protected IEventBus EventBus { get; }
    protected ILoggingService Logger { get; }
    protected IConfigurationService Configuration { get; }
    protected IServiceContainer ServiceContainer { get; }
    
    public virtual Task OnLoadAsync();
    public virtual Task OnUnloadAsync();
}
```

## Event System

### IEvent Interface

Base interface for all events.

```csharp
public interface IEvent
{
    string EventId { get; }
    DateTime Timestamp { get; }
    bool IsCancellable { get; }
    bool IsCancelled { get; set; }
}
```

### BaseEvent Class

Base implementation of IEvent.

```csharp
public abstract class BaseEvent : IEvent
{
    public string EventId { get; }
    public DateTime Timestamp { get; }
    public virtual bool IsCancellable => false;
    public bool IsCancelled { get; set; }
}
```

### IEventBus Interface

Event bus for publishing and subscribing to events.

```csharp
public interface IEventBus
{
    void Subscribe<T>(EventHandler<T> handler) where T : IEvent;
    void Unsubscribe<T>(EventHandler<T> handler) where T : IEvent;
    Task PublishAsync<T>(T eventArgs) where T : IEvent;
    void Publish<T>(T eventArgs) where T : IEvent;
}
```

### Event Handler Delegate

```csharp
public delegate Task EventHandler<in T>(T eventArgs) where T : IEvent;
```

### Example Usage

```csharp
// Subscribe to an event
EventBus.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);

// Event handler
private async Task OnPlayerJoined(PlayerJoinedEvent eventArgs)
{
    Logger.LogInformation($"Player {eventArgs.PlayerName} joined");
}

// Publish an event
await EventBus.PublishAsync(new PlayerJoinedEvent(playerId, playerName));
```

## Command System

### CommandAttribute

Marks a method as a command.

```csharp
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string name, string description = null, 
        string permission = null, bool consoleOnly = false, 
        bool playerOnly = false, params string[] aliases);
}
```

### CommandParameterAttribute

Defines command parameters.

```csharp
[AttributeUsage(AttributeTargets.Parameter)]
public class CommandParameterAttribute : Attribute
{
    public CommandParameterAttribute(string name, string description = null, 
        bool isOptional = false, object defaultValue = null);
}
```

### CommandContext

Provides context for command execution.

```csharp
public class CommandContext
{
    public string SenderId { get; set; }
    public string SenderName { get; set; }
    public string[] Arguments { get; set; }
    public string RawInput { get; set; }
    public bool IsConsole { get; set; }
    public object Sender { get; set; }
}
```

### CommandResult

Represents the result of command execution.

```csharp
public class CommandResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public Exception Exception { get; set; }
    
    public static CommandResult Successful(string message = null);
    public static CommandResult Failed(string message, Exception exception = null);
}
```

### ICommandService Interface

Service for managing commands.

```csharp
public interface ICommandService
{
    void RegisterCommands(object commandHandler);
    void UnregisterCommands(object commandHandler);
    Task<CommandResult> ExecuteCommandAsync(CommandContext context);
    IEnumerable<CommandInfo> GetCommands();
    CommandInfo GetCommand(string name);
}
```

### Example Command

```csharp
[Command("teleport", "Teleport to coordinates", permission: "admin.teleport")]
public CommandResult TeleportCommand(CommandContext context,
    [CommandParameter("x", "X coordinate")] float x,
    [CommandParameter("y", "Y coordinate")] float y,
    [CommandParameter("z", "Z coordinate")] float z)
{
    // Implementation
    return CommandResult.Successful($"Teleported to {x}, {y}, {z}");
}
```

## Configuration System

### IConfigurationService Interface

Service for managing configuration.

```csharp
public interface IConfigurationService
{
    T GetValue<T>(string key, T defaultValue = default);
    void SetValue<T>(string key, T value);
    Task LoadFromFileAsync(string filePath);
    Task SaveToFileAsync(string filePath);
    bool HasKey(string key);
    void RemoveKey(string key);
}
```

### Example Usage

```csharp
// Get configuration value
var maxUsers = Configuration.GetValue<int>("MaxUsers", 100);

// Set configuration value
Configuration.SetValue("MaxUsers", 150);

// Load from file
await Configuration.LoadFromFileAsync("config.json");

// Save to file
await Configuration.SaveToFileAsync("config.json");
```

## Logging System

### LogLevel Enumeration

```csharp
public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Critical = 5,
    None = 6
}
```

### ILoggingService Interface

Service for logging messages.

```csharp
public interface ILoggingService
{
    void Log(LogLevel level, string message, Exception exception = null);
    void LogTrace(string message);
    void LogDebug(string message);
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception exception = null);
    void LogCritical(string message, Exception exception = null);
}
```

### Example Usage

```csharp
Logger.LogInformation("Plugin loaded successfully");
Logger.LogWarning("Configuration value not found, using default");
Logger.LogError("Failed to connect to database", exception);
```

## Dependency Injection

### ServiceLifetime Enumeration

```csharp
public enum ServiceLifetime
{
    Transient,  // New instance every time
    Singleton,  // Single instance
    Scoped      // Single instance per scope
}
```

### IServiceContainer Interface

Container for dependency injection.

```csharp
public interface IServiceContainer
{
    void Register<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TImplementation : class, TService;
    void Register<TService>(Func<IServiceProvider, TService> factory, 
        ServiceLifetime lifetime = ServiceLifetime.Transient);
    void RegisterSingleton<TService>(TService instance);
    TService Resolve<TService>();
    object Resolve(Type serviceType);
    bool TryResolve<TService>(out TService service);
}
```

### Example Usage

```csharp
// Register services
ServiceContainer.Register<IMyService, MyService>(ServiceLifetime.Singleton);
ServiceContainer.Register<IDatabase>(provider => new Database("connection"), 
    ServiceLifetime.Singleton);

// Resolve services
var myService = ServiceContainer.Resolve<IMyService>();
```

## LabAPI Integration

### PlayerWrapper Class

Simplified wrapper around LabAPI player functionality.

```csharp
public class PlayerWrapper
{
    public string Id { get; }
    public string Name { get; }
    public string Role { get; }
    public int Health { get; }
    public bool IsAlive { get; }
    public Vector3 Position { get; }
    public List<ItemWrapper> Inventory { get; }
    
    public void SendMessage(string message, int duration = 5);
    public void Teleport(Vector3 position);
    public void SetHealth(int health);
    public void GiveItem(string itemType, int amount = 1);
    public void RemoveItem(string itemType, int amount = 1);
    public void SetRole(string role);
    public void Kill(string reason = "Unknown");
    public void Kick(string reason = "Kicked by administrator");
    public void Ban(int duration, string reason = "Banned by administrator");
}
```

### ItemWrapper Class

Simplified wrapper around LabAPI item functionality.

```csharp
public class ItemWrapper
{
    public string Id { get; }
    public string Type { get; }
    public string Name { get; }
    public int Durability { get; }
    public Dictionary<string, object> Properties { get; }
    
    public void SetProperty(string key, object value);
    public T GetProperty<T>(string key, T defaultValue = default);
}
```

### Game Events

Pre-defined events for common game occurrences.

```csharp
public class PlayerJoinedEvent : BaseEvent
{
    public string PlayerId { get; }
    public string PlayerName { get; }
    public DateTime JoinTime { get; }
}

public class PlayerLeftEvent : BaseEvent
{
    public string PlayerId { get; }
    public string PlayerName { get; }
    public DateTime LeaveTime { get; }
}

public class RoundStartedEvent : BaseEvent
{
    public int RoundNumber { get; }
    public int PlayerCount { get; }
}

public class RoundEndedEvent : BaseEvent
{
    public int RoundNumber { get; }
    public string WinningTeam { get; }
    public TimeSpan RoundDuration { get; }
}
```

## Plugin Development

### Creating a Plugin

1. Create a class that inherits from `BasePlugin`
2. Implement required properties
3. Override `OnLoadAsync` and `OnUnloadAsync` methods
4. Add command methods with `[Command]` attribute

```csharp
public class MyPlugin : BasePlugin
{
    public override string Name => "My Plugin";
    public override string Version => "1.0.0";
    public override string Author => "Your Name";
    public override string Description => "My awesome plugin";
    
    public override async Task OnLoadAsync()
    {
        await base.OnLoadAsync();
        
        // Register commands
        var commandService = ServiceContainer.Resolve<ICommandService>();
        commandService.RegisterCommands(this);
        
        // Subscribe to events
        EventBus.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);
        
        Logger.LogInformation("My plugin loaded!");
    }
    
    public override async Task OnUnloadAsync()
    {
        // Cleanup
        var commandService = ServiceContainer.Resolve<ICommandService>();
        commandService.UnregisterCommands(this);
        
        await base.OnUnloadAsync();
    }
    
    private async Task OnPlayerJoined(PlayerJoinedEvent eventArgs)
    {
        Logger.LogInformation($"Player {eventArgs.PlayerName} joined");
    }
    
    [Command("hello", "Says hello")]
    public CommandResult HelloCommand(CommandContext context)
    {
        return CommandResult.Successful("Hello from my plugin!");
    }
}
```

### Best Practices

1. **Error Handling**: Always wrap potentially failing code in try-catch blocks
2. **Resource Cleanup**: Properly dispose of resources in `OnUnloadAsync`
3. **Configuration**: Use the configuration service for plugin settings
4. **Logging**: Use appropriate log levels for different types of messages
5. **Performance**: Avoid blocking operations in event handlers
6. **Testing**: Write unit tests for your plugin logic

### Plugin Structure

```
Plugin/
 MyPlugin.cs              # Main plugin class
 Commands/                 # Command classes
 AdminCommands.cs
 PlayerCommands.cs
â”œâ”€â”€ Events/                   # Event handlers
 PlayerEvents.cs
RoundEvents.cs
Services/                 # Plugin services
DatabaseService.cs
ApiService.cs
â”œâ”€â”€ Models/                   # Data models
 PlayerData.cs
 Config/                   # Configuration classes
PluginConfig.cs
 README.md                 # Plugin documentation
```

This documentation provides a comprehensive overview of the LabFramework API. For more detailed examples and tutorials, please refer to the example plugins and the framework's GitHub repository.
# --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
# LabFramework Developer Guide

## Table of Contents

1. [Introduction](#introduction)
2. [Getting Started](#getting-started)
3. [Core Concepts](#core-concepts)
4. [Plugin Development](#plugin-development)
5. [Command System](#command-system)
6. [Permission System](#permission-system)
7. [Custom Items](#custom-items)
8. [Event System](#event-system)
9. [Configuration Management](#configuration-management)
10. [Best Practices](#best-practices)
11. [Troubleshooting](#troubleshooting)
12. [Advanced Topics](#advanced-topics)

## Introduction

LabFramework is a next-generation plugin framework designed specifically for SCP: Secret Laboratory servers. Built from the ground up with performance, modularity, and developer experience in mind, LabFramework provides a comprehensive platform for creating powerful server modifications while maintaining seamless integration with the official LabAPI.

### Why LabFramework?

Traditional plugin frameworks often suffer from performance bottlenecks, complex APIs, and limited extensibility. LabFramework addresses these challenges by implementing modern software engineering principles and leveraging the latest .NET technologies to deliver a framework that is both powerful and easy to use.

The framework's architecture is built around several core principles that set it apart from existing solutions. First, performance is paramount - every component has been optimized for high-throughput scenarios with minimal memory allocation and efficient resource utilization. Second, modularity ensures that developers can use only the components they need, reducing complexity and improving maintainability. Third, the developer experience has been carefully crafted to minimize boilerplate code while providing powerful abstractions that make complex tasks simple.

LabFramework's integration with LabAPI goes beyond simple wrapper functions. The framework provides intelligent abstractions that simplify common operations while still allowing direct access to the underlying LabAPI when needed. This approach ensures that developers can leverage the full power of LabAPI without being constrained by framework limitations.

### Key Features

The framework includes a comprehensive set of features designed to address the most common needs of SCP:SL server administrators and plugin developers. The event system provides high-performance asynchronous event handling with support for cancellable events and priority-based execution. The command system offers automatic parameter parsing, permission integration, and support for both synchronous and asynchronous command execution.

Permission management is handled through a hierarchical system that supports groups, inheritance, and fine-grained access control. The custom items system allows developers to create complex interactive items with custom behaviors, while the configuration system provides runtime updates and type-safe access to settings.

All of these features are built on top of a lightweight dependency injection container that promotes loose coupling and testability. The framework also includes comprehensive logging capabilities with multiple output targets and configurable log levels.

## Getting Started

### Prerequisites

Before you begin developing with LabFramework, ensure that your development environment meets the following requirements:

- .NET 4.8 or later installed on your development machine
- A SCP: Secret Laboratory server with LabAPI installed and configured
- Basic familiarity with C# programming and asynchronous programming concepts
- Understanding of SCP:SL game mechanics and server administration

### Installation

Installing LabFramework on your server is straightforward and can be accomplished through several methods. The recommended approach is to use the provided installation script, which automates the entire process and ensures proper configuration.

To install LabFramework using the installation script, download the latest release from the GitHub repository and extract it to a temporary directory. Open a terminal or command prompt and navigate to the extracted directory. Run the installation script with appropriate permissions:

```bash
chmod +x install.sh
./install.sh
```

The installation script will prompt you for your server directory and automatically detect your LabAPI installation. It will create the necessary directory structure, copy framework files, and configure the startup scripts.

For manual installation, create a `LabFramework` directory in your server root and copy the framework DLL files to this location. Create subdirectories for `plugins`, `configs`, and `logs`. Update your server startup script to include the LabFramework initialization code.

### Project Setup

Creating a new LabFramework plugin project requires setting up the proper project structure and dependencies. Start by creating a new .NET class library project using the dotnet CLI or your preferred IDE:

```bash
dotnet new classlib -n MyAwesomePlugin
cd MyAwesomePlugin
```

Add references to the LabFramework components you'll be using. At minimum, most plugins will need the Core and Commands packages:

```bash
dotnet add reference path/to/LabFramework.Core.dll
dotnet add reference path/to/LabFramework.Commands.dll
```

Configure your project file to target .NET 8.0 and enable nullable reference types for better code safety:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>
</Project>
```

### Your First Plugin

Creating your first plugin is an excellent way to understand the framework's basic concepts and development workflow. A simple plugin demonstrates the essential patterns you'll use in more complex projects.

Start by creating a class that inherits from `BasePlugin`. This base class provides access to all framework services and handles the plugin lifecycle:

```csharp
using LabFramework.Core;
using LabFramework.Commands;

public class MyFirstPlugin : BasePlugin
{
    public override string Name => "My First Plugin";
    public override string Version => "1.0.0";
    public override string Author => "Your Name";
    public override string Description => "A simple example plugin";
    
    public override async Task OnLoadAsync()
    {
        await base.OnLoadAsync();
        
        // Register commands
        var commandService = ServiceContainer.Resolve<ICommandService>();
        commandService.RegisterCommands(this);
        
        Logger.LogInformation("My first plugin has been loaded!");
    }
    
    public override async Task OnUnloadAsync()
    {
        // Clean up resources
        var commandService = ServiceContainer.Resolve<ICommandService>();
        commandService.UnregisterCommands(this);
        
        await base.OnUnloadAsync();
    }
    
    [Command("hello", "Says hello to the user")]
    public CommandResult HelloCommand(CommandContext context)
    {
        return CommandResult.Successful($"Hello, {context.SenderName}!");
    }
}
```

This example demonstrates several important concepts. The plugin class inherits from `BasePlugin` and implements the required properties and methods. The `OnLoadAsync` method is called when the plugin is loaded and is where you should initialize your plugin's functionality. The `OnUnloadAsync` method is called when the plugin is unloaded and should clean up any resources.

The `[Command]` attribute marks methods as commands that can be executed by players or administrators. The command system automatically handles parameter parsing and validation, making it easy to create powerful administrative tools.

## Core Concepts

Understanding LabFramework's core concepts is essential for effective plugin development. These concepts form the foundation upon which all framework functionality is built and provide the architectural patterns that ensure consistency and maintainability across the entire ecosystem.

### Service Container and Dependency Injection

LabFramework uses dependency injection as a core architectural pattern to promote loose coupling and testability. The service container manages the lifecycle of framework services and provides a centralized location for service registration and resolution.

The dependency injection pattern allows components to declare their dependencies through constructor parameters or service resolution, rather than creating dependencies directly. This approach makes code more modular, testable, and maintainable by reducing tight coupling between components.

Services in LabFramework are registered with specific lifetimes that control how instances are created and managed. Singleton services are created once and reused throughout the application lifetime, making them suitable for stateful services like configuration and logging. Transient services are created each time they're requested, making them suitable for stateless operations. Scoped services are created once per scope, which is useful for request-specific operations.

To register a service, use the service container's registration methods:

```csharp
ServiceContainer.Register<IMyService, MyService>(ServiceLifetime.Singleton);
ServiceContainer.RegisterSingleton<IDatabase>(new DatabaseConnection("connection-string"));
```

To resolve services, use the container's resolution methods:

```csharp
var myService = ServiceContainer.Resolve<IMyService>();
var database = ServiceContainer.Resolve<IDatabase>();
```

### Plugin Lifecycle

Every plugin in LabFramework follows a well-defined lifecycle that ensures proper initialization and cleanup. Understanding this lifecycle is crucial for developing robust plugins that integrate seamlessly with the framework.

The plugin lifecycle begins when the framework discovers and loads the plugin assembly. The framework creates an instance of the plugin class and calls the `OnLoadAsync` method. This method should perform all necessary initialization, including service registration, event subscription, and resource allocation.

During the plugin's active lifetime, it can respond to events, handle commands, and interact with other framework components. The plugin should maintain its state and provide its functionality to users and other plugins.

When the plugin is unloaded, either due to server shutdown or manual unloading, the framework calls the `OnUnloadAsync` method. This method should clean up all resources, unregister services, and unsubscribe from events to prevent memory leaks and ensure clean shutdown.

The asynchronous nature of the lifecycle methods allows plugins to perform complex initialization and cleanup operations without blocking the server. This is particularly important for plugins that need to establish database connections, load large configuration files, or perform network operations during startup.

### Configuration Management

LabFramework provides a powerful configuration system that supports runtime updates, type safety, and hierarchical configuration structures. The configuration system is designed to handle both framework-level settings and plugin-specific configuration with equal ease.

Configuration values are stored in a hierarchical key-value structure that supports nested objects and arrays. The system uses JSON as the primary serialization format, providing human-readable configuration files that are easy to edit and version control.

Type safety is ensured through generic methods that automatically handle type conversion and validation. The configuration system can convert between compatible types and provides meaningful error messages when conversion fails.

Runtime updates allow configuration changes to take effect without restarting the server. Plugins can subscribe to configuration change events to respond immediately to updated settings:

```csharp
Configuration.SetValue("MyPlugin.MaxUsers", 150);
var maxUsers = Configuration.GetValue<int>("MyPlugin.MaxUsers", 100);

// Subscribe to configuration changes
Configuration.OnValueChanged += (key, oldValue, newValue) =>
{
    if (key.StartsWith("MyPlugin."))
    {
        Logger.LogInformation($"Configuration updated: {key} = {newValue}");
    }
};
```

### Error Handling and Logging

Robust error handling and comprehensive logging are essential for maintaining stable and debuggable server environments. LabFramework provides sophisticated logging capabilities that support multiple output targets, configurable log levels, and structured logging.

The logging system supports six log levels: Trace, Debug, Information, Warning, Error, and Critical. Each level serves a specific purpose in the debugging and monitoring workflow. Trace and Debug levels are used for detailed diagnostic information during development. Information level provides general operational messages. Warning level indicates potential issues that don't prevent operation. Error level indicates failures that affect functionality. Critical level indicates severe failures that may cause system instability.

Structured logging allows log messages to include additional context information that can be used for filtering, searching, and analysis. The logging system automatically includes timestamp, log level, and source information with each message.

Exception handling should follow established patterns to ensure consistent error reporting and recovery. Catch specific exception types when possible and provide meaningful error messages that help with troubleshooting:

```csharp
try
{
    await SomeRiskyOperation();
}
catch (SpecificException ex)
{
    Logger.LogError($"Specific error occurred: {ex.Message}", ex);
    // Handle specific error case
}
catch (Exception ex)
{
    Logger.LogError($"Unexpected error: {ex.Message}", ex);
    // Handle general error case
}
```

## Plugin Development

Developing plugins for LabFramework involves understanding the framework's architecture, following established patterns, and leveraging the provided tools and services. This section provides comprehensive guidance for creating robust, maintainable, and performant plugins.

### Plugin Architecture

A well-designed plugin follows a clear architectural pattern that separates concerns and promotes maintainability. The recommended architecture consists of several layers, each with specific responsibilities and clear interfaces.

The presentation layer handles user interaction through commands, events, and user interfaces. This layer should be thin and focused on translating user input into business logic calls. Commands should validate input and delegate actual work to service classes.

The business logic layer contains the core functionality of your plugin. This layer should be independent of the presentation layer and focus on implementing the plugin's features. Business logic should be organized into service classes that can be easily tested and reused.

The data access layer handles persistence and external system integration. This layer should abstract the details of data storage and provide clean interfaces for the business logic layer. Whether you're using files, databases, or web APIs, the data access layer should hide these implementation details.

The configuration layer manages plugin settings and provides type-safe access to configuration values. This layer should handle configuration validation and provide sensible defaults for all settings.

### Service Registration

Plugins often need to register their own services with the dependency injection container to provide functionality to other plugins or framework components. Service registration should be performed during plugin initialization and follow established patterns for service lifetime management.

When registering services, consider the appropriate lifetime for each service. Singleton services are suitable for stateful services that maintain data across requests. Transient services are appropriate for stateless operations that don't maintain state. Scoped services are useful for request-specific operations that need to maintain state within a single operation.

Service interfaces should be designed to be stable and extensible. Avoid exposing implementation details through interfaces and prefer composition over inheritance when designing service hierarchies:

```csharp
public interface IPlayerDataService
{
    Task<PlayerData> GetPlayerDataAsync(string playerId);
    Task SavePlayerDataAsync(PlayerData data);
    Task<IEnumerable<PlayerData>> GetTopPlayersAsync(int count);
}

public class DatabasePlayerDataService : IPlayerDataService
{
    private readonly IDatabase _database;
    private readonly ILoggingService _logger;
    
    public DatabasePlayerDataService(IDatabase database, ILoggingService logger)
    {
        _database = database;
        _logger = logger;
    }
    
    // Implementation details...
}

// Register in plugin OnLoadAsync
ServiceContainer.Register<IPlayerDataService, DatabasePlayerDataService>(ServiceLifetime.Singleton);
```

### Event Handling

Events are the primary mechanism for responding to game state changes and implementing reactive functionality. LabFramework's event system is designed for high performance and supports both synchronous and asynchronous event handlers.

Event handlers should be lightweight and avoid blocking operations when possible. Long-running operations should be offloaded to background tasks or queues to prevent impacting server performance. Event handlers should also be defensive and handle exceptions gracefully to prevent one plugin from affecting others.

Event subscription should be performed during plugin initialization, and unsubscription should be performed during plugin cleanup to prevent memory leaks:

```csharp
public override async Task OnLoadAsync()
{
    await base.OnLoadAsync();
    
    EventBus.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);
    EventBus.Subscribe<PlayerLeftEvent>(OnPlayerLeft);
}

public override async Task OnUnloadAsync()
{
    EventBus.Unsubscribe<PlayerJoinedEvent>(OnPlayerJoined);
    EventBus.Unsubscribe<PlayerLeftEvent>(OnPlayerLeft);
    
    await base.OnUnloadAsync();
}

private async Task OnPlayerJoined(PlayerJoinedEvent eventArgs)
{
    try
    {
        var playerData = await _playerDataService.GetPlayerDataAsync(eventArgs.PlayerId);
        if (playerData == null)
        {
            playerData = new PlayerData { PlayerId = eventArgs.PlayerId };
            await _playerDataService.SavePlayerDataAsync(playerData);
        }
        
        Logger.LogInformation($"Player {eventArgs.PlayerName} joined with {playerData.PlayTime} total play time");
    }
    catch (Exception ex)
    {
        Logger.LogError($"Error handling player join event: {ex.Message}", ex);
    }
}
```

### Resource Management

Proper resource management is crucial for maintaining server stability and performance. Plugins should carefully manage memory usage, file handles, network connections, and other system resources to prevent resource leaks and ensure clean shutdown.

Implement the `IDisposable` pattern for classes that manage unmanaged resources or need explicit cleanup. Use `using` statements or `using` declarations to ensure resources are properly disposed of even when exceptions occur.

For long-running operations, consider implementing cancellation support using `CancellationToken` to allow graceful shutdown when the plugin is unloaded:

```csharp
public class BackgroundTaskService : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Task _backgroundTask;
    
    public BackgroundTaskService()
    {
        _backgroundTask = RunBackgroundTaskAsync(_cancellationTokenSource.Token);
    }
    
    private async Task RunBackgroundTaskAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Background task error: {ex.Message}", ex);
            }
        }
    }
    
    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _backgroundTask.Wait(TimeSpan.FromSeconds(5));
        _cancellationTokenSource.Dispose();
    }
}
```

## Command System

The command system is one of LabFramework's most powerful features, providing a sophisticated framework for creating administrative tools and player interactions. The system handles parameter parsing, validation, permission checking, and execution in a unified and extensible manner.

### Command Definition

Commands are defined using the `[Command]` attribute on methods within classes that are registered with the command service. The attribute accepts several parameters that control the command's behavior and accessibility.

The command name is the primary identifier used to invoke the command. Names should be descriptive and follow consistent naming conventions. The description provides help text that is displayed to users when they request command information.

Permission strings control who can execute the command. The permission system supports hierarchical permissions and group-based access control. Commands without permission requirements can be executed by anyone.

Aliases provide alternative names for commands, allowing users to use shorter or more familiar names. Aliases are particularly useful for frequently used commands or when migrating from other systems.

Console-only and player-only restrictions control the execution context for commands. Console-only commands can only be executed from the server console, making them suitable for administrative operations. Player-only commands can only be executed by connected players, making them suitable for player-specific operations:

```csharp
[Command("teleport", "Teleport a player to coordinates", 
         permission: "admin.teleport", aliases: new[] { "tp", "goto" })]
public async Task<CommandResult> TeleportCommand(CommandContext context,
    [CommandParameter("player", "Target player name")] string playerName,
    [CommandParameter("x", "X coordinate")] float x,
    [CommandParameter("y", "Y coordinate")] float y,
    [CommandParameter("z", "Z coordinate")] float z)
{
    var player = await FindPlayerAsync(playerName);
    if (player == null)
    {
        return CommandResult.Failed($"Player '{playerName}' not found");
    }
    
    player.Teleport(new Vector3(x, y, z));
    return CommandResult.Successful($"Teleported {player.Name} to ({x}, {y}, {z})");
}
```

### Parameter Handling

The command system provides automatic parameter parsing and type conversion, eliminating the need for manual string parsing and validation. Parameters are defined using the `[CommandParameter]` attribute, which provides metadata about the parameter's purpose and constraints.

Required parameters must be provided by the user and will cause command execution to fail if missing. Optional parameters have default values and can be omitted by the user. The default value is used when the parameter is not provided.

Type conversion is handled automatically for common types including strings, numbers, booleans, and enums. Custom type converters can be registered for complex types or domain-specific objects.

Parameter validation can be implemented through custom attributes or by checking parameter values within the command method. Validation should provide clear error messages that help users understand what went wrong:

```csharp
[Command("ban", "Ban a player from the server")]
public async Task<CommandResult> BanCommand(CommandContext context,
    [CommandParameter("player", "Player to ban")] string playerName,
    [CommandParameter("duration", "Ban duration in minutes", isOptional: true, defaultValue: 60)] int duration,
    [CommandParameter("reason", "Ban reason", isOptional: true, defaultValue: "No reason provided")] string reason)
{
    if (duration <= 0)
    {
        return CommandResult.Failed("Duration must be greater than 0");
    }
    
    if (duration > 43200) // 30 days
    {
        return CommandResult.Failed("Duration cannot exceed 30 days");
    }
    
    var player = await FindPlayerAsync(playerName);
    if (player == null)
    {
        return CommandResult.Failed($"Player '{playerName}' not found");
    }
    
    player.Ban(duration, reason);
    return CommandResult.Successful($"Banned {player.Name} for {duration} minutes. Reason: {reason}");
}
```

### Asynchronous Commands

LabFramework fully supports asynchronous command execution, allowing commands to perform long-running operations without blocking the server. Asynchronous commands should return `Task<CommandResult>` instead of `CommandResult`.

Asynchronous operations are particularly useful for commands that need to access databases, make web requests, or perform file I/O operations. The command system handles the asynchronous execution transparently and ensures that exceptions are properly caught and reported.

When implementing asynchronous commands, consider providing progress feedback for long-running operations and implement cancellation support where appropriate:

```csharp
[Command("backup", "Create a server backup", consoleOnly: true)]
public async Task<CommandResult> BackupCommand(CommandContext context,
    [CommandParameter("name", "Backup name", isOptional: true)] string backupName = null)
{
    backupName ??= $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
    
    try
    {
        Logger.LogInformation($"Starting backup: {backupName}");
        
        var backupService = ServiceContainer.Resolve<IBackupService>();
        var progress = new Progress<BackupProgress>(p => 
        {
            Logger.LogInformation($"Backup progress: {p.Percentage}% - {p.CurrentFile}");
        });
        
        await backupService.CreateBackupAsync(backupName, progress);
        
        return CommandResult.Successful($"Backup '{backupName}' created successfully");
    }
    catch (Exception ex)
    {
        Logger.LogError($"Backup failed: {ex.Message}", ex);
        return CommandResult.Failed($"Backup failed: {ex.Message}");
    }
}
```

### Command Context

The `CommandContext` provides information about the command execution environment, including the sender, arguments, and execution context. This information is essential for implementing context-aware command behavior.

The sender information includes the user ID, display name, and whether the command was executed from the console. This information can be used to customize command behavior based on who is executing the command.

The arguments array contains the raw command arguments as provided by the user. While the parameter system handles most parsing needs, the raw arguments can be useful for implementing variable-length parameter lists or custom parsing logic.

The execution context indicates whether the command was executed from the console or by a player. This information is automatically used by the framework to enforce console-only and player-only restrictions, but can also be used by command implementations to provide different behavior in different contexts:

```csharp
[Command("status", "Show server status")]
public CommandResult StatusCommand(CommandContext context)
{
    var serverInfo = GetServerInfo();
    
    if (context.IsConsole)
    {
        // Provide detailed information for console users
        return CommandResult.Successful($@"
Server Status:
  Players: {serverInfo.PlayerCount}/{serverInfo.MaxPlayers}
  Uptime: {serverInfo.Uptime}
  Memory: {serverInfo.MemoryUsage:F1} MB
  CPU: {serverInfo.CpuUsage:F1}%
  Round: {serverInfo.RoundNumber} ({serverInfo.RoundTime})
");
    }
    else
    {
        // Provide basic information for players
        return CommandResult.Successful($"Players: {serverInfo.PlayerCount}/{serverInfo.MaxPlayers} | Round: {serverInfo.RoundNumber}");
    }
}
```
#-------------------------------------------------------------------------------------------------------------------------------------------------------
# LabFramework Quick Start Guide

## Introduction

Welcome to LabFramework! This guide will help you get up and running with the framework in just a few minutes. By the end of this guide, you'll have a working plugin that demonstrates the core features of LabFramework.

## Prerequisites

- .NET 4.8 SDK installed
- SCP: Secret Laboratory server with LabAPI
- Basic C# knowledge

## Step 1: Installation

1. Download the latest LabFramework release
2. Extract to your server directory
3. Run the installation script:

```bash
chmod +x install.sh
./install.sh
```

## Step 2: Create Your First Plugin

Create a new directory for your plugin:

```bash
mkdir MyFirstPlugin
cd MyFirstPlugin
dotnet new classlib
```

Add LabFramework references:

```bash
dotnet add reference ../LabFramework/LabFramework.Core.dll
dotnet add reference ../LabFramework/LabFramework.Commands.dll
```

## Step 3: Write the Plugin Code

Replace the contents of `Class1.cs` with:

```csharp
using LabFramework.Core;
using LabFramework.Commands;
using LabFramework.Core.Events;

namespace MyFirstPlugin
{
    public class Plugin : BasePlugin
    {
        public override string Name => "My First Plugin";
        public override string Version => "1.0.0";
        public override string Author => "Your Name";
        public override string Description => "My first LabFramework plugin";

        public override async Task OnLoadAsync()
        {
            await base.OnLoadAsync();
            
            // Register commands
            var commandService = ServiceContainer.Resolve<ICommandService>();
            commandService.RegisterCommands(this);
            
            // Subscribe to events
            EventBus.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);
            
            Logger.LogInformation("My First Plugin loaded successfully!");
        }

        public override async Task OnUnloadAsync()
        {
            // Clean up
            var commandService = ServiceContainer.Resolve<ICommandService>();
            commandService.UnregisterCommands(this);
            
            EventBus.Unsubscribe<PlayerJoinedEvent>(OnPlayerJoined);
            
            await base.OnUnloadAsync();
        }

        [Command("hello", "Say hello to a player")]
        public CommandResult HelloCommand(CommandContext context,
            [CommandParameter("name", "Player name")] string name)
        {
            return CommandResult.Successful($"Hello, {name}! Welcome to the server!");
        }

        [Command("time", "Show current server time")]
        public CommandResult TimeCommand(CommandContext context)
        {
            return CommandResult.Successful($"Current server time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        private async Task OnPlayerJoined(PlayerJoinedEvent eventArgs)
        {
            Logger.LogInformation($"Player {eventArgs.PlayerName} joined the server!");
            
            // Send welcome message
            // eventArgs.Player.SendMessage("Welcome to our server!");
        }
    }
}
```

## Step 4: Build and Deploy

Build your plugin:

```bash
dotnet build --configuration Release
```

Copy the built DLL to your server's plugins directory:

```bash
cp bin/Release/net8.0/MyFirstPlugin.dll ../LabFramework/plugins/
```

## Step 5: Test Your Plugin

1. Start your server
2. Check the logs for "My First Plugin loaded successfully!"
3. Test the commands:
   - `hello John` - Should respond with "Hello, John! Welcome to the server!"
   - `time` - Should show the current server time

## Next Steps

Now that you have a working plugin, you can:

1. Add more commands with different parameter types
2. Implement event handlers for game events
3. Create custom items and roles
4. Set up a permission system
5. Add configuration options

Check out the full Developer Guide for detailed information on all framework features!

## Common Issues

**Plugin not loading**: Check that all DLL files are in the correct directory and that there are no compilation errors.

**Commands not working**: Ensure the command service is properly registered and that you have the necessary permissions.

**Events not firing**: Make sure you've subscribed to events in the `OnLoadAsync` method and that the event types are correct.

For more help, check the troubleshooting section in the Developer Guide or visit our community forums.




