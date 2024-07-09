using System.Collections.Generic;
using Markins.Runtime.Game.Storage.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Markins.Runtime.Game.Configs
{
    [CreateAssetMenu(fileName = "InventoryConfig", menuName = "Game/Player/Create InventoryConfig")]
    public class InventoryConfig : SerializedScriptableObject
    {
        [SerializeField] private List<EffectConfig> _openEffects;

        [SerializeField] private InventoryModel _inventoryModel;

        public InventoryModel PresetInventoryModel => _inventoryModel;
    }
}