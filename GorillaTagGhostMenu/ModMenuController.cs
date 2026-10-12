using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GorillaTagGhostMenu;

public sealed class ModMenuController
{
    private readonly Dictionary<string, FeatureDefinition> _features;
    private readonly List<string> _enabledFeatureNames = new();

    public ModMenuController()
    {
        _features = FeatureCatalog.All.ToDictionary(f => f.Name, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<string> EnabledFeatures => _enabledFeatureNames;

    public IEnumerable<FeatureDefinition> AllFeatures => _features.Values;

    public void Toggle(string featureName)
    {
        if (!_features.TryGetValue(featureName, out var feature))
            throw new InvalidOperationException($"Unknown feature: {featureName}");

        feature.Enabled = !feature.Enabled;
        if (feature.Enabled)
        {
            if (!_enabledFeatureNames.Contains(feature.Name, StringComparer.OrdinalIgnoreCase))
                _enabledFeatureNames.Add(feature.Name);
        }
        else
        {
            _enabledFeatureNames.RemoveAll(x => string.Equals(x, feature.Name, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void SetEnabled(string featureName, bool enabled)
    {
        if (!_features.TryGetValue(featureName, out var feature))
            throw new InvalidOperationException($"Unknown feature: {featureName}");

        feature.Enabled = enabled;
        if (feature.Enabled)
        {
            if (!_enabledFeatureNames.Contains(feature.Name, StringComparer.OrdinalIgnoreCase))
                _enabledFeatureNames.Add(feature.Name);
        }
        else
        {
            _enabledFeatureNames.RemoveAll(x => string.Equals(x, feature.Name, StringComparison.OrdinalIgnoreCase));
        }
    }

    public bool IsEnabled(string featureName)
    {
        if (!_features.TryGetValue(featureName, out var feature))
            return false;

        return feature.Enabled;
    }

    public IEnumerable<FeatureDefinition> GetEnabledFeatures()
    {
        return _features.Values.Where(f => f.Enabled);
    }

    public void ApplyPreset(string presetName)
    {
        if (string.Equals(presetName, "Spectator Ghost", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var name in FeatureCatalog.GetNames())
            {
                SetEnabled(name, false);
            }

            foreach (var name in new[]
                     {
                         "Ghost Trail",
                         "Floating Aura",
                         "Phantom Outline",
                         "Spectral Fog",
                         "Soft Shadow",
                         "Moon Glow",
                         "Misty Ambience",
                         "Smooth Step",
                         "Sidebar UI",
                         "HUD Toggle",
                         "Ghost Compass",
                         "Ethereal Filter",
                         "Ghost Ribbon",
                         "Echo Pulse",
                         "Prism Veil",
                         "Ghost Bloom"
                     })
            {
                SetEnabled(name, true);
            }

            return;
        }

        if (string.Equals(presetName, "Arena Night", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var name in FeatureCatalog.GetNames())
            {
                SetEnabled(name, false);
            }

            foreach (var name in new[]
                     {
                         "Haunted Sky",
                         "Misty Ambience",
                         "Night Bloom",
                         "Moon Glow",
                         "Fog Density +",
                         "Cloud Drift",
                         "Starfall Dust",
                         "Lunar Haze",
                         "Ghost Trail",
                         "Prism Luster",
                         "Minimal HUD",
                         "Ghost Compass"
                     })
            {
                SetEnabled(name, true);
            }

            return;
        }

        if (string.Equals(presetName, "Full Ghost", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var name in FeatureCatalog.GetNames())
            {
                SetEnabled(name, true);
            }

            return;
        }

        throw new InvalidOperationException($"Unknown preset: {presetName}");
    }

    public void Reset()
    {
        foreach (var feature in _features.Values)
        {
            feature.Enabled = false;
        }

        _enabledFeatureNames.Clear();
    }

    public void SaveToFile(string filePath)
    {
        var data = _features.Values
            .OrderBy(f => f.Category)
            .ThenBy(f => f.Name)
            .Select(f => new { f.Name, f.Category, f.Enabled, f.Hotkey, f.Description })
            .ToList();

        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");
        File.WriteAllText(filePath, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        var json = File.ReadAllText(filePath);
        var data = JsonSerializer.Deserialize<List<FeatureState>>(json);
        if (data is null)
            return;

        foreach (var state in data)
        {
            if (_features.TryGetValue(state.Name, out var feature))
            {
                feature.Enabled = state.Enabled;
            }
        }

        _enabledFeatureNames.Clear();
        foreach (var feature in _features.Values.Where(f => f.Enabled))
        {
            _enabledFeatureNames.Add(feature.Name);
        }
    }

    private sealed class FeatureState
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string Hotkey { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
