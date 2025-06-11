using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LabFramework.Core.Configuration
{
    /// <summary>
    /// Configuration service interface
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Get a configuration value
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Configuration value</returns>
        T GetValue<T>(string key, T defaultValue = default);
        
        /// <summary>
        /// Set a configuration value
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Value to set</param>
        void SetValue<T>(string key, T value);
        
        /// <summary>
        /// Load configuration from file
        /// </summary>
        /// <param name="filePath">Path to configuration file</param>
        /// <returns>Task for async loading</returns>
        Task LoadFromFileAsync(string filePath);
        
        /// <summary>
        /// Save configuration to file
        /// </summary>
        /// <param name="filePath">Path to configuration file</param>
        /// <returns>Task for async saving</returns>
        Task SaveToFileAsync(string filePath);
        
        /// <summary>
        /// Check if a configuration key exists
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>True if key exists</returns>
        bool HasKey(string key);
        
        /// <summary>
        /// Remove a configuration key
        /// </summary>
        /// <param name="key">Configuration key</param>
        void RemoveKey(string key);
    }
    
    /// <summary>
    /// JSON-based configuration service implementation
    /// </summary>
    public class JsonConfigurationService : IConfigurationService
    {
        private readonly Dictionary<string, object> _configuration = new();
        private readonly object _lock = new();
        
        public T GetValue<T>(string key, T defaultValue = default)
        {
            lock (_lock)
            {
                if (!_configuration.ContainsKey(key))
                    return defaultValue;
                
                var value = _configuration[key];
                
                if (value is JsonElement jsonElement)
                {
                    return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
                }
                
                if (value is T directValue)
                    return directValue;
                
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
        }
        
        public void SetValue<T>(string key, T value)
        {
            lock (_lock)
            {
                _configuration[key] = value;
            }
        }
        
        public async Task LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return;
            
            var json =  File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            
            lock (_lock)
            {
                _configuration.Clear();
                if (data != null)
                {
                    foreach (var kvp in data)
                    {
                        _configuration[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
        
        public async Task SaveToFileAsync(string filePath)
        {
            Dictionary<string, object> configCopy;
            lock (_lock)
            {
                configCopy = new Dictionary<string, object>(_configuration);
            }
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            var json = JsonSerializer.Serialize(configCopy, options);
             File.WriteAllText(filePath, json);
        }
        
        public bool HasKey(string key)
        {
            lock (_lock)
            {
                return _configuration.ContainsKey(key);
            }
        }
        
        public void RemoveKey(string key)
        {
            lock (_lock)
            {
                _configuration.Remove(key);
            }
        }
    }
}

