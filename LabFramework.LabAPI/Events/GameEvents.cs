using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LabFramework.Core;
using LabFramework.Core.Events;

namespace LabFramework.LabAPI.Events
{
    /// <summary>
    /// Player-related events
    /// </summary>
    public class PlayerJoinedEvent : BaseEvent
    {
        public string PlayerId { get; }
        public string PlayerName { get; }
        public DateTime JoinTime { get; }
        
        public PlayerJoinedEvent(string playerId, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            JoinTime = DateTime.UtcNow;
        }
    }
    
    public class PlayerLeftEvent : BaseEvent
    {
        public string PlayerId { get; }
        public string PlayerName { get; }
        public DateTime LeaveTime { get; }
        
        public PlayerLeftEvent(string playerId, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            LeaveTime = DateTime.UtcNow;
        }
    }
    
    public class PlayerDiedEvent : BaseEvent
    {
        public string PlayerId { get; }
        public string KillerId { get; }
        public string DeathReason { get; }
        
        public PlayerDiedEvent(string playerId, string killerId, string deathReason)
        {
            PlayerId = playerId;
            KillerId = killerId;
            DeathReason = deathReason;
        }
    }
    
    /// <summary>
    /// Round-related events
    /// </summary>
    public class RoundStartedEvent : BaseEvent
    {
        public int RoundNumber { get; }
        public int PlayerCount { get; }
        
        public RoundStartedEvent(int roundNumber, int playerCount)
        {
            RoundNumber = roundNumber;
            PlayerCount = playerCount;
        }
    }
    
    public class RoundEndedEvent : BaseEvent
    {
        public int RoundNumber { get; }
        public string WinningTeam { get; }
        public TimeSpan RoundDuration { get; }
        
        public RoundEndedEvent(int roundNumber, string winningTeam, TimeSpan roundDuration)
        {
            RoundNumber = roundNumber;
            WinningTeam = winningTeam;
            RoundDuration = roundDuration;
        }
    }
    
    /// <summary>
    /// Item-related events
    /// </summary>
    public class ItemPickedUpEvent : BaseEvent
    {
        public string PlayerId { get; }
        public string ItemId { get; }
        public string ItemType { get; }
        
        public ItemPickedUpEvent(string playerId, string itemId, string itemType)
        {
            PlayerId = playerId;
            ItemId = itemId;
            ItemType = itemType;
        }
    }
    
    public class ItemDroppedEvent : BaseEvent
    {
        public string PlayerId { get; }
        public string ItemId { get; }
        public string ItemType { get; }
        
        public ItemDroppedEvent(string playerId, string itemId, string itemType)
        {
            PlayerId = playerId;
            ItemId = itemId;
            ItemType = itemType;
        }
    }
}

