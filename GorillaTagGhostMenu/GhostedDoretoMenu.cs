using System;
using System.Collections.Generic;
using System.Linq;

namespace GorillaTagGhostMenu;

public enum OculusButton
{
    A,
    B,
    X,
    Y,
    LeftGrip,
    RightGrip,
    LeftTrigger,
    RightTrigger,
    LeftThumbstick,
    RightThumbstick,
    LeftThumbstickClick,
    RightThumbstickClick,
    MenuButton,
    StartButton
}

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
    public OculusButton? Hotkey { get; set; }
    public bool Enabled { get; set; }

    public GhostedDoretoFeature(string name, GhostedDoretoCategory category, string description, OculusButton? hotkey = null, bool enabled = false)
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
    private readonly Dictionary<OculusButton, string> _hotkeyBindings;
    
    public bool MenuOpen { get; set; }
    private int _selectedIndex = 0;
    private GhostedDoretoCategory _currentCategory = GhostedDoretoCategory.Ghosting;

    public GhostedDoretoMenu()
    {
        _features = new Dictionary<string, GhostedDoretoFeature>(StringComparer.OrdinalIgnoreCase)
        {
            // Ghosting category
            ["Ghost Monke"] = new("Ghost Monke", GhostedDoretoCategory.Ghosting,
                "Visible rig + transparent ghost rig for local visual control.", OculusButton.A, false),
            ["Ghost Rig Visible"] = new("Ghost Rig Visible", GhostedDoretoCategory.Ghosting,
                "Main rig visible to others, ghost copy stays transparent.", OculusButton.X, false),
            ["Ghost Rig Transparent"] = new("Ghost Rig Transparent", GhostedDoretoCategory.Ghosting,
                "Keeps ghost copy partially transparent for spectral effect.", OculusButton.Y, false),

            // Combat category
            ["Kick Gun"] = new("Kick Gun", GhostedDoretoCategory.Combat,
                "Projectile-based kick interaction for targets.", OculusButton.RightTrigger, false),
            ["Kick All"] = new("Kick All", GhostedDoretoCategory.Combat,
                "Kick all nearby players (local environment only).", OculusButton.RightGrip, false),
            ["Kick Stump"] = new("Kick Stump", GhostedDoretoCategory.Combat,
                "Specialized kick for anchor/stump location.", OculusButton.B, false),

            // Movement category
            ["No Finger Movement"] = new("No Finger Movement", GhostedDoretoCategory.Movement,
                "Disables finger animation.", OculusButton.LeftThumbstickClick, false),
            ["Platforms"] = new("Platforms", GhostedDoretoCategory.Movement,
                "Creates temporary platforms using grip input.", OculusButton.LeftGrip, false),
            ["No Clip"] = new("No Clip", GhostedDoretoCategory.Movement,
                "Free local movement through solid geometry.", OculusButton.LeftTrigger, false),
            ["Speed Boost"] = new("Speed Boost", GhostedDoretoCategory.Movement,
                "Boosts movement speed locally.", OculusButton.RightThumbstickClick, false),
            ["WASD Walk"] = new("WASD Walk", GhostedDoretoCategory.Movement,
                "Keyboard-style movement input (local only).", OculusButton.MenuButton, false),

            // Visuals category
            ["No Name"] = new("No Name", GhostedDoretoCategory.Visuals,
                "Hides local player nameplate.", null, false),
            ["No Texture"] = new("No Texture", GhostedDoretoCategory.Visuals,
                "Disables avatar texture display locally.", null, false),

            // Utility category
            ["Hidden on Leaderboard"] = new("Hidden on Leaderboard", GhostedDoretoCategory.Utility,
                "Suppresses leaderboard visibility (local only).", null, false),

            // Info category
            ["Grab Player Info"] = new("Grab Player Info", GhostedDoretoCategory.Info,
                "Display live player data for local viewing.", OculusButton.StartButton, false)
        };

        // Build hotkey bindings dictionary
        _hotkeyBindings = new Dictionary<OculusButton, string>();
        foreach (var feature in _features.Values)
        {
            if (feature.Hotkey.HasValue && !_hotkeyBindings.ContainsKey(feature.Hotkey.Value))
            {
                _hotkeyBindings[feature.Hotkey.Value] = feature.Name;
            }
        }
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

    public IEnumerable<GhostedDoretoFeature> GetCategoryFeatures(GhostedDoretoCategory category)
    {
        return _features.Values.Where(f => f.Category == category);
    }

    public string? GetFeatureByHotkey(OculusButton button)
    {
        return _hotkeyBindings.TryGetValue(button, out var featureName) ? featureName : null;
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
            if (_features.TryGetValue(name, out var feature))
                feature.Enabled = true;
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
            if (_features.TryGetValue(name, out var feature))
                feature.Enabled = true;
        }
    }

    public void ClearAll()
    {
        foreach (var feature in _features.Values)
            feature.Enabled = false;
    }

    public void OpenMenu()
    {
        MenuOpen = true;
        _selectedIndex = 0;
    }

    public void CloseMenu()
    {
        MenuOpen = false;
    }

    public void NavigateUp()
    {
        if (_selectedIndex > 0)
            _selectedIndex--;
    }

    public void NavigateDown()
    {
        var categoryFeatures = GetCategoryFeatures(_currentCategory).ToList();
        if (_selectedIndex < categoryFeatures.Count - 1)
            _selectedIndex++;
    }

    public void SelectFeature()
    {
        var categoryFeatures = GetCategoryFeatures(_currentCategory).ToList();
        if (_selectedIndex >= 0 && _selectedIndex < categoryFeatures.Count)
        {
            Toggle(categoryFeatures[_selectedIndex].Name);
        }
    }

    public void SwitchCategory(GhostedDoretoCategory category)
    {
        _currentCategory = category;
        _selectedIndex = 0;
    }

    public void CycleCategory(bool forward)
    {
        var categories = Enum.GetValues(typeof(GhostedDoretoCategory)).Cast<GhostedDoretoCategory>().ToList();
        int currentIndex = categories.IndexOf(_currentCategory);
        
        if (forward)
            currentIndex = (currentIndex + 1) % categories.Count;
        else
            currentIndex = (currentIndex - 1 + categories.Count) % categories.Count;

        SwitchCategory(categories[currentIndex]);
    }

    public GhostedDoretoCategory CurrentCategory => _currentCategory;
    public int SelectedIndex => _selectedIndex;
}

