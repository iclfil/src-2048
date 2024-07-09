using System;
using Assets.markins._2048.Runtime.Configs;
using Assets.markins._2048.Runtime.Game.Services;
using DG.Tweening;
using UnityEngine;

namespace Markins.Runtime.Game.Controllers
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ChipController : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private bool _hasCollision = true;
        [SerializeField] private ChipView _view;

        public int Power => _config.Power;
        public Action<ChipController, ChipController, Vector3> ChipCollideInPosition { get; set; }
        public float Size => _size;

        private float _size;

        private ChipConfig _config;

        private IPowerToStringFormatterService _powerToStringFormatterService;

        public void Init(ChipConfig config, IPowerToStringFormatterService powerToStringFormatterService, MaterialPropertyBlock block, ChipView viewPrefab)
        {
            _config = config;
            transform.localScale = _config.Size;
            _size = _config.Size.x;
            _powerToStringFormatterService = powerToStringFormatterService;
            ((SphereCollider)_collider).radius = _config.Size.x;
            AddListeners();

            _view = Instantiate(viewPrefab, transform);
            _view.transform.localPosition = Vector3.zero;

            SetupView(block);
        }

        private void SetupView(MaterialPropertyBlock block)
        {
            _block = block;
            _view.Init(block);
            _view.SetColor(_config.Color);

            var number = _config.Power < 23
                ? _config.NumberOfPower.ToString()
                : _powerToStringFormatterService.FormatPowerOfTwo(_config.Power);

            _view.SetSymbol(number);
        }

        private void AddListeners()
        {
        }

        private void RemoveListeners()
        {
        }
        public void Free()
        {
            ChipCollideInPosition = null;
            _config = null;
            RemoveListeners();
        }

        public void Spawn()
        {
            var offset = new Vector3(0.3f, 0.3f, 0.3f);
            transform.localScale = Vector3.zero + offset;
            transform.DOScale(_config.Size + offset, 0.3f).SetEase(Ease.OutSine).Play();
            transform.DOScale(_config.Size, 0.4f).SetEase(Ease.OutBounce).Play().SetDelay(0.25f);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }


        private MaterialPropertyBlock _block;

        public void OnCollisionEnter(Collision collision)
        {
            if (_hasCollision == false)
                return;

            if (collision.gameObject.TryGetComponent(out ChipController chip))
            {
                ChipCollideInPosition?.Invoke(this, chip, collision.GetContact(0).point);
            }
        }

        private void HitHandler()
        {
            _hasCollision = false;
            _collider.enabled = false;
            _rigidbody.isKinematic = true;
        }

        public void DisablePhysics()
        {
            _hasCollision = false;
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
        }

        public void ChangeView(ChipView viewPrefab)
        {
            _view = viewPrefab;
            SetupView(_block);
        }
    }
}

//if (collision.collider.CompareTag(Borders))
//{
//    MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
//    return;
//}