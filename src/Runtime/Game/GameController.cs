using System.Collections;
using System.Linq;
using Assets.markins._2048.Runtime.Game.Services;
using Markins.Runtime.Game.Storage;
using Client.GameSignals;
using DG.Tweening;
using InstantGamesBridge;
using Markins.Runtime.Game.Controllers;
using Markins.Runtime.Game.GUI.Contexts;
using Markins.Runtime.Game.GUI.MySignals;
using Markins.Runtime.Game.Storage.Models;
using Markins.Runtime.Game.Utils;
using Supyrb;
using UnityEngine;
using InstantGamesBridge.Modules.Leaderboard;

namespace Markins.Runtime.Game
{
    public enum StateGame
    {
        None,
        StarGame,
        Game,
        Pause,
        EngGame,
        Lose
    }

    public class GameController : Singleton<GameController>
    {
        public StateGame State = StateGame.StarGame;

        public LevelChangeSignal LevelChangeSignal;
        public OnScoresChangedSignal OnScoreChangedSignal;
        public TargetChipChangeSignal TargetChipChangeSignal;
        public BestScoreChangeSignal BestScoreChangeSignal;

        public UserSelectNextLevelSignal UserSelectNextLevelSignal;
        public UserExplodeTargetChipSignal UserExplodeTargetChipSignal;

        private FieldController _fieldController;
        private RewardController _rewardController;

        private GameView _view;
        [SerializeField]
        private GameModel _model;
        private GameStorage _gameStorage;
        private GameConfig _config;

        private ShootControlService _inputController;
        private SimpleTimer _simpleTimer;
        private IPowerToStringFormatterService _formatterService;
        private InventoryController _inventoryController;
        private int _countWinnings = 1;
        private int _currentWinnings = 0;

        public void Init(GameModel model, GameConfig config, GameStorage gameStorage)
        {
            _model = model;
            _gameStorage = gameStorage;
            _config = config;
            _formatterService = new PowerToStringFormatterService();
            _simpleTimer = new GameObject("SimpleTimer").AddComponent<SimpleTimer>();

            _inputController = ShootControlService.instance;
            _fieldController = FieldController.instance;
            _rewardController = RewardController.instance;
            _inventoryController = InventoryController.instance;

            _model.OnScoreChanged += ScoreChangeHandler;
            _model.OnLevelChanged += LevelChangeHandler;
            _model.OnTargetScoreChanged += TargetScoreChangedHandler;
            _model.OnTargetChipChanged += TargetChipChangeHandler;
            _model.OnBestScoreChanged += BestScoreChangeHandler;

            IsLeaderboardSupported = Bridge.leaderboard.isSetScoreSupported;

            InitSignals();
            AddListeners();
        }



        private bool IsLeaderboardSupported = false;

        public void Free()
        {
            RemoveListeners();

            _model.OnScoreChanged -= ScoreChangeHandler;
            _model.OnLevelChanged -= LevelChangeHandler;
            _model.OnTargetChipChanged -= TargetChipChangeHandler;
            _model.OnBestScoreChanged -= BestScoreChangeHandler;


        }

        private void InitSignals()
        {
            OnScoreChangedSignal = Signals.Get<OnScoresChangedSignal>();
            TargetChipChangeSignal = Signals.Get<TargetChipChangeSignal>();
            LevelChangeSignal = Signals.Get<LevelChangeSignal>();
            BestScoreChangeSignal = Signals.Get<BestScoreChangeSignal>();
            UserSelectNextLevelSignal = Signals.Get<UserSelectNextLevelSignal>();
            UserExplodeTargetChipSignal = Signals.Get<UserExplodeTargetChipSignal>();
        }

        private void AddListeners()
        {

            _rewardController.OnPlayerCollectedCrystal += OnPlayerCollectedCrystal;
            _fieldController.OnMatchChips += CalculateScore;

            UserSelectNextLevelSignal.AddListener(NextLevelHandler);
            UserExplodeTargetChipSignal.AddListener(ExplodeTargetChipCheckAds);

            Signals.Get<OnSelectGameSkinChangedSignal>().AddListener(GameSkinChanged);

            Signals.Get<OnStoreOpenedSignal>().AddListener(PauseGame);
            Signals.Get<OnSettingsOpenedSignal>().AddListener(PauseGame);

            Signals.Get<OnStoreClosedSignal>().AddListener(StoreOrSettingsCloseHandler);
            Signals.Get<OnSettingsClosedSignal>().AddListener(StoreOrSettingsCloseHandler);

            Signals.Get<OnStoreClosedSignal>().AddListener(ContinueGame);
            Signals.Get<OnSettingsClosedSignal>().AddListener(ContinueGame);

            ShootControlService.instance.ChipSelectedAction += PlayerSelectChip;
            ShootControlService.instance.ShootChipAction += PlayerShootChip;

            Signals.Get<OnClickAgainButtonSignal>().AddListener(ClickAgainButtonHandler);

            AdsManager.instance.RewardedAdsRewarded += UserAdsRewarded;
            AdsManager.instance.RewardedAdsClosed += UserAdsClosed;
            AdsManager.instance.RewardedAdsFailed += UserAdsClosed;
        }

