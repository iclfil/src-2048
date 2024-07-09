using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Widgets
{
    public class SettingsButton : MonoBehaviour
    {
        public Button button;
        public Sprite StateOn;
        public Sprite StateOff;

        private bool enable = true;

        public void Enable(bool value)
        {
            button.image.sprite = value == false ? StateOff : StateOn;
            enable = value;
        }

        public void SwitchState()
        {
            Enable(!enable);
        }
    }
}