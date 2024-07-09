using System;
using System.Collections;
using System.Collections.Generic;
using Markins.Runtime.Game.Storage.Models;
using UnityEngine;
using DG.Tweening;
using Markins.Runtime.Game.Configs;
using Markins.Runtime.Game.Storage.Converter;
using Markins.Runtime.Game.Utils;
using MoreMountains.NiceVibrations;
using Supyrb;

namespace Markins.Runtime.Game.Controllers
{
    public enum FieldState
    {
        None,
        Ready,
        HasMatches,
        FinishChanges,
        Waiting,
    }

    public class FieldController : Singleton<FieldController>
    {
        [SerializeField] private Transform _chipsRoot;
        [SerializeField] private Transform _center;
        [SerializeField] private Transform _leftBottomPoint;
        [SerializeField] private Transform _spawnChipsPosition;
        public Transform Center => _center;

        public FieldState State = FieldState.Ready;

        public Action<int, int> OnMatchChips { get; set; }

        public IEnumerable<ChipController> Chips => _chipsManager.Chips;

        public Action OnFieldChangeFinished;

        public static Vector3 BottomLeftBorder;

        public float MaxDensity = 11.16f; // Высчитал опытным путем:) каждый раз при изменении фишки надо пересчитывать, в норм игре - переделать.

        public float CurrentDensity;


        private string _nameView = "Default";
        public bool IsStop = false;
        private FieldConfig _config;
        private ChipsManager _chipsManager;
        private IMatchChipsAnimation _matchChipsAnimation;
        private float _spawnHeight = 0;
        private FieldView _view;
        private List<Match> _matches = new();
        public int Shoot = 5;

        public Action<int> OnAddShoot;
        public Action<int> OnRemoveShoot;


        private void OnDestroy()
        {
            Free();
        }

        public void Init(FieldConfig config)
        {
            _config = config;
            _nameView = config.DefaultNameView;
            _chipsRoot = transform;


            _matchChipsAnimation = new MatchChipsAnimation();
            _chipsManager = new ChipsManager(_config.ChipCollection, _chipsRoot);
            _chipsManager.OnMatchChips += MatchHandler;
            SetupView();
            BottomLeftBorder = _leftBottomPoint.transform.localPosition;
            _spawnHeight = _spawnChipsPosition.localPosition.y;
            ResizeRack();
            AddListeners();
        }


        public void Free()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            Signals.Get<OnSelectFieldSkinChangedSignal>().AddListener(FieldSkinChangeHandler);
        }
        private void RemoveListeners()
        {

        }

        public IEnumerator _CreateField()
        {
            ShowView();
            yield return new WaitForSeconds(1f);

            if (_chipDatas == null)
            {
                for (int i = 0; i < 14; i++)
                {
                    _chipsManager.CreateRandomChip(GetRandomPosition());
                    yield return new WaitForSeconds(0.005f);
                }
            }
            else
            {
                foreach (var chipData in _chipDatas)
                {
                    _chipsManager.CreateChip(chipData.Power, new Vector3(chipData.PosX, _spawnHeight, chipData.PosZ));
                    yield return new WaitForSeconds(0.005f);
                }
            }
            yield break;
        }

        private void ShowView()
        {
            _view.transform.DOScale(1, 1).SetEase(Ease.OutBounce).Play();
        }

        private void SetupView()
        {
            var viewPrefab = _config.GetPrefabFieldView(_nameView);

            if (_view != null)
            {
                Destroy(_view.gameObject);
            }

            _view = Instantiate(viewPrefab, transform);
            _view.name = _nameView;
            _view.transform.localPosition = Vector3.zero;
            _view.transform.localScale = Vector3.zero;
        }

        private void FieldSkinChangeHandler(string nameSkin)
        {
            _nameView = nameSkin;
            SetupView();
            _view.transform.localScale = Vector3.one;
        }

