using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostedDoreto;

public enum GhostedDoretoCategory
{
    Settings,
    GhostingMods,
    CombatMods,
    MovementMods,
    VisualMods,
    UtilityMods,
    WorldMods
}

public sealed class GhostedDoretoFeature
{
    public string Name { get; set; } = string.Empty;
    public GhostedDoretoCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Enabled { get; set; }

    public GhostedDoretoFeature(string name, GhostedDoretoCategory category, string description, bool enabled = false)
    {
        Name = name;
        Category = category;
        Description = description;
        Enabled = enabled;
    }
}

public sealed class GhostedDoretoMenuController
{
    private readonly Dictionary<string, GhostedDoretoFeature> _features;
    private GhostedDoretoCategory _currentCategory = GhostedDoretoCategory.GhostingMods;

    public GhostedDoretoMenuController()
    {
        _features = new Dictionary<string, GhostedDoretoFeature>(StringComparer.OrdinalIgnoreCase)
        {
            // Settings
            ["Menu Scale"] = new("Menu Scale", GhostedDoretoCategory.Settings, "Adjust menu size", false),
            ["Menu Opacity"] = new("Menu Opacity", GhostedDoretoCategory.Settings, "Adjust menu transparency", false),
            ["Auto Save"] = new("Auto Save", GhostedDoretoCategory.Settings, "Save settings automatically", true),

            // Ghosting Mods
            ["Ghost Monke"] = new("Ghost Monke", GhostedDoretoCategory.GhostingMods, "Visible rig + transparent ghost rig", false),
            ["Ghost Rig Visible"] = new("Ghost Rig Visible", GhostedDoretoCategory.GhostingMods, "Main rig visible to others", false),
            ["Ghost Rig Transparent"] = new("Ghost Rig Transparent", GhostedDoretoCategory.GhostingMods, "Ghost copy stays transparent", false),
            ["No Name"] = new("No Name", GhostedDoretoCategory.GhostingMods, "Hide your nameplate", false),
            ["No Texture"] = new("No Texture", GhostedDoretoCategory.GhostingMods, "Remove avatar texture", false),

            // Combat Mods
            ["Kick Gun"] = new("Kick Gun", GhostedDoretoCategory.CombatMods, "Projectile kick interaction", false),
            ["Kick All"] = new("Kick All", GhostedDoretoCategory.CombatMods, "Kick all nearby players", false),
            ["Kick Stump"] = new("Kick Stump", GhostedDoretoCategory.CombatMods, "Kick stump anchor", false),
            ["Grab Player Info"] = new("Grab Player Info", GhostedDoretoCategory.CombatMods, "Display player data", false),

            // Movement Mods
            ["No Finger Movement"] = new("No Finger Movement", GhostedDoretoCategory.MovementMods, "Disable finger animation", false),
            ["Platforms"] = new("Platforms", GhostedDoretoCategory.MovementMods, "Create temporary platforms", false),
            ["No Clip"] = new("No Clip", GhostedDoretoCategory.MovementMods, "Walk through walls", false),
            ["Speed Boost"] = new("Speed Boost", GhostedDoretoCategory.MovementMods, "Boost movement speed", false),
            ["WASD Walk"] = new("WASD Walk", GhostedDoretoCategory.MovementMods, "Keyboard movement", false),
            ["Smooth Movement"] = new("Smooth Movement", GhostedDoretoCategory.MovementMods, "Smooth motion locally", false),

            // Visual Mods
            ["Ghost Trail"] = new("Ghost Trail", GhostedDoretoCategory.VisualMods, "Add trailing ghost effect", false),
            ["Floating Aura"] = new("Floating Aura", GhostedDoretoCategory.VisualMods, "Glow around avatar", false),
            ["Spectral Fog"] = new("Spectral Fog", GhostedDoretoCategory.VisualMods, "Fog effect around player", false),
            ["Ethereal Bloom"] = new("Ethereal Bloom", GhostedDoretoCategory.VisualMods, "Energy bloom effect", false),
            ["Phantom Outline"] = new("Phantom Outline", GhostedDoretoCategory.VisualMods, "Phantom outline on body", false),

            // Utility Mods
            ["Hidden on Leaderboard"] = new("Hidden on Leaderboard", GhostedDoretoCategory.UtilityMods, "Hide from leaderboard", false),
            ["Config Lock"] = new("Config Lock", GhostedDoretoCategory.UtilityMods, "Lock configuration", false),
            ["Clear All"] = new("Clear All", GhostedDoretoCategory.UtilityMods, "Disable all mods", false),
            ["Reset Menu"] = new("Reset Menu", GhostedDoretoCategory.UtilityMods, "Reset to default", false),
            ["Save Profile"] = new("Save Profile", GhostedDoretoCategory.UtilityMods, "Save current preset", false),

            // World Mods
            ["Haunted Sky"] = new("Haunted Sky", GhostedDoretoCategory.WorldMods, "Haunted sky palette", false),
            ["Fog Density"] = new("Fog Density", GhostedDoretoCategory.WorldMods, "Increase fog density", false),
            ["Night Bloom"] = new("Night Bloom", GhostedDoretoCategory.WorldMods, "Night lighting effect", false),
            ["Moon Glow"] = new("Moon Glow", GhostedDoretoCategory.WorldMods, "Moon-like lighting", false),
            ["Misty Ambience"] = new("Misty Ambience", GhostedDoretoCategory.WorldMods, "Misty atmosphere", false)
        };
    }

    public void SetCurrentCategory(int categoryIndex)
    {
        var categories = Enum.GetValues(typeof(GhostedDoretoCategory)).Cast<GhostedDoretoCategory>().ToList();
        if (categoryIndex >= 0 && categoryIndex < categories.Count)
        {
            _currentCategory = categories[categoryIndex];
        }
    }

    public int GetCategoryCount()
    {
        return Enum.GetValues(typeof(GhostedDoretoCategory)).Length;
    }

    public string GetCategoryName(int index)
    {
        var categories = Enum.GetValues(typeof(GhostedDoretoCategory)).Cast<GhostedDoretoCategory>().ToList();
        return categories[index].ToString();
    }

    public List<GhostedDoretoFeature> GetCurrentCategoryFeatures()
    {
        return _features.Values.Where(f => f.Category == _currentCategory).ToList();
    }

    public int GetCurrentCategoryFeatureCount()
    {
        return GetCurrentCategoryFeatures().Count;
    }

    public void ToggleFeatureAtIndex(int index)
    {
        var features = GetCurrentCategoryFeatures();
        if (index >= 0 && index < features.Count)
        {
            features[index].Enabled = !features[index].Enabled;
        }
    }

    public void Toggle(string featureName)
    {
        if (_features.TryGetValue(featureName, out var feature))
        {
            feature.Enabled = !feature.Enabled;
        }
    }

    public List<GhostedDoretoFeature> GetEnabledFeatures()
    {
        return _features.Values.Where(f => f.Enabled).ToList();
    }

    public bool IsEnabled(string featureName)
    {
        return _features.TryGetValue(featureName, out var feature) && feature.Enabled;
    }
}
