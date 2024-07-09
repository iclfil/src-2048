using System;
using Markins.Runtime.Game.Utils;
using Markins.Runtime.Game.Views;
using UnityEngine;

namespace Markins.Runtime.Game.Controllers
{
    /// <summary>
    /// Знает, что нажали на фишку.
    /// Сообщает GameController.
    /// GameController в зависимости от стейта игры в данный момент, говорит что делать Инпуту, или же передает данные в Бустер систем, и та обратывает их как нужно.
    /// Если обратно инпуту, то появляется прицел и игрок может стрельнуть фишкей.
    /// Если в бустер систем передается, то туда просто передается выбранная фишка.
    /// Создает прицел, крутит прицел, стреляет фишкой.
    /// </summary>
    public class ShootControlService : Singleton<ShootControlService>
    {
        public Action ShootChipAction;
        public Action ChipSelectedAction;
        public Action ItemDeselectedAction;


        public float ShootForce = 45;

        public Vector3 StartFingerPosition { get; private set; }
        public Vector3 CurrentFingerPosition { get; private set; }
        public Vector3 Delta { get; private set; }

        private InputPlayerView _view;
        private AimView _aimView;
        private float _angle = 0;

        public GameObject SelectableItem { get; private set; }

        public enum InputState
        {
            None,
            Ready,
            SelectChip,
            Disable,
            Aiming,
            Shooting
        }

        private InputState _state = InputState.Ready;

        public void Init(InputPlayerView viewPrefab, AimView aimViewPrefab)
        {
            _aimView = Instantiate(aimViewPrefab, transform);
            _aimView.gameObject.SetActive(false);

            //TODO
            _view = Instantiate(viewPrefab, transform);
            _view.Init();

            _view.OnDown = OnFingerDown;
            _view.OnUp = OnFingerUp;
            _view.OnFingerMoving = OnFingerMoving;
        }

        public void Tick()
        {
            if (SelectableItem == null)
            {
                TurnOffAim();
                _state = InputState.Ready;
            }

            if (_state == InputState.Aiming)
            {
                AttachAimToObject();
                RotateAim();
            }
        }

        public void Free()
        {
            ShootChipAction = null;
            ChipSelectedAction = null;
            ItemDeselectedAction = null;
            SelectableItem = null;
            Destroy(_aimView.gameObject);
            Destroy(_view.gameObject);
            Destroy(gameObject);
        }

        public void Enable()
        {
            _view.gameObject.SetActive(true);
        }

        public void Disable()
        {
            _view.gameObject.SetActive(false);
        }

        public void StartAiming()
        {
            _state = InputState.Aiming;
            TurnOnAim();
        }

        private void TurnOnAim()
        {
            _aimView.transform.position = SelectableItem.transform.position;
            _aimView.transform.localScale = SelectableItem.transform.localScale;
            _aimView.gameObject.SetActive(true);
        }

        private void TurnOffAim()
        {
            _aimView.gameObject.SetActive(false);
        }

        private void RotateAim()
        {
            _angle = Mathf.Atan2(Delta.x, Delta.y) * Mathf.Rad2Deg;
            _aimView.transform.eulerAngles = new Vector3(_aimView.transform.eulerAngles.x, _angle, _aimView.transform.eulerAngles.z);
        }
        private void AttachAimToObject()
        {
            _aimView.transform.position = SelectableItem.transform.position;
        }

        private void Shoot()
        {
            _state = InputState.Shooting;
            var direction = Delta.normalized;
            direction.Scale(new Vector3(ShootForce, ShootForce, 1));
            SelectableItem.GetComponent<Rigidbody>().AddForce(direction.x, 0, direction.y, ForceMode.Impulse);
            ShootChipAction?.Invoke();
        }

        private void OnFingerDown(FingerEvent eventData)
        {
            if (eventData.Selection == null)
                return;

            if (_state != InputState.Ready)
                return;

            _state = InputState.SelectChip;
            StartFingerPosition = eventData.Finger.StartPosition;
            SelectableItem = eventData.Selection;
            ChipSelectedAction?.Invoke();
        }

        private void OnFingerMoving(Vector2 fingerPosition)
        {
            CurrentFingerPosition = fingerPosition;
            Delta = StartFingerPosition - CurrentFingerPosition;
        }

        private void OnFingerUp(FingerEvent eventData)
        {
            if (_state == InputState.Aiming)
            {
                Shoot();
                TurnOffAim();
                _state = InputState.Ready;
            }
            else
            {
                ItemDeselectedAction?.Invoke();
            }
        }
    }
}
