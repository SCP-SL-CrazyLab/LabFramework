using System;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using LabFramework.Core;

namespace Plugin;

internal class HelloWorldPlugin : LabApi.Loader.Features.Plugins.Plugin
{
    public override string Name { get; } = "Plugin Loader Core";

    public override string Description { get; } = "Core loader plugin.";

    public override string Author { get; } = "Rakun - MONCEF50G";

    public override Version Version { get; } = new Version(1, 0, 0, 0);

    public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);

    public override void Enable()
    {
        Logger.Info("framework core initialized");
        
    }

    public override void Disable()
    {
    }

    }
