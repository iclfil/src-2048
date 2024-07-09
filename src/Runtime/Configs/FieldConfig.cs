using System.Collections.Generic;
using Markins.Runtime.Game;
using Sirenix.OdinInspector;
using UnityEngine;

    //данные который не будут менять в течении игры, статичные данные, их можно брать из конфигов и они могут быть моделями в контроллерах.
    //если данные могут меняться, то надо делать отдельную модель. И так же зависит от размера логики, если логики немного, то можно закидывать в контроллер(и логика
    //не планирует меняться.
    //  

namespace Markins.Runtime.Game.Configs
{
    [CreateAssetMenu(fileName = "FieldConfig", menuName = "Game/Configs/Create Field Config")]
    public class FieldConfig : SerializedScriptableObject
    {
        public string DefaultNameView;

        public ChipCollection ChipCollection;

        public Dictionary<string, FieldView> ViewPrefabs = new();

        public FieldView GetPrefabFieldView(string nameView)
        {
            if (ViewPrefabs.TryGetValue(nameView, out var view))
            {
                return view;
            }
            else
            {
                Debug.Log($"No GameTheme With name: {nameView}");
                return ViewPrefabs[DefaultNameView];
            }
        }
    }
}
