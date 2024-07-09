using System.Collections.Generic;
using Markins.Runtime.Game;
using Markins.Runtime.Game.Controllers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.markins._2048.Runtime.Configs
{
    [CreateAssetMenu(fileName = "FieldCollection", menuName = "Game/Field/Creat FieldCollection")]
    public class FieldCollection : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<string, FieldView> _fields = new();

        public FieldView GetFieldViewPrefab(string fieldName)
        {
            return _fields[fieldName];
        }
    }
}