        private void RemoveListeners()
        {

            Signals.Get<OnStoreOpenedSignal>().RemoveListener(PauseGame);

            Signals.Get<OnStoreClosedSignal>().RemoveListener(ContinueGame);

            Signals.Get<OnSettingsOpenedSignal>().RemoveListener(PauseGame);
            Signals.Get<OnSettingsClosedSignal>().RemoveListener(ContinueGame);

            ShootControlService.instance.ChipSelectedAction -= PlayerSelectChip;
            ShootControlService.instance.ShootChipAction -= PlayerShootChip;

            _fieldController.OnMatchChips -= CalculateScore;
            UserSelectNextLevelSignal.RemoveListener(NextLevelHandler);
        }

        public bool UserHasDoubleRewardForScoreCompleted;

        public bool UserHasDoubleRewardForChipTargetCompleted;

        private void UserAdsRewarded()
        {
            if (_model.TargetCompleted)
            {
                UserHasDoubleRewardForChipTargetCompleted = true;
            }

            if (_model.ScoresCompleted)
            {
                UserHasDoubleRewardForScoreCompleted = true;
            }
        }

        private void UserAdsClosed()
        {
            if (_model.TargetCompleted)
            {
                ExplodeTargetChip(UserHasDoubleRewardForChipTargetCompleted);
                UserHasDoubleRewardForChipTargetCompleted = false;
            }

            if (_model.ScoresCompleted)
            {
                NextLevel(UserHasDoubleRewardForScoreCompleted);
                UserHasDoubleRewardForScoreCompleted = false;
            }
        }

        private void ClickAgainButtonHandler()
        {
            _gameStorage.DeleteChipOnField();
            _fieldController.ClearField();
            StartCoroutine(_fieldController._CreateField());
            UIManager.instance.Frame.CloseCurrentWindow();
            ContinueGame();
        }

        private void StoreOrSettingsCloseHandler()
        {
            StartCoroutine(_gameStorage.Save());
        }

        private void OnPlayerCollectedCrystal()
        {
            var countMoney = _config.MoneyForCrystal;
            _inventoryController.AddMoney(countMoney);
        }

        private void GameSkinChanged(string nameTheme)
        {
            Debug.Log("Name Skin" + nameTheme);
            Destroy(_view.gameObject);
            var gameThemePrefab = _config.GetGameTheme(nameTheme);
            _view = Instantiate(gameThemePrefab, transform);
        }

        public void StartGame()
        {
            State = StateGame.StarGame;
            SetupView();
            StartCoroutine(_StartGame());

            Debug.Log("Language" + Bridge.platform.language);

        }

        private IEnumerator _StartGame()
        {
            if (_model.Score == 0)
            {
                OpenTutorial();
                yield break;
            }
            else
            {
                OpenGame();
            }
        }

        private void OpenGame()
        {
            UIManager.instance.Frame.ShowPanel(ScreenIds.GamePanel);
            StartCoroutine(_fieldController._CreateField());
            State = StateGame.Game;
        }

        private void OpenTutorial()
        {
            Signals.Get<OnClickCloseTutorialSignal>().AddListener(CloseTutorial);
            UIManager.instance.Frame.OpenWindow(ScreenIds.Tutorial);
        }

        private void CloseTutorial()
        {
            Signals.Get<OnClickCloseTutorialSignal>().RemoveListener(CloseTutorial);
            UIManager.instance.Frame.CloseWindow(ScreenIds.Tutorial);
            OpenGame();
        }

        private void SetupView()
        {
            _view = Instantiate(_model.PrefabView, transform);
        }

        private ChipController _targetChip;

        private void Update()
        {
            if (State != StateGame.Game)
            {
                return;
            }

            _inputController.Tick();

            if (_fieldController.HasChanges == false)
            {
                if (_model.ScoreWinning())
                {
                    PauseGame();
                    OpenWinLevelWindow();
                }
                else if (_model.TargetWinning(_fieldController))
                {
                    PauseGame();
                    _targetChip = _fieldController.GetAndRemoveChipBySymbol(_model.TargetChip);
                    _targetChip.DisablePhysics();
                    EffectsManager.instance.Play(EffectCollection.EffectsEvent.SELECT_TARGET_CHIP, _targetChip.transform);
                    var seq = DOTween.Sequence();
                    seq.AppendInterval(0.5f);
                    seq.Append(_targetChip.transform.DOJump(_fieldController.Center.position + new Vector3(0, 5, 0), 4, 1, 1f).SetEase(Ease.OutSine));
                    seq.AppendInterval(0.5f);
                    seq.AppendCallback(OpenTargetChipRewardPopup);
                    seq.Play();
                }
                else if (_fieldController.IsFilled())
                {
                    PauseGame();
                    UIManager.instance.ShowLoseWindow();
                    State = StateGame.Lose;
                }
                else if (_fieldController.IsHalfFilled())
                {
                    UIManager.instance.ShowHalfFilledFieldMessage();
                }
            }

            if (State == StateGame.Game)
            {
                _fieldController.Run();
            }
        }

