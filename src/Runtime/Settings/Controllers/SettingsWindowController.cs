using Assets.Scripts.UI.Widgets;
using deVoid.UIFramework;
using Markins.Runtime.Game.Models;
using Markins.Runtime.Game.Settings.MySignals;
using Supyrb;
using UnityEngine;
using UnityEngine.UI;

namespace Markins.Runtime.Game.GUI.Screens
{
    public class SettingsWindowController : AWindowController
    {
        [SerializeField] private SettingsButton _buttonSound;
        [SerializeField] private SettingsButton _buttonHaptic;
        [SerializeField] private SettingsButton _buttonShadows;
        [Space]
        [SerializeField] private Button _buttonEnglish;
        [SerializeField] private Button _buttonRussian;
        [SerializeField] private Button _buttonTurkish;
        [Space]
        [SerializeField] private Button _buttonClose;

        private OnClickSoundButtonSignal _soundButtonSignal;
        private OnClickHapticButtonSignal _hapticButtonSignal;
        private OnClickShadowButtonSignal _shadowButtonSignal;
        private OnClickLanguageButtonSignal _languageButtonSignal;

        private void ChangeLanguageHandler(SettingsModel.TypeLanguage language)
        {
            Debug.Log("Fix Connect Settings Controller with View Button language");//TODO
        }

        private void ChangeHapticHandler(bool value)
        {
            _buttonHaptic.Enable(value);
        }

        private void ChangeSoundHandler(bool value)
        {
            _buttonSound.Enable(value);
        }

        private void ChangeShadowHandler(bool value)
        {
            _buttonShadows.Enable(value);
        }

        protected override void AddListeners()
        {
            _buttonEnglish.onClick.AddListener(ClickEnglishButton);
            _buttonRussian.onClick.AddListener(ClickRussianButton);
            _buttonTurkish.onClick.AddListener(ClickTurkishButton);
            _buttonClose.onClick.AddListener(ClickCloseButton);

            _shadowButtonSignal = Signals.Get<OnClickShadowButtonSignal>();
            _soundButtonSignal = Signals.Get<OnClickSoundButtonSignal>();
            _hapticButtonSignal = Signals.Get<OnClickHapticButtonSignal>();
            _languageButtonSignal = Signals.Get<OnClickLanguageButtonSignal>();

            Signals.Get<ChangeSoundSignal>().AddListener(ChangeSoundHandler);
            Signals.Get<ChangeLanguageSignal>().AddListener(ChangeLanguageHandler);
            Signals.Get<ChangeHapticSignal>().AddListener(ChangeHapticHandler);
            Signals.Get<ChangeShadowSignal>().AddListener(ChangeShadowHandler);

            _buttonSound.button.onClick.AddListener(ClickSoundButton);
            _buttonHaptic.button.onClick.AddListener(ClickHapticButton);
            _buttonShadows.button.onClick.AddListener(ClickShadowsButton);
        }

        private void ClickShadowsButton()
        {
            Signals.Get<OnClickShadowButtonSignal>().Dispatch();
        }

        protected override void RemoveListeners()
        {
            _buttonSound.button.onClick.RemoveListener(ClickSoundButton);
            _buttonHaptic.button.onClick.RemoveListener(ClickHapticButton);
        }

        private void ClickCloseButton()
        {
            Debug.Log("CLICK CLOSE SETTINGS");
            Signals.Get<OnClickCloseSettings>().Dispatch();
        }

        private void ClickTurkishButton()
        {
            _languageButtonSignal.Dispatch(SettingsModel.TypeLanguage.Turkish);
        }

        private void ClickRussianButton()
        {
            _languageButtonSignal.Dispatch(SettingsModel.TypeLanguage.Russian);
        }

        private void ClickEnglishButton()
        {
            _languageButtonSignal.Dispatch(SettingsModel.TypeLanguage.English);
        }

        private void ClickSoundButton()
        {
            _soundButtonSignal.Dispatch();
        }

        private void ClickHapticButton()
        {
            _hapticButtonSignal.Dispatch();
        }
    }
}

