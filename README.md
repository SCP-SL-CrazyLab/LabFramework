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