        private void PlayerSelectChip()
        {
            //если стейт игры пулять фишки
            ShootControlService.instance.StartAiming();
        }


        private int shootCounts;
        private void PlayerShootChip()
        {
            shootCounts++;
            if (shootCounts == 22)
            {
                SaveField();
                SaveGame();
                shootCounts = 0;
            }
            _fieldController.PlayerShoot();
        }

        private void ExplodeTargetChipCheckAds(bool hasAds)
        {
            if (hasAds)
            {
                AdsManager.instance.ShowRewarded();
            }
            else
            {
                ExplodeTargetChip(false);
            }
        }

        private void ExplodeTargetChip(bool doubleReward)
        {
            var crystals = _model.RewardForTargetCompleted;

            if (doubleReward)
            {
                crystals *= 2;
            }

            UIManager.instance.Frame.CloseCurrentWindow();
            var seq = _rewardController.ExplodeChipWithCrystalls(_targetChip, crystals);
            seq.InsertCallback(0.1f, GameCamera.instance.Shake);
            seq.Play().OnComplete(ContinueGame);
            _model.NextTargetChip();
            StartCoroutine(_gameStorage.Save());
        }

        private void PauseGame()
        {
            State = StateGame.Pause;
            ShootControlService.instance.Disable();
            _fieldController.Disable();
        }

        private void ContinueGame()
        {
            State = StateGame.Game;
            ShootControlService.instance.Enable();
            _fieldController.Enable();
        }

        private void NextLevelHandler(bool withAdsReward)
        {
            if (withAdsReward)
            {
                AdsManager.instance.ShowRewarded();
                return;
            }

            NextLevel(false);
        }

        private void NextLevel(bool doubleReward)
        {
            var reward = _model.RewardForScoreCollected;

            if (doubleReward)
                reward *= 2;

            ContinueGame();
            UIManager.instance.Frame.CloseCurrentWindow();
            _rewardController.AddMoney(reward);

            SaveGame();
        }

        private void CalculateScore(int firstSymbolId, int secondSymbolId)
        {
            _model.AddScore();
        }

        private void OpenWinLevelWindow()
        {
            _currentWinnings++;

            Debug.Log("Open Win LEvel Window" + _currentWinnings);
            var props = new WinLevelWindowProps
            {
                CountReward = _model.RewardForScoreCollected,
                HasReward = true,
                Level = _model.Level,
            };

            _model.NextLevel();

            UIManager.instance.Frame.OpenWindow(ScreenIds.WinLevelWindow, props);

            if (_currentWinnings >= _countWinnings)
            {
                _simpleTimer.StartTask(0.5f, AdsManager.instance.ShowInter);
                _currentWinnings = 0;
            }

            SaveField();
            SaveGame();
        }

        public void SaveGame()
        {
            StartCoroutine(_gameStorage.Save());
        }

        public void SaveField()
        {
            //небольшая проверка, чтобы не сохранить фишки, которые вдруг выпали с поля, и летят в бесконечность.
            var chips = _fieldController.Chips.Where(x => x.transform.position.y >= 0);
            StartCoroutine(_gameStorage._SaveField(chips));
        }

        private void OpenTargetChipRewardPopup()
        {
            var props = new RewardPopupProps();
            props.CountReward = _model.RewardForTargetCompleted;
            props.HasAds = true;
            UIManager.instance.Frame.OpenWindow(ScreenIds.TargetCompletePopup, props);
            SaveField();
            SaveGame();
        }

        private void BestScoreChangeHandler(int value)
        {
            BestScoreChangeSignal.Dispatch(value);
        }

        private void TargetScoreChangedHandler(int newValue)
        {
            Signals.Get<OnTargetScoreChangedSignal>().Dispatch(newValue);
        }

        private void TargetChipChangeHandler(int power)
        {
            var data = _formatterService.FormatPowerOfTwo(power);
            TargetChipChangeSignal.Dispatch(data);
        }

        private void LevelChangeHandler(int value)
        {
            LevelChangeSignal.Dispatch(value);
        }

        private void ScoreChangeHandler(int value)
        {

            if (IsLeaderboardSupported)
            {
                SendScoreToLeaderboard(value);
            }


            OnScoreChangedSignal.Dispatch(value, _model.ScorePerMatch);
        }

        private void SendScoreToLeaderboard(int score)
        {
            var leaderboardName = "BestScore";
            var dataOptions = new SetScoreYandexOptions(score, leaderboardName);
            Bridge.leaderboard.SetScore(dataOptions);
        }
    }
}