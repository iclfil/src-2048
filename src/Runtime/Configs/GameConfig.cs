using System.Collections.Generic;
using Assets.markins.Game.Runtime.Game;
using Markins.Runtime.Game;
using Markins.Runtime.Game.Configs;
using Markins.Runtime.Game.Models;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Create GameConfig")]
public class GameConfig : SerializedScriptableObject
{
    [SerializeField] private string _defaultView = "Blue";
    [SerializeField] private Dictionary<string, GameView> _views;
    [SerializeField] private Dictionary<string, GameModel> _models;

    [SerializeField] private FieldConfig _fieldConfig;
    [SerializeField] private ChipCollection _chipCollection;

    public FieldConfig FieldConfig => _fieldConfig;
    public ChipCollection ChipCollection => _chipCollection;
    public int MoneyForCrystal;

    public GameModel GetModel(string nameModel = "default")
    {
        if (_models.TryGetValue(nameModel, out var model))
        {
            return model;
        }

        return null;
    }

    public SettingsModel CloneSettingsModelByName()
    {
        return new SettingsModel();
    }

    public GameView GetGameTheme(string nameTheme)
    {
        if (_views.TryGetValue(nameTheme, out var view))
        {
            return view;
        }
        else
        {
            Debug.Log($"No GameTheme With name: {nameTheme}");
            return _views[_defaultView];
        }
    }
}
