using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ArtifactMMO
{
    // ==============================
    // General API response wrapper
    // ==============================
    public class ApiResponse<TResponse>
    {
        [JsonPropertyName("data")]
        public TResponse? Data { get; set; }
    }

    // =======================
    // Movement API Response
    // =======================
    public class MoveResponse : IHasCooldown
    {
        [JsonPropertyName("cooldown")]
        public CooldownData? Cooldown { get; set; }

        [JsonPropertyName("destination")]
        public DestinationData? Destination { get; set; }

        [JsonPropertyName("character")]
        public CharacterData? Character { get; set; }
    }

    // =======================
    // Attack API Response
    // =======================
    public class AttackResponse : IHasCooldown
    {
        [JsonPropertyName("cooldown")]
        public CooldownData? Cooldown { get; set; }
    }

    // =======================
    // Rest API Response
    // =======================
    public class RestResponse : IHasCooldown
    {
        [JsonPropertyName("cooldown")]
        public CooldownData? Cooldown { get; set; }
    }

    // =======================
    // Gathering API Response
    // =======================
    public class gatheringResponse : IHasCooldown
    {
        [JsonPropertyName("cooldown")]
        public CooldownData? Cooldown { get; set; }
    }

    // =======================
    // Unequip API Response
    // =======================
    public class unequipResponse : IHasCooldown
    {
        [JsonPropertyName("cooldown")]
        public CooldownData? Cooldown { get; set; }
    }

    // ==========================
    // Multi-Use API Response's
    // ==========================
    public interface IHasCooldown
    {
        CooldownData? Cooldown { get; set; }
    }

    public class CooldownData
    {
        [JsonPropertyName("total_seconds")]
        public int TotalSeconds { get; set; }

        [JsonPropertyName("remaining_seconds")]
        public int RemainingSeconds { get; set; }

        [JsonPropertyName("started_at")]
        public DateTime StartedAt { get; set; }

        [JsonPropertyName("expiration")]
        public DateTime Expiration { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }

    public class DestinationData
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }
    }

    public class CharacterData
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("xp")]
        public int XP { get; set; }

        [JsonPropertyName("gold")]
        public int Gold { get; set; }

        [JsonPropertyName("cooldown")]
        public int Cooldown { get; set; }
    }
}