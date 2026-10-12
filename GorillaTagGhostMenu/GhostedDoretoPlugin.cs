using BepInEx;
using UnityEngine;
using System.Collections.Generic;

namespace GhostedDoreto;

[BepInPlugin("com.DorteoGTAG.GhostedDoreto", "Ghosted Doreto", "1.0.0")]
public class GhostedDoretoPlugin : BaseUnityPlugin
{
    private GhostedDoretoMenuController _menuController;
    private bool _menuOpen = false;
    private int _selectedCategoryIndex = 0;
    private int _selectedFeatureIndex = 0;

    private void Awake()
    {
        _menuController = new GhostedDoretoMenuController();
        Logger.LogInfo("Ghosted Doreto menu initialized!");
    }

    private void Update()
    {
        // Hold Y button to toggle menu
        if (Input.GetKey(KeyCode.Y))
        {
            _menuOpen = true;
        }

        if (Input.GetKeyUp(KeyCode.Y))
        {
            _menuOpen = false;
        }

        if (!_menuOpen)
            return;

        // Menu navigation
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _selectedFeatureIndex--;
            if (_selectedFeatureIndex < 0)
                _selectedFeatureIndex = _menuController.GetCurrentCategoryFeatureCount() - 1;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _selectedFeatureIndex++;
            if (_selectedFeatureIndex >= _menuController.GetCurrentCategoryFeatureCount())
                _selectedFeatureIndex = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _selectedCategoryIndex--;
            if (_selectedCategoryIndex < 0)
                _selectedCategoryIndex = _menuController.GetCategoryCount() - 1;
            _menuController.SetCurrentCategory(_selectedCategoryIndex);
            _selectedFeatureIndex = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _selectedCategoryIndex++;
            if (_selectedCategoryIndex >= _menuController.GetCategoryCount())
                _selectedCategoryIndex = 0;
            _menuController.SetCurrentCategory(_selectedCategoryIndex);
            _selectedFeatureIndex = 0;
        }

        // Toggle selected feature
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            _menuController.ToggleFeatureAtIndex(_selectedFeatureIndex);
        }
    }

    private void OnGUI()
    {
        if (!_menuOpen)
            return;

        GUILayout.BeginArea(new Rect(50, 50, 400, 600));
        GUILayout.Box("GHOSTED DORETO", GUILayout.ExpandWidth(true));
        
        // Category tabs
        GUILayout.BeginHorizontal();
        for (int i = 0; i < _menuController.GetCategoryCount(); i++)
        {
            var style = i == _selectedCategoryIndex ? GUI.skin.button : GUI.skin.label;
            if (GUILayout.Button(_menuController.GetCategoryName(i), style))
            {
                _selectedCategoryIndex = i;
                _menuController.SetCurrentCategory(i);
                _selectedFeatureIndex = 0;
            }
        }
        GUILayout.EndHorizontal();

        // Features list
        GUILayout.Box($"Features - Hold Y to toggle", GUILayout.ExpandWidth(true));
        var features = _menuController.GetCurrentCategoryFeatures();
        for (int i = 0; i < features.Count; i++)
        {
            var feature = features[i];
            var style = i == _selectedFeatureIndex ? GUI.skin.box : GUI.skin.label;
            var status = feature.Enabled ? "[✓]" : "[ ]";
            
            GUILayout.Label($"{status} {feature.Name}", style);
        }

        GUILayout.Label("Enabled Features:");
        var enabledFeatures = _menuController.GetEnabledFeatures();
        foreach (var feature in enabledFeatures)
        {
            GUILayout.Label($"  ✓ {feature.Name}");
        }

        GUILayout.EndArea();
    }
}
