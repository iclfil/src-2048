using Markins.Runtime.Game.GUI.MySignals;
using Markins.Runtime.Game.Models;
using Markins.Runtime.Game.Settings.MySignals;
using Markins.Runtime.Game.Storage;
using Markins.Runtime.Game.Utils;
using Supyrb;
using UnityEngine;

namespace Markins.Runtime.Game.Controllers
{
    public class SettingsController : Singleton<SettingsController>
    {
        private SettingsModel _model;
        [SerializeField]
        private Light DirectionalLight; //TODO SCENE CONTEXT

        public void Init(SettingsModel model)
        {
            _model = model;
            _model.Shadows = true;
            _model.OnLanguageChanged += LanguageChanged;
            _model.OnHapticChanged += HapticChanged;
            _model.OnSoundChanged += SoundChanged;
            _model.OnShadowsChanged += ShadowsChanged;
            //TODO
            //fromView

            Signals.Get<OnClickOpenSettingsSignal>().AddListener(Open);
            Signals.Get<OnClickCloseSettings>().AddListener(Close);
            Signals.Get<OnClickSoundButtonSignal>().AddListener(SoundHandler); ;
            Signals.Get<OnClickHapticButtonSignal>().AddListener(HapticHandler);
            Signals.Get<OnClickShadowButtonSignal>().AddListener(ShadowHandler);
            Signals.Get<OnClickLanguageButtonSignal>().AddListener(ChangeLanguageHandler);
        }

        private void ShadowsChanged()
        {
            if (_model.Shadows)
                DirectionalLight.shadows = LightShadows.Hard;
            else
                DirectionalLight.shadows = LightShadows.None;

            Signals.Get<ChangeShadowSignal>().Dispatch(_model.Shadows);
        }

        private void ShadowHandler()
        {
            _model.SwitchShadows();
        }

        private void HapticChanged()
        {
            Signals.Get<ChangeHapticSignal>().Dispatch(_model.Haptic);
        }

        private void SoundChanged()
        {
            Signals.Get<ChangeSoundSignal>().Dispatch(_model.Sound);
        }

        private void LanguageChanged()
        {
            Signals.Get<ChangeLanguageSignal>().Dispatch(_model.Lang);
        }

        private void ChangeLanguageHandler(SettingsModel.TypeLanguage language)
        {
            _model.SwitchLanguage(language);
        }

        private void HapticHandler()
        {
            _model.SwitchHaptic();
        }

        private void SoundHandler()
        {
            _model.SwitchSound();
        }

        public void Open()
        {
            Signals.Get<OnSettingsOpenedSignal>().Dispatch();
            UIManager.instance.Frame.OpenWindow(ScreenIds.SettingsWindow);
        }

        public void Close()
        {
            Signals.Get<OnSettingsClosedSignal>().Dispatch();
            UIManager.instance.Frame.CloseCurrentWindow();
        }
    }
}