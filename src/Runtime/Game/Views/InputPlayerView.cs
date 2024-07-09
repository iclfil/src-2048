using System;
using UnityEngine;

namespace Markins.Runtime.Game.Views
{
    public class InputPlayerView : MonoBehaviour
    {
        public Action<FingerDownEvent> OnDown;
        public Action<FingerUpEvent> OnUp;
        public Action<Vector2> OnFingerMoving;
        public bool Debug = false;
        public ScreenRaycaster Raycaster;

        public void Init()
        {
            Raycaster.Cameras[0] = Camera.main;
            FingerGestures.OnFingerEvent += Finger;
        }

        private void Finger(FingerEvent eventdata)
        {
            if (eventdata.Finger.Phase == FingerGestures.FingerPhase.Moving)
            {
               
            }
        }

        public void FingerDown(FingerDownEvent eventData)
        {
            OnDown?.Invoke(eventData);
        }
        public void FingerUp(FingerUpEvent eventData)
        {
            OnUp?.Invoke(eventData);
        }

        public void FingerMove(FingerGestures.Finger finger)
        {
            OnFingerMoving?.Invoke(finger.Position);
        }
    }
}

