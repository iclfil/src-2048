using System;
using System.Collections;
using Markins.Runtime.Game.Storage.Models;
using Markins.Runtime.Game.Storage.Views;
using Markins.Runtime.Game.Utils;
using Supyrb;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Markins.Runtime.Game.Storage
{
    public class PlayEffectSignal : Signal<EffectCollection.EffectsEvent, Vector3>
    {

    }

    public class EffectsManager : Singleton<EffectsManager>
    {
        [SerializeField] private EffectCollection _config;
        [SerializeField] private EffectsView _view;

        [SerializeField] private string _effectSkin;

        public void Awake()
        {
            Signals.Get<OnSelectEffectSkinChangedSignal>().AddListener(OnChangeCurrentEffectSkin);
            Signals.Get<PlayEffectSignal>().AddListener(Play);
        }

        private void OnChangeCurrentEffectSkin(string nameEffect)
        {
            _effectSkin = nameEffect;
        }

        public IEnumerator Init(EffectCollection config)
        {
            _config = config;
            yield break;
        }

        public IEnumerator Free()
        {

            yield return null;
        }

        public void Play(EffectCollection.EffectsEvent eventName, Vector3 position)
        {
            if (eventName == EffectCollection.EffectsEvent.MATCH_CHIPS)
            {
                var effectConfig = _config.GetEffectByNameEvent(EffectCollection.EffectsEvent.MATCH_CHIPS, _effectSkin);

                var effect = Instantiate(effectConfig.EffectPrefab, _view.transform);
                effect.transform.position = position + effectConfig.SpawnOffset;
                Destroy(effect, effectConfig.LifeTime);
            }

            if (eventName == EffectCollection.EffectsEvent.EXPLODE_TARGET_CHIP)
            {
                var effectConfig = _config.GetEffectByNameEvent(eventName, String.Empty);
                var effect = Instantiate(effectConfig.EffectPrefab, _view.transform);
                effect.transform.position = position + effectConfig.SpawnOffset;

                if (effectConfig.LifeTime > 0)
                    Destroy(effect, effectConfig.LifeTime);
            }
        }

        public void Play(EffectCollection.EffectsEvent eventName, Transform target)
        {
            if (eventName == EffectCollection.EffectsEvent.SELECT_TARGET_CHIP)
            {
                var effectConfig = _config.GetEffectByNameEvent(eventName, String.Empty);
                var effect = Instantiate(effectConfig.EffectPrefab, _view.transform);
                effect.transform.position = target.transform.position + effectConfig.SpawnOffset;

                if (effectConfig.AttachToObject)
                    effect.transform.SetParent(target);

                if (effectConfig.LifeTime > 0)
                    Destroy(effect, effectConfig.LifeTime);
            }
        }
    }
}