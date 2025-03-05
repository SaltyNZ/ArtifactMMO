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
        public characterInfoResponse? Character { get; set; }
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
        [JsonPropertyName("character")]
        public characterInfoResponse? Character { get; set; }
        [JsonPropertyName("details")]
        public DetailData? Detail { get; set; }
    }

    // =======================
    // Crafting API Response
    // =======================
    public class craftResponse : IHasCooldown
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

    // =======================
    // equip API Response
    // =======================
    public class equipResponse : IHasCooldown
    {
        [JsonPropertyName("cooldown")]
        public CooldownData? Cooldown { get; set; }
    }
    
    // ==========================
    // Multi-Use API Response's
    // ==========================
    public class bankItemResponse : IHasCooldown
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

    //
    //
    //
    public class characterInfoResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("account")]
        public string? Account { get; set; }

        [JsonPropertyName("skin")]
        public string? Skin { get; set; }

        [JsonPropertyName("level")]
        public int Level { get; set; }

        [JsonPropertyName("xp")]
        public int Xp { get; set; }

        [JsonPropertyName("max_xp")]
        public int MaxXp { get; set; }

        [JsonPropertyName("gold")]
        public int Gold { get; set; }

        [JsonPropertyName("speed")]
        public int Speed { get; set; }

        [JsonPropertyName("hp")]
        public int Hp { get; set; }

        [JsonPropertyName("max_hp")]
        public int MaxHp { get; set; }

        [JsonPropertyName("haste")]
        public int Haste { get; set; }

        [JsonPropertyName("critical_strike")]
        public int CriticalStrike { get; set; }

        [JsonPropertyName("wisdom")]
        public int Wisdom { get; set; }

        [JsonPropertyName("prospecting")]
        public int Prospecting { get; set; }

        [JsonPropertyName("attack_fire")]
        public int AttackFire { get; set; }

        [JsonPropertyName("attack_earth")]
        public int AttackEarth { get; set; }

        [JsonPropertyName("attack_water")]
        public int AttackWater { get; set; }

        [JsonPropertyName("attack_air")]
        public int AttackAir { get; set; }

        [JsonPropertyName("dmg")]
        public int Damage { get; set; }

        [JsonPropertyName("dmg_fire")]
        public int DamageFire { get; set; }

        [JsonPropertyName("dmg_earth")]
        public int DamageEarth { get; set; }

        [JsonPropertyName("dmg_water")]
        public int DamageWater { get; set; }

        [JsonPropertyName("dmg_air")]
        public int DamageAir { get; set; }

        [JsonPropertyName("res_fire")]
        public int ResistanceFire { get; set; }

        [JsonPropertyName("res_earth")]
        public int ResistanceEarth { get; set; }

        [JsonPropertyName("res_water")]
        public int ResistanceWater { get; set; }

        [JsonPropertyName("res_air")]
        public int ResistanceAir { get; set; }

        //World Position

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        //Action Cooldown

        [JsonPropertyName("cooldown")]
        public int Cooldown { get; set; }

        [JsonPropertyName("cooldown_expiration")]
        public DateTime CooldownExpiration { get; set; }

        //Tasks

        [JsonPropertyName("task")]
        public string? Task { get; set; }

        [JsonPropertyName("task_type")]
        public string? TaskType { get; set; }

        [JsonPropertyName("task_progress")]
        public int TaskProgress { get; set; }

        [JsonPropertyName("task_total")]
        public int TaskTotal { get; set; }

        //Item Slots

        [JsonPropertyName("weapon_slot")]
        public string? WeaponSlot { get; set; }

        [JsonPropertyName("rune_slot")]
        public string? RuneSlot { get; set; }

        [JsonPropertyName("shield_slot")]
        public string? ShieldSlot { get; set; }

        [JsonPropertyName("helmet_slot")]
        public string? HelmetSlot { get; set; }

        [JsonPropertyName("body_armor_slot")]
        public string? BodyArmorSlot { get; set; }

        [JsonPropertyName("leg_armor_slot")]
        public string? LegArmorSlot { get; set; }

        [JsonPropertyName("boots_slot")]
        public string? BootsSlot { get; set; }

        [JsonPropertyName("ring1_slot")]
        public string? Ring1Slot { get; set; }

        [JsonPropertyName("ring2_slot")]
        public string? Ring2Slot { get; set; }

        [JsonPropertyName("amulet_slot")]
        public string? AmuletSlot { get; set; }

        [JsonPropertyName("artifact1_slot")]
        public string? Artifact1Slot { get; set; }

        [JsonPropertyName("artifact2_slot")]
        public string? Artifact2Slot { get; set; }

        [JsonPropertyName("artifact3_slot")]
        public string? Artifact3Slot { get; set; }

        [JsonPropertyName("utility1_slot")]
        public string? Utility1Slot { get; set; }

        [JsonPropertyName("utility1_slot_quantity")]
        public int Utility1SlotQuantity { get; set; }

        [JsonPropertyName("utility2_slot")]
        public string? Utility2Slot { get; set; }

        [JsonPropertyName("utility2_slot_quantity")]
        public int Utility2SlotQuantity { get; set; }

        [JsonPropertyName("bag_slot")]
        public string? BagSlot { get; set; }

        // Skills
        [JsonPropertyName("mining_level")]
        public int MiningLevel { get; set; }

        [JsonPropertyName("mining_xp")]
        public int MiningXp { get; set; }

        [JsonPropertyName("mining_max_xp")]
        public int MiningMaxXp { get; set; }

        [JsonPropertyName("woodcutting_level")]
        public int WoodcuttingLevel { get; set; }

        [JsonPropertyName("woodcutting_xp")]
        public int WoodcuttingXp { get; set; }

        [JsonPropertyName("woodcutting_max_xp")]
        public int WoodcuttingMaxXp { get; set; }

        [JsonPropertyName("fishing_level")]
        public int FishingLevel { get; set; }

        [JsonPropertyName("fishing_xp")]
        public int FishingXp { get; set; }

        [JsonPropertyName("fishing_max_xp")]
        public int FishingMaxXp { get; set; }

        [JsonPropertyName("weaponcrafting_level")]
        public int WeaponcraftingLevel { get; set; }

        [JsonPropertyName("weaponcrafting_xp")]
        public int WeaponcraftingXp { get; set; }

        [JsonPropertyName("weaponcrafting_max_xp")]
        public int WeaponcraftingMaxXp { get; set; }

        [JsonPropertyName("gearcrafting_level")]
        public int GearcraftingLevel { get; set; }

        [JsonPropertyName("gearcrafting_xp")]
        public int GearcraftingXp { get; set; }

        [JsonPropertyName("gearcrafting_max_xp")]
        public int GearcraftingMaxXp { get; set; }

        [JsonPropertyName("jewelrycrafting_level")]
        public int JewelrycraftingLevel { get; set; }

        [JsonPropertyName("jewelrycrafting_xp")]
        public int JewelrycraftingXp { get; set; }

        [JsonPropertyName("jewelrycrafting_max_xp")]
        public int JewelrycraftingMaxXp { get; set; }

        [JsonPropertyName("cooking_level")]
        public int CookingLevel { get; set; }

        [JsonPropertyName("cooking_xp")]
        public int CookingXp { get; set; }

        [JsonPropertyName("cooking_max_xp")]
        public int CookingMaxXp { get; set; }

        [JsonPropertyName("alchemy_level")]
        public int AlchemyLevel { get; set; }

        [JsonPropertyName("alchemy_xp")]
        public int AlchemyXp { get; set; }

        [JsonPropertyName("alchemy_max_xp")]
        public int AlchemyMaxXp { get; set; }

        //Inventory

        [JsonPropertyName("inventory_max_items")]
        public int InventoryMaxItems { get; set; }

        [JsonPropertyName("inventory")]
        public List<InventoryData>? Inventory { get; set; }

    }


    public class InventoryData
    {
        [JsonPropertyName("slot")]
        public int Slot { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    public class DetailData
    {
        [JsonPropertyName("xp")]
        public int XP { get; set; }
        [JsonPropertyName("items")]
        public List<ItemsData>? Items { get; set; }
    }

    public class ItemsData
    {
        [JsonPropertyName("code")]
        public string? Code { get; set;}
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}