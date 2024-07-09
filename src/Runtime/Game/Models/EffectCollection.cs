using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Markins.Runtime.Game.Storage.Models
{
    [CreateAssetMenu(fileName = "EffectCollection", menuName = "Game/Effects/Create EffectCollection")]
    public class EffectCollection : SerializedScriptableObject
    {
        public enum EffectsEvent
        {
            None = 0,
            MATCH_CHIPS = 1,
            SELECT_TARGET_CHIP = 2,
            EXPLODE_TARGET_CHIP = 3
        }
        [SerializeField] private Dictionary<EffectsEvent, List<EffectConfig>> Effects;

        public EffectConfig GetEffectByNameEvent(EffectsEvent eventName, string nameEffect)
        {
            var effects = Effects[eventName];
            if (effects == null)
            {
                Debug.LogError("No Effects By Event" + eventName);
                return null;
            }
            //TODO - fix check null - return default
            return effects.Find(x => x.Name == nameEffect);
        }
    }
}