public static class GhostedDoretoMenuUI
{
    public static void RenderMenu(GhostedDoretoMenu menu)
    {
        if (!menu.MenuOpen)
            return;

        Console.Clear();
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║            GHOSTED DORETO - OCULUS CONTROLLER MENU              ║");
        Console.WriteLine("║             Press RightThumbstickClick to Toggle Menu           ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        // Category tabs with Oculus button mapping
        Console.Write("Categories: ");
        foreach (var category in Enum.GetValues(typeof(GhostedDoretoCategory)).Cast<GhostedDoretoCategory>())
        {
            if (menu.CurrentCategory == category)
                Console.Write($"[> {category} <] ");
            else
                Console.Write($"  {category}   ");
        }
        Console.WriteLine("\n");

        // Features list
        Console.WriteLine($"┌─ {menu.CurrentCategory} Features ─────────────────────────────────────────┐");
        var categoryFeatures = menu.GetCategoryFeatures(menu.CurrentCategory).ToList();
        
        for (int i = 0; i < categoryFeatures.Count; i++)
        {
            var feature = categoryFeatures[i];
            var isSelected = (i == menu.SelectedIndex);
            var isEnabled = feature.Enabled ? "✓" : " ";
            var marker = isSelected ? ">>>" : "   ";
            var hotkeyStr = feature.Hotkey.HasValue ? $"({feature.Hotkey.Value})" : "(No Hotkey)";

            Console.ForegroundColor = isSelected ? ConsoleColor.Cyan : ConsoleColor.White;
            Console.WriteLine($"│ {marker} [{isEnabled}] {feature.Name,-25} {hotkeyStr}");
            Console.ResetColor();
        }
        Console.WriteLine("└─────────────────────────────────────────────────────────────────┘");

        Console.WriteLine("\nEnabled Features:");
        var enabled = menu.GetEnabledFeatures().ToList();
        if (enabled.Count == 0)
            Console.WriteLine("  (None)");
        else
            foreach (var feature in enabled)
                Console.WriteLine($"  ✓ {feature.Name}");

        Console.WriteLine("\n[OCULUS CONTROLLER LAYOUT]");
        Console.WriteLine("  RightThumbstickClick - Toggle Menu  |  Left/Right Thumbstick - Navigate");
        Console.WriteLine("  RightTrigger/Grip/A/B/X/Y - Quick Toggle Features  |  MenuButton - Close");
    }

    public static void RenderMenuConsole(GhostedDoretoMenu menu)
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        GHOSTED DORETO - OCULUS CONTROLLER MOD MENU              ║");
        Console.WriteLine("║                    (Local-Only Prototype)                       ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("BUTTON MAPPINGS:");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  RightThumbstickClick - Open/Close Menu");
        Console.WriteLine("  Left/Right Thumbstick Up/Down - Navigate Menu");
        Console.WriteLine("  A Button - Toggle Ghost Monke");
        Console.WriteLine("  X Button - Toggle Ghost Rig Visible");
        Console.WriteLine("  Y Button - Toggle Ghost Rig Transparent");
        Console.WriteLine("  B Button - Toggle Kick Stump");
        Console.WriteLine("  Right Trigger - Toggle Kick Gun");
        Console.WriteLine("  Right Grip - Toggle Kick All");
        Console.WriteLine("  Left Trigger - Toggle No Clip");
        Console.WriteLine("  Left Grip - Toggle Platforms");
        Console.WriteLine("  Left Thumbstick Click - Toggle No Finger Movement");
        Console.WriteLine("  Right Thumbstick Click - Toggle Speed Boost");
        Console.WriteLine("  Start Button - Grab Player Info");
        Console.WriteLine("  Menu Button - Back/Close");
        Console.ResetColor();

        Console.WriteLine("\n" + new string('─', 66) + "\n");

        foreach (var group in Enum.GetValues(typeof(GhostedDoretoCategory)).Cast<GhostedDoretoCategory>())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{group}]");
            Console.ResetColor();
            
            foreach (var feature in menu.GetCategoryFeatures(group))
            {
                var status = feature.Enabled ? "✓" : " ";
                var hotkeyInfo = feature.Hotkey.HasValue ? $"Hotkey: {feature.Hotkey.Value}" : "No Hotkey";
                Console.WriteLine($"  [{status}] {feature.Name}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"      └─ {feature.Description} ({hotkeyInfo})");
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }
}