        public void PlayerShoot()
        {
            SimpleTimer.instance.StartTask(1, () => _chipsManager.CreateRandomChip(GetRandomPosition()));

        }

        public bool HasChanges = false;
        public void Run()
        {
            IsStop = false;

            if (_matches.Count >= 1)
            {
                HasChanges = true;
                StartCoroutine(_ChangeField());
            }

            if (_matches.Count == 0)
            {
                HasChanges = false;

                OnFieldChangeFinished?.Invoke();
            }
        }

        public bool IsFilled()
        {
            if (CurrentDensity >= MaxDensity)
            {
                return true;
            }

            return false;
        }

        public bool IsHalfFilled()
        {
            if (CurrentDensity >= MaxDensity - 10)
            {
                return true;
            }

            return false;
        }


        public void Stop()
        {
            IsStop = true;
        }

        public IEnumerator _ChangeField()
        {
            var match = _matches.Find(x => x.Calculate == false);

            if (match == null)
                yield break;

            match.Calculate = true;

            yield return HandleAndPlayMatch(match).Play().WaitForCompletion();
            Shoot++;
            OnAddShoot?.Invoke(Shoot);
            _matches.Remove(match);
        }

        private Sequence HandleAndPlayMatch(Match match)
        {
            OnMatchChips?.Invoke(match.First.Power, match.First.Power);

            var firstChip = match.First;
            var secondChip = match.Second;
            var power = firstChip.Power;

            _chipsManager.RemoveChip(firstChip);
            _chipsManager.RemoveChip(secondChip);

            var posX = match.MatchPosition.x;
            var posZ = match.MatchPosition.z;
            var posY = _spawnHeight;

            var matchPosition = new Vector3(posX, posY, posZ);

            Signals.Get<OnChipsMatchedSignal>().Dispatch(matchPosition);
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
            return _matchChipsAnimation.MatchAnimation(firstChip, secondChip, matchPosition,
                () => _chipsManager.CreateNextChip(power, matchPosition));

        }

        private void ResizeRack()
        {
            float aspectRatio = Screen.height / Screen.width;

            if (aspectRatio >= 2)
                transform.localScale = new Vector3(0.91f, 0.91f, 0.91f);
        }

        private void MatchHandler(ChipController first, ChipController second, Vector3 matchPos)
        {
            if (IsStop)
                return;

            if (first.Power != second.Power)
                return;

            first.DisablePhysics();
            second.DisablePhysics();

            _matches.Add(new Match()
            {
                First = first,
                Second = second,
                MatchPosition = matchPos,
            });
        }

        public Vector3 GetRandomPosition()
        {
            float randX = UnityEngine.Random.Range(BottomLeftBorder.x, BottomLeftBorder.x * -1);
            float randZ = UnityEngine.Random.Range(BottomLeftBorder.z, BottomLeftBorder.z * -1);
            return new Vector3(randX, _spawnHeight, randZ);
        }

        public bool HasSymbolId(int power)
        {
            return _chipsManager.Chips.Exists(x => x.Power == power);
        }
        public void Enable()
        {
        }
        public void Disable()
        {

        }

        #region Chips

        public ChipController GetAndRemoveChipBySymbol(int power)
        {
            var chip = _chipsManager.Chips.Find(x => x.Power == power);
            _chipsManager.RemoveChip(chip);
            return chip;
        }

        #endregion

        private IEnumerable<DataConverter.ChipData> _chipDatas;

        public void AddChips(IEnumerable<DataConverter.ChipData> chips)
        {
            _chipDatas = chips;
        }

        public void AddChipHandler(ChipController chip)
        {
            CurrentDensity += chip.Size;
        }

        public void RemoveChipHandler(ChipController chip)
        {
            CurrentDensity -= chip.Size;
        }

        public void ClearField()
        {
            CurrentDensity = 0;
            _chipsManager.DeleteAllChips();
        }
    }
}