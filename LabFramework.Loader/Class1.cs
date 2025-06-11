using System;
using System.Threading.Tasks;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using LabFramework.Core;
using LabFramework.Loader;

namespace Plugin;

internal class HelloWorldPlugin : IPlugin
{
    public string Name { get; } = "Hello World";
    public string Version { get; } = "1.0.0.0";
    public string Author { get; } = "MONCEF50G";
    public string Description { get; } = "This is a simple plugin.";
    public void OnLoadAsync()
    {
    }

    public void OnUnloadAsync()
    {
    }
}

        

 

