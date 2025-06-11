using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabFramework.Core;
using LabFramework.Core.Configuration;
using LabFramework.Core.Logging;

namespace LabFramework.Permissions
{
    /// <summary>
    /// Permission node representing a specific permission
    /// </summary>
    public class Permission
    {
        public string Node { get; set; }
        public bool Value { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string GrantedBy { get; set; }
        public string Reason { get; set; }
        
        public Permission(string node, bool value = true, DateTime? expiresAt = null, string grantedBy = null, string reason = null)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Value = value;
            ExpiresAt = expiresAt;
            GrantedBy = grantedBy;
            Reason = reason;
        }
        
        public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
        
        public bool IsValid => !IsExpired;
    }
    
    /// <summary>
    /// Permission group containing multiple permissions
    /// </summary>
    public class PermissionGroup
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<string> InheritedGroups { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        
        public PermissionGroup(string name, string displayName = null, string description = null, int priority = 0)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? name;
            Description = description ?? "";
            Priority = priority;
            Permissions = new List<Permission>();
            InheritedGroups = new List<string>();
            Metadata = new Dictionary<string, object>();
        }
        
        public void AddPermission(string node, bool value = true, DateTime? expiresAt = null, string grantedBy = null, string reason = null)
        {
            var permission = new Permission(node, value, expiresAt, grantedBy, reason);
            Permissions.RemoveAll(p => p.Node == node); // Remove existing permission with same node
            Permissions.Add(permission);
        }
        
        public void RemovePermission(string node)
        {
            Permissions.RemoveAll(p => p.Node == node);
        }
        
