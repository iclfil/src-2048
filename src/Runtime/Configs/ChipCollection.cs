using System;
using System.Collections.Generic;
using System.IO;
using Assets.markins._2048.Runtime.Configs;
using Markins.Runtime.Game;
using Markins.Runtime.Game.Controllers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "ChipCollection", menuName = "Game/Field/Create ChipCollection")]
public class ChipCollection : SerializedScriptableObject
{
    [SerializeField] private string _nameDefaultChip = "Circle";
    [SerializeField] private string _nameDefaultChipView = "Circle";

    [SerializeField] private Dictionary<int, ChipConfig> _chipsConfigs = new();

    [SerializeField] private Dictionary<string, ChipController> _chips = new();

    [SerializeField] private Dictionary<string, ChipView> _views = new();

    public ChipConfig GetChipConfig(int power)
    {
        return _chipsConfigs[power];
    }

    public ChipView GetChipViewByName(string nameChip)
    {
        if (string.IsNullOrEmpty(nameChip))
            nameChip = _nameDefaultChipView;

        if (_views.TryGetValue(nameChip, out var chip))
        {
            return chip;
        }

        return _views[_nameDefaultChipView];
    }

    public ChipController GetChipByName(string nameChip)
    {
        if (string.IsNullOrEmpty(nameChip))
            nameChip = _nameDefaultChip;

        if (_chips.TryGetValue(nameChip, out var chip))
        {
            return chip;
        }

        return _chips[_nameDefaultChip];
    }

    #region UnitEditor

#if UNITY_EDITOR

    [ContextMenu("ReadAndSetColors")]
    private void ReadAndSetColors()
    {
        string path = "Assets/markins.2048/Configs/Field/Chips/palette_2.json";
        string json = File.ReadAllText(path);

        Dictionary<string, string> colorDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);


        List<Color> colorsList = new List<Color>();
        foreach (var item in colorDict)
        {
            if (ColorUtility.TryParseHtmlString(item.Value, out Color newColor))
            {
                colorsList.Add(newColor);
            }
        }

        Debug.Log("ALL COLORS" + colorsList.Count);

        var index = 0;

        foreach (var chipConfig in _chipsConfigs)
        {
            chipConfig.Value.SetColor(colorsList[index]);
            index++;

        }


        Debug.Log("READ ALL COMPLETE");
    }

    [ContextMenu("GenerateSize")]

    private void GenerateSizeChip()
    {
        var size = new Vector3(0.72f, 1f, 0.72f);
        var stepSize = 0.037f;
        foreach (var config in _chipsConfigs)
        {
            config.Value.SetSize(size);
            var randNumber = UnityEngine.Random.Range(0f, 0.005f);
            stepSize += randNumber;
            size += new Vector3(stepSize, 0, stepSize);
            EditorUtility.SetDirty(config.Value);
        }
        // Сохранение ассетов и обновление базы данных ассетов
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [ContextMenu("Create Chips")]
    private void CreateChips()
    {
        var powerMax = 24;
        var size = new Vector3(0.5f, 0.5f, 0.5f);
        var stepSize = 0.015f;

        for (int p = 1; p < powerMax; p++)
        {
            var color = GetRandomColor();
            var numberOfPower = MathF.Pow(2, p);
            var nameConfig = $"Chip{p}";
            var chipConfig = ScriptableObject.CreateInstance<ChipConfig>();
            Debug.Log("NUMBER OF POWER:" + numberOfPower);
            chipConfig.SetNumberOfPower(numberOfPower);
            chipConfig.SetPower(p);
            chipConfig.SetColor(color);
            chipConfig.SetSize(size);
            chipConfig.SetName(nameConfig);
            chipConfig.name = $"Chip{numberOfPower}";

            size.x += stepSize;
            size.z += stepSize;

            _chipsConfigs.Add(p, chipConfig);

            SOUtility.SaveAsset(chipConfig, "markins.2048/Configs/Field/Chips/Configs/" + chipConfig.name);
        }
    }
    private Random _random = new Random();

    private Color GetRandomColor()
    {
        // Генерация случайных значений для компонентов цвета
        int r = _random.Next(256); // Красный (0-255)
        int g = _random.Next(256); // Зеленый (0-255)
        int b = _random.Next(256); // Синий (0-255)

        // Создание и возврат цвета
        return new Color((float)1 / r, (float)1 / g, (float)1 / b);
    }

#endif
    #endregion

}
