using System;
using System.Collections.Generic;
using System.Linq;

namespace GorillaTagGhostMenu;

public enum GhostedDoretoCategory
{
    Ghosting,
    Combat,
    Movement,
    Visuals,
    Utility,
    Info
}

public sealed class GhostedDoretoFeature
{
    public string Name { get; set; } = string.Empty;
    public GhostedDoretoCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Hotkey { get; set; } = string.Empty;
    public bool Enabled { get; set; }

    public GhostedDoretoFeature(string name, GhostedDoretoCategory category, string description, string hotkey = "", bool enabled = false)
    {
        Name = name;
        Category = category;
        Description = description;
        Hotkey = hotkey;
        Enabled = enabled;
    }
}

public sealed class GhostedDoretoMenu
{
    private readonly Dictionary<string, GhostedDoretoFeature> _features;

    public GhostedDoretoMenu()
    {
        _features = new Dictionary<string, GhostedDoretoFeature>(StringComparer.OrdinalIgnoreCase)
        {
            ["Ghost Monke"] = new("Ghost Monke", GhostedDoretoCategory.Ghosting,
                "Creates a visible rig and a transparent ghost rig for local visual control.", "A", false),
            ["Kick Gun"] = new("Kick Gun", GhostedDoretoCategory.Combat,
                "Projectile/area kick interaction for nearby targets.", "K", false),
            ["Kick All"] = new("Kick All", GhostedDoretoCategory.Combat,
                "Kick all nearby players in a local testing environment.", "Shift+K", false),
            ["Kick Stump"] = new("Kick Stump", GhostedDoretoCategory.Combat,
                "Specialized kick behavior for a target stump or anchor location.", "Ctrl+K", false),
            ["No Finger Movement"] = new("No Finger Movement", GhostedDoretoCategory.Movement,
                "Disables finger movement animation.", "F", false),
            ["Platforms"] = new("Platforms", GhostedDoretoCategory.Movement,
                "Creates temporary platform surfaces using grip input.", "Grip", false),
            ["No Clip"] = new("No Clip", GhostedDoretoCategory.Movement,
                "Allows free local movement through solid geometry.", "Trigger", false),
            ["Speed Boost"] = new("Speed Boost", GhostedDoretoCategory.Movement,
                "Boosts normal movement speed locally.", "Shift+S", false),
            ["WASD Walk"] = new("WASD Walk", GhostedDoretoCategory.Movement,
                "Uses keyboard movement input for local movement.", "WASD", false),
            ["No Name"] = new("No Name", GhostedDoretoCategory.Visuals,
                "Hides local nameplate display.", "N", false),
            ["No Texture"] = new("No Texture", GhostedDoretoCategory.Visuals,
                "Disables avatar texture display locally.", "T", false),
            ["Hidden on Leaderboard"] = new("Hidden on Leaderboard", GhostedDoretoCategory.Utility,
                "Suppresses leaderboard visibility locally.", "L", false),
            ["Grab Player Info"] = new("Grab Player Info", GhostedDoretoCategory.Info,
                "Pulls and displays live player data for local analysis.", "I", false),
            ["Ghost Rig Visible"] = new("Ghost Rig Visible", GhostedDoretoCategory.Ghosting,
                "Makes the main rig visible to others while the ghost copy remains transparent.", "Alt+G", false),
            ["Ghost Rig Transparent"] = new("Ghost Rig Transparent", GhostedDoretoCategory.Ghosting,
                "Keeps the ghost copy partially transparent for a spectral effect.", "Alt+T", false)
        };
    }

    public IReadOnlyCollection<GhostedDoretoFeature> Features => _features.Values;

    public bool Toggle(string featureName)
    {
        if (!_features.TryGetValue(featureName, out var feature))
            throw new InvalidOperationException($"Feature not found: {featureName}");

        feature.Enabled = !feature.Enabled;
        return feature.Enabled;
    }

    public void SetEnabled(string featureName, bool enabled)
    {
        if (!_features.TryGetValue(featureName, out var feature))
            throw new InvalidOperationException($"Feature not found: {featureName}");

        feature.Enabled = enabled;
    }

    public bool IsEnabled(string featureName)
    {
        return _features.TryGetValue(featureName, out var feature) && feature.Enabled;
    }

    public IEnumerable<GhostedDoretoFeature> GetEnabledFeatures()
    {
        return _features.Values.Where(f => f.Enabled);
    }

    public void ApplyGhostPreset()
    {
        foreach (var feature in _features.Values)
            feature.Enabled = false;

        foreach (var name in new[]
                 {
                     "Ghost Monke",
                     "Ghost Rig Visible",
                     "Ghost Rig Transparent",
                     "No Name",
                     "No Texture",
                     "Hidden on Leaderboard",
                     "No Finger Movement",
                     "WASD Walk"
                 })
        {
            _features[name].Enabled = true;
        }
    }

    public void ApplyUtilityPreset()
    {
        foreach (var feature in _features.Values)
            feature.Enabled = false;

        foreach (var name in new[]
                 {
                     "Platforms",
                     "No Clip",
                     "Speed Boost",
                     "Grab Player Info",
                     "Hidden on Leaderboard"
                 })
        {
            _features[name].Enabled = true;
        }
    }

    public void ClearAll()
    {
        foreach (var feature in _features.Values)
            feature.Enabled = false;
    }
}

public static class GhostedDoretoMenuConsole
{
    public static void Render(GhostedDoretoMenu menu)
    {
        Console.Clear();
        Console.WriteLine("Ghosted Doreto - Local-Only Prototype Menu");
        Console.WriteLine("========================================");
        Console.WriteLine();

        foreach (var group in Enum.GetValues(typeof(GhostedDoretoCategory)).Cast<GhostedDoretoCategory>())
        {
            Console.WriteLine($"[{group}] ");
            foreach (var feature in menu.Features.Where(f => f.Category == group))
            {
                Console.WriteLine($"  {(feature.Enabled ? "[X]" : "[ ]")} {feature.Name} - {feature.Description}");
            }
            Console.WriteLine();
        }
    }
}