        public bool HasPermission(string node)
        {
            var permission = Permissions.FirstOrDefault(p => p.Node == node && p.IsValid);
            return permission?.Value ?? false;
        }
    }
    
    /// <summary>
    /// User permission data
    /// </summary>
    public class UserPermissions
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public List<string> Groups { get; set; }
        public List<Permission> DirectPermissions { get; set; }
        public DateTime LastUpdated { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        
        public UserPermissions(string userId, string username = null)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Username = username ?? userId;
            Groups = new List<string>();
            DirectPermissions = new List<Permission>();
            LastUpdated = DateTime.UtcNow;
            Metadata = new Dictionary<string, object>();
        }
        
        public void AddGroup(string groupName)
        {
            if (!Groups.Contains(groupName))
            {
                Groups.Add(groupName);
                LastUpdated = DateTime.UtcNow;
            }
        }
        
        public void RemoveGroup(string groupName)
        {
            if (Groups.Remove(groupName))
            {
                LastUpdated = DateTime.UtcNow;
            }
        }
        
        public void AddPermission(string node, bool value = true, DateTime? expiresAt = null, string grantedBy = null, string reason = null)
        {
            var permission = new Permission(node, value, expiresAt, grantedBy, reason);
            DirectPermissions.RemoveAll(p => p.Node == node);
            DirectPermissions.Add(permission);
            LastUpdated = DateTime.UtcNow;
        }
        
        public void RemovePermission(string node)
        {
            if (DirectPermissions.RemoveAll(p => p.Node == node) > 0)
            {
                LastUpdated = DateTime.UtcNow;
            }
        }
    }
    
    /// <summary>
    /// Permission service interface
    /// </summary>
    public interface IPermissionService
    {
        // Group management
        Task<PermissionGroup> CreateGroupAsync(string name, string displayName = null, string description = null, int priority = 0);
        Task<bool> DeleteGroupAsync(string name);
        Task<PermissionGroup> GetGroupAsync(string name);
        Task<IEnumerable<PermissionGroup>> GetAllGroupsAsync();
        Task<bool> UpdateGroupAsync(PermissionGroup group);
        
        // User management
        Task<UserPermissions> GetUserPermissionsAsync(string userId);
        Task<bool> SetUserPermissionsAsync(UserPermissions userPermissions);
        Task<bool> AddUserToGroupAsync(string userId, string groupName);
        Task<bool> RemoveUserFromGroupAsync(string userId, string groupName);
        Task<bool> SetUserPermissionAsync(string userId, string node, bool value = true, DateTime? expiresAt = null, string grantedBy = null, string reason = null);
        Task<bool> RemoveUserPermissionAsync(string userId, string node);
        
        // Permission checking
        Task<bool> HasPermissionAsync(string userId, string node);
        Task<bool> HasAnyPermissionAsync(string userId, params string[] nodes);
        Task<bool> HasAllPermissionsAsync(string userId, params string[] nodes);
        Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, bool includeGroupPermissions = true);
        
        // Utility methods
        Task<bool> IsUserInGroupAsync(string userId, string groupName);
        Task<IEnumerable<string>> GetUserGroupsAsync(string userId);
        Task CleanupExpiredPermissionsAsync();
        Task<bool> SaveAsync();
        Task<bool> LoadAsync();
    }
    
    /// <summary>
    /// Permission service implementation
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly Dictionary<string, PermissionGroup> _groups = new();
        private readonly Dictionary<string, UserPermissions> _users = new();
        private readonly IConfigurationService _configuration;
        private readonly ILoggingService _logger;
        private readonly object _lock = new();
        
        public PermissionService(IConfigurationService configuration, ILoggingService logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            InitializeDefaultGroups();
        }
        
        private void InitializeDefaultGroups()
        {
            // Create default groups
            var defaultGroup = new PermissionGroup("default", "Default", "Default group for all users", 0);
            defaultGroup.AddPermission("basic.chat", true);
            defaultGroup.AddPermission("basic.move", true);
            
            var adminGroup = new PermissionGroup("admin", "Administrator", "Administrator group with full permissions", 1000);
            adminGroup.AddPermission("*", true); // Wildcard permission for admins
            
            var moderatorGroup = new PermissionGroup("moderator", "Moderator", "Moderator group with limited admin permissions", 500);
            moderatorGroup.AddPermission("admin.kick", true);
            moderatorGroup.AddPermission("admin.mute", true);
            moderatorGroup.AddPermission("admin.teleport", true);
            moderatorGroup.InheritedGroups.Add("default");
            
            _groups["default"] = defaultGroup;
            _groups["admin"] = adminGroup;
            _groups["moderator"] = moderatorGroup;
        }
        
        public async Task<PermissionGroup> CreateGroupAsync(string name, string displayName = null, string description = null, int priority = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Group name cannot be null or empty", nameof(name));
            
            lock (_lock)
            {
                if (_groups.ContainsKey(name.ToLower()))
                    return null; // Group already exists
                
                var group = new PermissionGroup(name.ToLower(), displayName, description, priority);
                _groups[name.ToLower()] = group;
                
                _logger.LogInformation($"Created permission group: {name}");
                return group;
            }
        }
        
        public async Task<bool> DeleteGroupAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            
            lock (_lock)
            {
                var removed = _groups.Remove(name.ToLower());
                if (removed)
                {
                    // Remove group from all users
                    foreach (var user in _users.Values)
                    {
                        user.RemoveGroup(name.ToLower());
                    }
                    
                    _logger.LogInformation($"Deleted permission group: {name}");
                }
                return removed;
            }
        }
        
        public async Task<PermissionGroup> GetGroupAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            
            lock (_lock)
            {
                _groups.TryGetValue(name.ToLower(), out var group);
                return group;
            }
        }
        
        public async Task<IEnumerable<PermissionGroup>> GetAllGroupsAsync()
        {
            lock (_lock)
            {
                return _groups.Values.ToList();
            }
        }
        
        public async Task<bool> UpdateGroupAsync(PermissionGroup group)
        {
            if (group == null)
                return false;
            
            lock (_lock)
            {
                _groups[group.Name.ToLower()] = group;
                _logger.LogInformation($"Updated permission group: {group.Name}");
                return true;
            }
        }
        
        public async Task<UserPermissions> GetUserPermissionsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;
            
            lock (_lock)
            {
                if (!_users.TryGetValue(userId, out var userPermissions))
                {
                    userPermissions = new UserPermissions(userId);
                    userPermissions.AddGroup("default"); // Add to default group
                    _users[userId] = userPermissions;
                }
                
                return userPermissions;
            }
        }
        
        public async Task<bool> SetUserPermissionsAsync(UserPermissions userPermissions)
        {
            if (userPermissions == null)
                return false;
            
            lock (_lock)
            {
                _users[userPermissions.UserId] = userPermissions;
                _logger.LogDebug($"Updated permissions for user: {userPermissions.UserId}");
                return true;
            }
        }
        
        public async Task<bool> AddUserToGroupAsync(string userId, string groupName)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(groupName))
                return false;
            
            var userPermissions = await GetUserPermissionsAsync(userId);
            var group = await GetGroupAsync(groupName);
            
            if (userPermissions == null || group == null)
                return false;
            
            userPermissions.AddGroup(groupName.ToLower());
            _logger.LogInformation($"Added user {userId} to group {groupName}");
            return true;
        }
        
        public async Task<bool> RemoveUserFromGroupAsync(string userId, string groupName)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(groupName))
                return false;
            
            var userPermissions = await GetUserPermissionsAsync(userId);
            if (userPermissions == null)
                return false;
            
            userPermissions.RemoveGroup(groupName.ToLower());
            _logger.LogInformation($"Removed user {userId} from group {groupName}");
            return true;
        }
        
        public async Task<bool> SetUserPermissionAsync(string userId, string node, bool value = true, DateTime? expiresAt = null, string grantedBy = null, string reason = null)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(node))
                return false;
            
            var userPermissions = await GetUserPermissionsAsync(userId);
            if (userPermissions == null)
                return false;
            
            userPermissions.AddPermission(node, value, expiresAt, grantedBy, reason);
            _logger.LogInformation($"Set permission {node}={value} for user {userId}");
            return true;
        }
        
        public async Task<bool> RemoveUserPermissionAsync(string userId, string node)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(node))
                return false;
            
            var userPermissions = await GetUserPermissionsAsync(userId);
            if (userPermissions == null)
                return false;
            
            userPermissions.RemovePermission(node);
            _logger.LogInformation($"Removed permission {node} from user {userId}");
            return true;
        }
        
        public async Task<bool> HasPermissionAsync(string userId, string node)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(node))
                return false;
            
            var userPermissions = await GetUserPermissionsAsync(userId);
            if (userPermissions == null)
                return false;
            
            // Check direct permissions first
            var directPermission = userPermissions.DirectPermissions.FirstOrDefault(p => p.Node == node && p.IsValid);
            if (directPermission != null)
                return directPermission.Value;
            
            // Check wildcard permissions
            var wildcardPermission = userPermissions.DirectPermissions.FirstOrDefault(p => p.Node == "*" && p.IsValid);
            if (wildcardPermission != null)
                return wildcardPermission.Value;
            
            // Check group permissions
            foreach (var groupName in userPermissions.Groups)
            {
                var group = await GetGroupAsync(groupName);
                if (group == null) continue;
                
                if (await CheckGroupPermissionAsync(group, node, new HashSet<string>()))
                    return true;
            }
            
            return false;
        }
        
        private async Task<bool> CheckGroupPermissionAsync(PermissionGroup group, string node, HashSet<string> visitedGroups)
        {
            if (visitedGroups.Contains(group.Name))
                return false; // Prevent infinite recursion
            
            visitedGroups.Add(group.Name);
            
            // Check direct group permissions
            var permission = group.Permissions.FirstOrDefault(p => p.Node == node && p.IsValid);
            if (permission != null)
                return permission.Value;
            
            // Check wildcard permissions
            var wildcardPermission = group.Permissions.FirstOrDefault(p => p.Node == "*" && p.IsValid);
            if (wildcardPermission != null)
                return wildcardPermission.Value;
            
            // Check inherited groups
            foreach (var inheritedGroupName in group.InheritedGroups)
            {
                var inheritedGroup = await GetGroupAsync(inheritedGroupName);
                if (inheritedGroup != null && await CheckGroupPermissionAsync(inheritedGroup, node, visitedGroups))
                    return true;
            }
            
            return false;
        }
        
        public async Task<bool> HasAnyPermissionAsync(string userId, params string[] nodes)
        {
            if (nodes == null || nodes.Length == 0)
                return false;
            
            foreach (var node in nodes)
            {
                if (await HasPermissionAsync(userId, node))
                    return true;
            }
            
            return false;
        }
        
        public async Task<bool> HasAllPermissionsAsync(string userId, params string[] nodes)
        {
            if (nodes == null || nodes.Length == 0)
                return true;
            
            foreach (var node in nodes)
            {
                if (!await HasPermissionAsync(userId, node))
                    return false;
            }
            
            return true;
        }
        
        public async Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, bool includeGroupPermissions = true)
        {
            var permissions = new HashSet<string>();
            var userPermissions = await GetUserPermissionsAsync(userId);
            
            if (userPermissions == null)
                return permissions;
            
            // Add direct permissions
            foreach (var permission in userPermissions.DirectPermissions.Where(p => p.IsValid && p.Value))
            {
                permissions.Add(permission.Node);
            }
            
            // Add group permissions
            if (includeGroupPermissions)
            {
                foreach (var groupName in userPermissions.Groups)
                {
                    var group = await GetGroupAsync(groupName);
                    if (group != null)
                    {
                        await AddGroupPermissionsAsync(group, permissions, new HashSet<string>());
                    }
                }
            }
            
            return permissions;
        }
        
        private async Task AddGroupPermissionsAsync(PermissionGroup group, HashSet<string> permissions, HashSet<string> visitedGroups)
        {
            if (visitedGroups.Contains(group.Name))
                return;
            
            visitedGroups.Add(group.Name);
            
            // Add group permissions
            foreach (var permission in group.Permissions.Where(p => p.IsValid && p.Value))
            {
                permissions.Add(permission.Node);
            }
            
            // Add inherited group permissions
            foreach (var inheritedGroupName in group.InheritedGroups)
            {
                var inheritedGroup = await GetGroupAsync(inheritedGroupName);
                if (inheritedGroup != null)
                {
                    await AddGroupPermissionsAsync(inheritedGroup, permissions, visitedGroups);
                }
            }
        }
        
        public async Task<bool> IsUserInGroupAsync(string userId, string groupName)
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            return userPermissions?.Groups.Contains(groupName.ToLower()) ?? false;
        }
        
        public async Task<IEnumerable<string>> GetUserGroupsAsync(string userId)
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            return userPermissions?.Groups ?? new List<string>();
        }
        
        public async Task CleanupExpiredPermissionsAsync()
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                var cleanedUsers = 0;
                var cleanedGroups = 0;
                
                // Clean user permissions
                foreach (var user in _users.Values)
                {
                    var removedCount = user.DirectPermissions.RemoveAll(p => p.IsExpired);
                    if (removedCount > 0)
                    {
                        user.LastUpdated = now;
                        cleanedUsers++;
                    }
                }
                
                // Clean group permissions
                foreach (var group in _groups.Values)
                {
                    var removedCount = group.Permissions.RemoveAll(p => p.IsExpired);
                    if (removedCount > 0)
                    {
                        cleanedGroups++;
                    }
                }
                
                if (cleanedUsers > 0 || cleanedGroups > 0)
                {
                    _logger.LogInformation($"Cleaned expired permissions: {cleanedUsers} users, {cleanedGroups} groups");
                }
            }
        }
        
        public async Task<bool> SaveAsync()
        {
            try
            {
                lock (_lock)
                {
                    var data = new
                    {
                        Groups = _groups,
                        Users = _users,
                        LastSaved = DateTime.UtcNow
                    };
                    
                    _configuration.SetValue("PermissionData", data);
                }
                
                await _configuration.SaveToFileAsync("permissions.json");
                _logger.LogInformation("Saved permission data");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save permission data", ex);
                return false;
            }
        }
        
        public async Task<bool> LoadAsync()
        {
            try
            {
                await _configuration.LoadFromFileAsync("permissions.json");
                
                var data = _configuration.GetValue<dynamic>("PermissionData");
                if (data  != null)
                {
                    // TODO: Implement proper deserialization
                    _logger.LogInformation("Loaded permission data");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load permission data", ex);
                return false;
            }
        }
    }
}

