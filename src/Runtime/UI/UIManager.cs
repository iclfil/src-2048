using deVoid.UIFramework;
using Markins.Runtime.Game.Utils;
using UnityEngine;

namespace Markins.Runtime.Game
{
    public class UIManager : Singleton<UIManager>
    {
        private UISettings _settings;
        public Camera UICamera => Frame.UICamera;
        public UIFrame Frame { get; private set; }

        public Canvas MessageCanvas;
        public MessageView MessageViewPrefab;

        public float timerMessage = 60;
        public bool isShowingTimer = false;

        public void Init(UISettings settings)
        {
            _settings = settings;
            Frame = _settings.CreateUIInstance();
        }

        public void ShowHalfFilledFieldMessage()
        {
            if (isShowingTimer)
                return;
            
            isShowingTimer = true;
            var message = Instantiate(MessageViewPrefab, MessageCanvas.transform);
            message.ShowAndHide();
            Destroy(message.gameObject,6);
        }

        private void Update()
        {
            if (isShowingTimer)
            {
                timerMessage-= Time.deltaTime;
                if (timerMessage <= 0)
                {
                    isShowingTimer = false;
                    timerMessage = 60;
                }
            }
        }

        public void ShowLoseWindow()
        {
            Frame.OpenWindow(ScreenIds.LostLevelWindow);
        }
    }
}

