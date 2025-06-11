using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LabFramework.Core;
using LabFramework.Core.Logging;

namespace LabFramework.Commands
{
    /// <summary>
    /// Command execution context
    /// </summary>
    public class CommandContext
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string[] Arguments { get; set; }
        public string RawInput { get; set; }
        public bool IsConsole { get; set; }
        public object Sender { get; set; }
        
        public CommandContext(string senderId, string senderName, string[] arguments, string rawInput, bool isConsole = false, object sender = null)
        {
            SenderId = senderId;
            SenderName = senderName;
            Arguments = arguments ?? new string[0];
            RawInput = rawInput;
            IsConsole = isConsole;
            Sender = sender;
        }
    }
    
    /// <summary>
    /// Command execution result
    /// </summary>
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        
        public static CommandResult Successful(string message = null)
        {
            return new CommandResult { Success = true, Message = message };
        }
        
        public static CommandResult Failed(string message, Exception exception = null)
        {
            return new CommandResult { Success = false, Message = message, Exception = exception };
        }
    }
    
    /// <summary>
    /// Command attribute for marking command methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public string[] Aliases { get; }
        public string Permission { get; }
        public bool ConsoleOnly { get; }
        public bool PlayerOnly { get; }
        
        public CommandAttribute(string name, string description = null, string permission = null, bool consoleOnly = false, bool playerOnly = false, params string[] aliases)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? "No description provided";
            Permission = permission;
            ConsoleOnly = consoleOnly;
            PlayerOnly = playerOnly;
            Aliases = aliases ?? new string[0];
        }
    }
    
    /// <summary>
    /// Command parameter attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class CommandParameterAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsOptional { get; }
        public object DefaultValue { get; }
        
        public CommandParameterAttribute(string name, string description = null, bool isOptional = false, object defaultValue = null)
        {
            Name = name;
            Description = description;
            IsOptional = isOptional;
            DefaultValue = defaultValue;
        }
    }
    
    /// <summary>
    /// Command information
    /// </summary>
    public class CommandInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Aliases { get; set; }
        public string Permission { get; set; }
        public bool ConsoleOnly { get; set; }
        public bool PlayerOnly { get; set; }
        public MethodInfo Method { get; set; }
        public object Instance { get; set; }
        public ParameterInfo[] Parameters { get; set; }
        
        public string GetUsage()
        {
            var usage = Name;
            foreach (var param in Parameters.Skip(1)) // Skip CommandContext parameter
            {
                var paramAttr = param.GetCustomAttribute<CommandParameterAttribute>();
                var paramName = paramAttr?.Name ?? param.Name;
                
                if (paramAttr?.IsOptional == true)
                {
                    usage += $" [{paramName}]";
                }
                else
                {
                    usage += $" <{paramName}>";
                }
            }
            return usage;
        }
    }
    
    /// <summary>
    /// Command service interface
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Register commands from an object
        /// </summary>
        /// <param name="commandHandler">Object containing command methods</param>
        void RegisterCommands(object commandHandler);
        
        /// <summary>
        /// Unregister commands from an object
        /// </summary>
        /// <param name="commandHandler">Object containing command methods</param>
        void UnregisterCommands(object commandHandler);
        
        /// <summary>
        /// Execute a command
        /// </summary>
        /// <param name="context">Command context</param>
        /// <returns>Command result</returns>
        Task<CommandResult> ExecuteCommandAsync(CommandContext context);
        
        /// <summary>
        /// Get all registered commands
        /// </summary>
        /// <returns>List of command information</returns>
        IEnumerable<CommandInfo> GetCommands();
        
        /// <summary>
        /// Get command by name or alias
        /// </summary>
        /// <param name="name">Command name or alias</param>
        /// <returns>Command information or null if not found</returns>
        CommandInfo GetCommand(string name);
    }
    
    /// <summary>
    /// Command service implementation
    /// </summary>
    public class CommandService : ICommandService
    {
        private readonly Dictionary<string, CommandInfo> _commands = new();
        private readonly ILoggingService _logger;
        
        public CommandService(ILoggingService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public void RegisterCommands(object commandHandler)
        {
            if (commandHandler == null)
                throw new ArgumentNullException(nameof(commandHandler));
            
            var type = commandHandler.GetType();
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var method in methods)
            {
                var commandAttr = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttr == null)
                    continue;
                
                var parameters = method.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(CommandContext))
                {
                    _logger.LogWarning($"Command method {method.Name} must have CommandContext as first parameter");
                    continue;
                }
                
                var commandInfo = new CommandInfo
                {
                    Name = commandAttr.Name.ToLower(),
                    Description = commandAttr.Description,
                    Aliases = commandAttr.Aliases.Select(a => a.ToLower()).ToArray(),
                    Permission = commandAttr.Permission,
                    ConsoleOnly = commandAttr.ConsoleOnly,
                    PlayerOnly = commandAttr.PlayerOnly,
                    Method = method,
                    Instance = commandHandler,
                    Parameters = parameters
                };
                
                // Register command by name
                _commands[commandInfo.Name] = commandInfo;
                
                // Register command by aliases
                foreach (var alias in commandInfo.Aliases)
                {
                    _commands[alias] = commandInfo;
                }
                
                _logger.LogDebug($"Registered command: {commandInfo.Name}");
            }
        }
        
        public void UnregisterCommands(object commandHandler)
        {
            if (commandHandler == null)
                return;
            
            var commandsToRemove = _commands.Values
                .Where(cmd => cmd.Instance == commandHandler)
                .ToList();
            
            foreach (var command in commandsToRemove)
            {
                // Remove by name
                _commands.Remove(command.Name);
                
                // Remove by aliases
                foreach (var alias in command.Aliases)
                {
                    _commands.Remove(alias);
                }
                
                _logger.LogDebug($"Unregistered command: {command.Name}");
            }
        }
        
        public async Task<CommandResult> ExecuteCommandAsync(CommandContext context)
        {
            if (context == null)
                return CommandResult.Failed("Invalid command context");
            
            if (context.Arguments.Length == 0)
                return CommandResult.Failed("No command specified");
            
            var commandName = context.Arguments[0].ToLower();
            var command = GetCommand(commandName);
            
            if (command == null)
                return CommandResult.Failed($"Unknown command: {commandName}");
            
            // Check console/player restrictions
            if (command.ConsoleOnly && !context.IsConsole)
                return CommandResult.Failed("This command can only be executed from console");
            
            if (command.PlayerOnly && context.IsConsole)
                return CommandResult.Failed("This command can only be executed by players");
            
            // TODO: Check permissions
            if (!string.IsNullOrEmpty(command.Permission))
            {
                // Permission checking would be implemented here
            }
            
            try
            {
                // Prepare method arguments
                var methodParams = command.Parameters;
                var args = new object[methodParams.Length];
                args[0] = context; // First parameter is always CommandContext
                
                // Parse remaining arguments
                for (int i = 1; i < methodParams.Length; i++)
                {
                    var param = methodParams[i];
                    var paramAttr = param.GetCustomAttribute<CommandParameterAttribute>();
                    var argIndex = i; // Arguments include command name, so we need to adjust
                    
                    if (argIndex < context.Arguments.Length)
                    {
                        // Try to convert argument to parameter type
                        args[i] = ConvertArgument(context.Arguments[argIndex], param.ParameterType);
                    }
                    else if (paramAttr?.IsOptional == true)
                    {
                        args[i] = paramAttr.DefaultValue ?? GetDefaultValue(param.ParameterType);
                    }
                    else
                    {
                        return CommandResult.Failed($"Missing required parameter: {paramAttr?.Name ?? param.Name}");
                    }
                }
                
                // Execute command
                var result = command.Method.Invoke(command.Instance, args);
                
                // Handle async methods
                if (result is Task task)
                {
                    await task;
                    
                    if (task.GetType().IsGenericType)
                    {
                        var property = task.GetType().GetProperty("Result");
                        result = property?.GetValue(task);
                    }
                    else
                    {
                        result = null;
                    }
                }
                
                // Convert result to CommandResult
                if (result is CommandResult commandResult)
                {
                    return commandResult;
                }
                else if (result is string message)
                {
                    return CommandResult.Successful(message);
                }
                else
                {
                    return CommandResult.Successful();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing command {commandName}", ex);
                return CommandResult.Failed($"Command execution failed: {ex.Message}", ex);
            }
        }
        
        public IEnumerable<CommandInfo> GetCommands()
        {
            return _commands.Values.Distinct();
        }
        
        public CommandInfo GetCommand(string name)
        {
            _commands.TryGetValue(name.ToLower(), out var command);
            return command;
        }
        
        private object ConvertArgument(string argument, Type targetType)
        {
            if (targetType == typeof(string))
                return argument;
            
            if (targetType == typeof(int))
                return int.Parse(argument);
            
            if (targetType == typeof(float))
                return float.Parse(argument);
            
            if (targetType == typeof(bool))
                return bool.Parse(argument);
            
            // Add more type conversions as needed
            return Convert.ChangeType(argument, targetType);
        }
        
        private object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            
            return null;
        }
    }
}

