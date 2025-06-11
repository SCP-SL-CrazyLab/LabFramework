using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabFramework.Core;
using LabFramework.Core.Events;
using LabFramework.Core.Logging;
using LabFramework.LabAPI.Wrappers;

namespace LabFramework.CustomItems
{
    /// <summary>
    /// Custom item behavior interface
    /// </summary>
    public interface ICustomItemBehavior
    {
        /// <summary>
        /// Called when the item is picked up
        /// </summary>
        Task OnPickupAsync(PlayerWrapper player, CustomItem item);
        
        /// <summary>
        /// Called when the item is dropped
        /// </summary>
        Task OnDropAsync(PlayerWrapper player, CustomItem item);
        
        /// <summary>
        /// Called when the item is used
        /// </summary>
        Task OnUseAsync(PlayerWrapper player, CustomItem item);
        
        /// <summary>
        /// Called when the item is thrown
        /// </summary>
        Task OnThrowAsync(PlayerWrapper player, CustomItem item);
        
        /// <summary>
        /// Called periodically while the item is held
        /// </summary>
        Task OnUpdateAsync(PlayerWrapper player, CustomItem item);
    }
    
    /// <summary>
    /// Custom item definition
    /// </summary>
    public class CustomItemDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BaseItemType { get; set; }
        public int MaxDurability { get; set; }
        public bool IsStackable { get; set; }
        public int MaxStackSize { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public List<string> Tags { get; set; }
        public string IconPath { get; set; }
        public string ModelPath { get; set; }
        public Type BehaviorType { get; set; }
        
        public CustomItemDefinition()
        {
            Properties = new Dictionary<string, object>();
            Tags = new List<string>();
            MaxDurability = 100;
            IsStackable = false;
            MaxStackSize = 1;
        }
        
        public T GetProperty<T>(string key, T defaultValue = default)
        {
            if (Properties.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }
        
        public void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }
        
        public bool HasTag(string tag)
        {
            return Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
        }
        
        public void AddTag(string tag)
        {
            if (!HasTag(tag))
            {
                Tags.Add(tag);
            }
        }
        
        public void RemoveTag(string tag)
        {
            Tags.RemoveAll(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase));
        }
    }
    
    /// <summary>
    /// Custom item instance
    /// </summary>
    public class CustomItem
    {
        public string InstanceId { get; private set; }
        public CustomItemDefinition Definition { get; private set; }
        public int CurrentDurability { get; set; }
        public int StackSize { get; set; }
        public DateTime CreatedAt { get; private set; }
        public string CreatedBy { get; set; }
        public Dictionary<string, object> InstanceData { get; private set; }
        public ICustomItemBehavior Behavior { get; private set; }
        
        public CustomItem(CustomItemDefinition definition, string createdBy = null)
        {
            InstanceId = Guid.NewGuid().ToString();
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            CurrentDurability = definition.MaxDurability;
            StackSize = 1;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
            InstanceData = new Dictionary<string, object>();
            
            // Create behavior instance
            if (definition.BehaviorType != null)
            {
                try
                {
                    Behavior = (ICustomItemBehavior)Activator.CreateInstance(definition.BehaviorType);
                }
                catch (Exception ex)
                {
                    // Log error but don't fail item creation
                    Console.WriteLine($"Failed to create behavior for item {definition.Id}: {ex.Message}");
                }
            }
        }
        
        public bool IsDestroyed => CurrentDurability <= 0;
        public bool IsStackFull => StackSize >= Definition.MaxStackSize;
        public float DurabilityPercentage => Definition.MaxDurability > 0 ? (float)CurrentDurability / Definition.MaxDurability : 1.0f;
        
        public T GetInstanceData<T>(string key, T defaultValue = default)
        {
            if (InstanceData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }
        
        public void SetInstanceData<T>(string key, T value)
        {
            InstanceData[key] = value;
        }
        
        public void DamageItem(int damage)
        {
            CurrentDurability = Math.Max(0, CurrentDurability - damage);
        }
        
        public void RepairItem(int amount)
        {
            CurrentDurability = Math.Min(Definition.MaxDurability, CurrentDurability + amount);
        }
        
        public bool CanStackWith(CustomItem other)
        {
            return other != null &&
                   Definition.IsStackable &&
                   Definition.Id == other.Definition.Id &&
                   !IsStackFull &&
                   !other.IsStackFull;
        }
        
        public int AddToStack(int amount)
        {
            if (!Definition.IsStackable)
                return 0;
            
            var canAdd = Math.Min(amount, Definition.MaxStackSize - StackSize);
            StackSize += canAdd;
            return canAdd;
        }
        
        public int RemoveFromStack(int amount)
        {
            var canRemove = Math.Min(amount, StackSize);
            StackSize -= canRemove;
            return canRemove;
        }
    }
    
    /// <summary>
    /// Custom item events
    /// </summary>
    public class CustomItemPickedUpEvent : BaseEvent
    {
        public PlayerWrapper Player { get; }
        public CustomItem Item { get; }
        
        public CustomItemPickedUpEvent(PlayerWrapper player, CustomItem item)
        {
            Player = player;
            Item = item;
        }
    }
    
    public class CustomItemDroppedEvent : BaseEvent
    {
        public PlayerWrapper Player { get; }
        public CustomItem Item { get; }
        public Vector3 Position { get; }
        
        public CustomItemDroppedEvent(PlayerWrapper player, CustomItem item, Vector3 position)
        {
            Player = player;
            Item = item;
            Position = position;
        }
    }
    
    public class CustomItemUsedEvent : BaseEvent
    {
        public PlayerWrapper Player { get; }
        public CustomItem Item { get; }
        
        public CustomItemUsedEvent(PlayerWrapper player, CustomItem item)
        {
            Player = player;
            Item = item;
        }
    }
    
    /// <summary>
    /// Custom item service interface
    /// </summary>
    public interface ICustomItemService
    {
        /// <summary>
        /// Register a custom item definition
        /// </summary>
        bool RegisterItem(CustomItemDefinition definition);
        
        /// <summary>
        /// Unregister a custom item definition
        /// </summary>
        bool UnregisterItem(string itemId);
        
        /// <summary>
        /// Get a custom item definition
        /// </summary>
        CustomItemDefinition GetItemDefinition(string itemId);
        
        /// <summary>
        /// Get all registered item definitions
        /// </summary>
        IEnumerable<CustomItemDefinition> GetAllItemDefinitions();
        
        /// <summary>
        /// Create a custom item instance
        /// </summary>
        CustomItem CreateItem(string itemId, string createdBy = null);
        
        /// <summary>
        /// Give a custom item to a player
        /// </summary>
        Task<bool> GiveItemToPlayerAsync(PlayerWrapper player, string itemId, int amount = 1);
        
        /// <summary>
        /// Remove a custom item from a player
        /// </summary>
        Task<bool> RemoveItemFromPlayerAsync(PlayerWrapper player, string itemId, int amount = 1);
        
        /// <summary>
        /// Get custom items in player inventory
        /// </summary>
        IEnumerable<CustomItem> GetPlayerCustomItems(PlayerWrapper player);
        
        /// <summary>
        /// Check if player has a custom item
        /// </summary>
        bool PlayerHasItem(PlayerWrapper player, string itemId);
        
        /// <summary>
        /// Spawn a custom item in the world
        /// </summary>
        Task<bool> SpawnItemAsync(string itemId, Vector3 position, string spawnedBy = null);
        
        /// <summary>
        /// Update all custom items (called periodically)
        /// </summary>
        Task UpdateItemsAsync();
    }
    
    /// <summary>
    /// Custom item service implementation
    /// </summary>
    public class CustomItemService : ICustomItemService
    {
        private readonly Dictionary<string, CustomItemDefinition> _itemDefinitions = new();
        private readonly Dictionary<string, List<CustomItem>> _playerItems = new();
        private readonly Dictionary<string, CustomItem> _worldItems = new();
        private readonly IEventBus _eventBus;
        private readonly ILoggingService _logger;
        private readonly object _lock = new();
        
        public CustomItemService(IEventBus eventBus, ILoggingService logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            RegisterDefaultItems();
        }
        
        private void RegisterDefaultItems()
        {
            // Register some example custom items
            var healingPotion = new CustomItemDefinition
            {
                Id = "healing_potion",
                Name = "Healing Potion",
                Description = "A magical potion that restores health",
                BaseItemType = "Medkit",
                MaxDurability = 1,
                IsStackable = true,
                MaxStackSize = 5,
                BehaviorType = typeof(HealingPotionBehavior)
            };
            healingPotion.SetProperty("HealAmount", 50);
            healingPotion.AddTag("consumable");
            healingPotion.AddTag("medical");
            
            var speedBoots = new CustomItemDefinition
            {
                Id = "speed_boots",
                Name = "Speed Boots",
                Description = "Boots that increase movement speed",
                BaseItemType = "Armor",
                MaxDurability = 100,
                IsStackable = false,
                BehaviorType = typeof(SpeedBootsBehavior)
            };
            speedBoots.SetProperty("SpeedMultiplier", 1.5f);
            speedBoots.SetProperty("Duration", 30);
            speedBoots.AddTag("equipment");
            speedBoots.AddTag("speed");
            
            RegisterItem(healingPotion);
            RegisterItem(speedBoots);
        }
        
        public bool RegisterItem(CustomItemDefinition definition)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.Id))
                return false;
            
            lock (_lock)
            {
                if (_itemDefinitions.ContainsKey(definition.Id))
                {
                    _logger.LogWarning($"Custom item {definition.Id} is already registered");
                    return false;
                }
                
                _itemDefinitions[definition.Id] = definition;
                _logger.LogInformation($"Registered custom item: {definition.Id} ({definition.Name})");
                return true;
            }
        }
        
        public bool UnregisterItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return false;
            
            lock (_lock)
            {
                var removed = _itemDefinitions.Remove(itemId);
                if (removed)
                {
                    _logger.LogInformation($"Unregistered custom item: {itemId}");
                    
                    // Remove all instances of this item
                    foreach (var playerItems in _playerItems.Values)
                    {
                        playerItems.RemoveAll(item => item.Definition.Id == itemId);
                    }
                    
                    var worldItemsToRemove = _worldItems.Where(kvp => kvp.Value.Definition.Id == itemId).Select(kvp => kvp.Key).ToList();
                    foreach (var worldItemId in worldItemsToRemove)
                    {
                        _worldItems.Remove(worldItemId);
                    }
                }
                return removed;
            }
        }
        
        public CustomItemDefinition GetItemDefinition(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return null;
            
            lock (_lock)
            {
                _itemDefinitions.TryGetValue(itemId, out var definition);
                return definition;
            }
        }
        
        public IEnumerable<CustomItemDefinition> GetAllItemDefinitions()
        {
            lock (_lock)
            {
                return _itemDefinitions.Values.ToList();
            }
        }
        
        public CustomItem CreateItem(string itemId, string createdBy = null)
        {
            var definition = GetItemDefinition(itemId);
            if (definition == null)
                return null;
            
            return new CustomItem(definition, createdBy);
        }
        
        public async Task<bool> GiveItemToPlayerAsync(PlayerWrapper player, string itemId, int amount = 1)
        {
            if (player == null || string.IsNullOrWhiteSpace(itemId) || amount <= 0)
                return false;

            var definition = GetItemDefinition(itemId);
            if (definition == null)
                return false;

            List<CustomItem> newItems = new List<CustomItem>();
            
            lock (_lock)
            {
                if (!_playerItems.ContainsKey(player.Id))
                {
                    _playerItems[player.Id] = new List<CustomItem>();
                }

                var playerItems = _playerItems[player.Id];
                var remainingAmount = amount;

                // Try to stack with existing items first
                if (definition.IsStackable)
                {
                    foreach (var existingItem in playerItems.Where(item => item.Definition.Id == itemId && !item.IsStackFull))
                    {
                        var added = existingItem.AddToStack(remainingAmount);
                        remainingAmount -= added;

                        if (remainingAmount <= 0)
                            break;
                    }
                }

                // Create new items for remaining amount
                while (remainingAmount > 0)
                {
                    var newItem = CreateItem(itemId, "system");
                    if (newItem == null)
                        break;

                    if (definition.IsStackable)
                    {
                        var stackAmount = Math.Min(remainingAmount, definition.MaxStackSize);
                        newItem.StackSize = stackAmount;
                        remainingAmount -= stackAmount;
                    }
                    else
                    {
                        remainingAmount--;
                    }

                    playerItems.Add(newItem);
                    newItems.Add(newItem);
                }
            }
            
            // Handle events and behaviors outside the lock
            foreach (var newItem in newItems)
            {
                // Trigger pickup event
                await _eventBus.PublishAsync(new CustomItemPickedUpEvent(player, newItem));

                // Call behavior
                if (newItem.Behavior != null)
                {
                    try
                    {
                        await newItem.Behavior.OnPickupAsync(player, newItem);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error in custom item behavior OnPickupAsync for {itemId}", ex);
                    }
                }
            }

            _logger.LogDebug($"Gave {newItems.Count} {itemId} to player {player.Id}");
            return newItems.Count > 0;
        }
        
        public async Task<bool> RemoveItemFromPlayerAsync(PlayerWrapper player, string itemId, int amount = 1)
        {
            if (player == null || string.IsNullOrWhiteSpace(itemId) || amount <= 0)
                return false;
            
            lock (_lock)
            {
                if (!_playerItems.ContainsKey(player.Id))
                    return false;
                
                var playerItems = _playerItems[player.Id];
                var remainingAmount = amount;
                
                for (int i = playerItems.Count - 1; i >= 0 && remainingAmount > 0; i--)
                {
                    var item = playerItems[i];
                    if (item.Definition.Id != itemId)
                        continue;
                    
                    var removeAmount = Math.Min(remainingAmount, item.StackSize);
                    item.RemoveFromStack(removeAmount);
                    remainingAmount -= removeAmount;
                    
                    if (item.StackSize <= 0)
                    {
                        playerItems.RemoveAt(i);
                    }
                }
                
                _logger.LogDebug($"Removed {amount - remainingAmount} {itemId} from player {player.Id}");
                return remainingAmount == 0;
            }
        }
        
        public IEnumerable<CustomItem> GetPlayerCustomItems(PlayerWrapper player)
        {
            if (player == null)
                return Enumerable.Empty<CustomItem>();
            
            lock (_lock)
            {
                if (_playerItems.TryGetValue(player.Id, out var items))
                {
                    return items.ToList();
                }
                return Enumerable.Empty<CustomItem>();
            }
        }
        
        public bool PlayerHasItem(PlayerWrapper player, string itemId)
        {
            return GetPlayerCustomItems(player).Any(item => item.Definition.Id == itemId);
        }
        
        public async Task<bool> SpawnItemAsync(string itemId, Vector3 position, string spawnedBy = null)
        {
            var item = CreateItem(itemId, spawnedBy);
            if (item == null)
                return false;
            
            lock (_lock)
            {
                _worldItems[item.InstanceId] = item;
            }
            
            // TODO: Implement actual world spawning via LabAPI
            _logger.LogDebug($"Spawned custom item {itemId} at {position}");
            return true;
        }
        
        public async Task UpdateItemsAsync()
        {
            // Update player items
            lock (_lock)
            {
                foreach (var playerItems in _playerItems.Values)
                {
                    foreach (var item in playerItems.ToList())
                    {
                        if (item.Behavior != null)
                        {
                            try
                            {
                                // TODO: Get actual player wrapper
                                // await item.Behavior.OnUpdateAsync(player, item);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error in custom item behavior OnUpdateAsync for {item.Definition.Id}", ex);
                            }
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Example custom item behaviors
    /// </summary>
    public class HealingPotionBehavior : ICustomItemBehavior
    {
        public async Task OnPickupAsync(PlayerWrapper player, CustomItem item)
        {
            // Nothing special on pickup
        }
        
        public async Task OnDropAsync(PlayerWrapper player, CustomItem item)
        {
            // Nothing special on drop
        }
        
        public async Task OnUseAsync(PlayerWrapper player, CustomItem item)
        {
            var healAmount = item.Definition.GetProperty<int>("HealAmount", 25);
            
            // Heal the player
            var newHealth = Math.Min(player.MaxHealth, player.Health + healAmount);
            player.SetHealth(newHealth);
            
            // Consume the item
            item.DamageItem(item.CurrentDurability);
            
            player.SendMessage($"You used a healing potion and restored {healAmount} health!");
        }
        
        public async Task OnThrowAsync(PlayerWrapper player, CustomItem item)
        {
            // Could create a healing area effect
        }
        
        public async Task OnUpdateAsync(PlayerWrapper player, CustomItem item)
        {
            // Nothing to update for healing potions
        }
    }
    
    public class SpeedBootsBehavior : ICustomItemBehavior
    {
        public async Task OnPickupAsync(PlayerWrapper player, CustomItem item)
        {
            // Apply speed boost when picked up
            var speedMultiplier = item.Definition.GetProperty<float>("SpeedMultiplier", 1.5f);
            // TODO: Apply speed effect via LabAPI
            
            player.SendMessage($"You equipped speed boots! Movement speed increased by {(speedMultiplier - 1) * 100}%");
        }
        
        public async Task OnDropAsync(PlayerWrapper player, CustomItem item)
        {
            // Remove speed boost when dropped
            // TODO: Remove speed effect via LabAPI
            
            player.SendMessage("You removed the speed boots. Movement speed returned to normal.");
        }
        
        public async Task OnUseAsync(PlayerWrapper player, CustomItem item)
        {
            // Maybe activate a temporary super speed boost
        }
        
        public async Task OnThrowAsync(PlayerWrapper player, CustomItem item)
        {
            // Nothing special on throw
        }
        
        public async Task OnUpdateAsync(PlayerWrapper player, CustomItem item)
        {
            // Gradually wear down the boots
            if (DateTime.UtcNow.Second % 10 == 0) // Every 10 seconds
            {
                item.DamageItem(1);
                
                if (item.IsDestroyed)
                {
                    player.SendMessage("Your speed boots have worn out!");
                    // TODO: Remove item from player inventory
                }
            }
        }
    }
}

