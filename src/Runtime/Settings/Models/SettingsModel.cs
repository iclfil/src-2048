using System;
using Lean.Localization;

namespace Markins.Runtime.Game.Models
{
    [Serializable]
    public class SettingsModel
    {
        public bool Haptic { get; private set; } = true;
        public bool Sound { get; private set; } = true;
        public TypeLanguage Lang { get; private set; }
        public bool Shadows { get; set; } = false;

        public Action OnHapticChanged;
        public Action OnSoundChanged;
        public Action OnLanguageChanged;
        public Action OnShadowsChanged;


        public enum TypeLanguage
        {
            None = 0,
            Russian,
            English,
            Turkish,
        }

        private LeanLocalization _localization;
        private AudioSystem _audioSystem;

        public void Init(LeanLocalization leanLocalization, AudioSystem audioSystem)
        {
            _localization = leanLocalization;
            _audioSystem = audioSystem;
        
            SetLanguage(TypeLanguage.Russian);
            SetShadows(false);
            SetSound(false);
            SetHaptic(false);
        }

        public void SwitchShadows()
        {
            SetShadows(!Shadows);
        }

        public void SwitchLanguage(TypeLanguage language)
        {
            _localization.CurrentLanguage = language.ToString();
            SetLanguage(language);
        }

        public void SwitchSound()
        {
            SetSound(!Sound);
           
        }

        public void SwitchHaptic()
        {
            SetHaptic(!Haptic);
        }

        private void SetHaptic(bool value)
        {
            Haptic = value;
            OnHapticChanged?.Invoke();
        }

        private void SetShadows(bool value)
        {
            Shadows = value;
            OnShadowsChanged?.Invoke();
        }

        private void SetSound(bool value)
        {
            Sound = value;
            _audioSystem.SetSoundMuted(!Sound);
            OnSoundChanged?.Invoke();
        }

        private void SetLanguage(TypeLanguage language)
        {
            Lang = language;
            OnLanguageChanged?.Invoke();
        }


    }
}


