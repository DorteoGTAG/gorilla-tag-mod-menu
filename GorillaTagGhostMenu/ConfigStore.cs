using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GorillaTagGhostMenu;

public static class ConfigStore
{
    public static void Save(string configPath, ModMenuController controller)
    {
        controller.SaveToFile(configPath);
    }

    public static ModMenuController Load(string configPath)
    {
        var controller = new ModMenuController();
        controller.LoadFromFile(configPath);
        return controller;
    }

    public static string CreateDefaultConfigJson()
    {
        var preset = new List<FeatureDefinition>
        {
            new("Ghost Trail", "Visuals", "Adds a soft ghost-like trail behind the player model.", "F1", true),
            new("Floating Aura", "Visuals", "Creates a floating glow around the avatar.", "F2", true),
            new("Spectral Fog", "Visuals", "Adds creeping fog around the area.", "F5", true),
            new("Haunted Sky", "Environment", "Sets the sky to a haunted color palette.", "G1", true),
            new("Smooth Step", "Movement", "Increases movement smoothness locally.", "M1", true),
            new("Ghost Compass", "UI", "Adds a subtle directional overlay.", "U10", true),
            new("Save Profile", "Utility", "Stores current menu preset.", "Q15", true)
        };

        return JsonSerializer.Serialize(preset, new JsonSerializerOptions { WriteIndented = true });
    }
}
