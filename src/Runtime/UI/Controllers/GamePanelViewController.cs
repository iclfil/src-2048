using Assets.Game.Code.Game;
using Markins.Runtime.Game;
using Assets.markins._2048.Runtime.UI.Widgets;
using Markins.Runtime.Game.Storage.Models;
using Client.GameSignals;
using deVoid.UIFramework;
using DG.Tweening;
using Markins.Runtime.Game.GUI.MySignals;
using Markins.Runtime.Game.GUI.Views;
using Supyrb;
using UnityEngine;
using UnityEngine.UI;

namespace Markins.Runtime.Game.GUI.Screens
{
    public class GamePanelViewController : APanelController
    {
        public float Radius = 1;
        public Vector2 Offset = new Vector2(0, 0);

        [SerializeField] private RectTransform canvas;

        [Header("Prefabs")]
        public UICoin CoinPrefab;

        [Header("UI Elements")]
        public Button Settings;
        public Button Store;
        [Header("UI Views")]
        public WalletView WalletView;
        [Space]
        public GameProgressView progressPanel;
        [Space]
        public TargetSymbolView TargetSymbolView;

        [SerializeField] private ComboPanelViewController _comboPanelViewController;

        private OnClickOpenSettingsSignal _onClickOpenSettingsSignal;
        private OnClickOpenStoreSignal _onClickOpenStoreSignal;

        protected override void Awake()
        {
            base.Awake();

            _comboPanelViewController.Init();

            _onClickOpenSettingsSignal = Signals.Get<OnClickOpenSettingsSignal>();
            _onClickOpenStoreSignal = Signals.Get<OnClickOpenStoreSignal>();

        }

        protected override void AddListeners()
        {
            Signals.Get<OnScoreForNextChangedSignal>().AddListener(progressPanel.SetScoreForNextLevel);

            Signals.Get<LevelChangeSignal>().AddListener(progressPanel.LevelChanged);
            Signals.Get<OnScoresChangedSignal>().AddListener(progressPanel.UpdateScore);

            Signals.Get<OnMoneyChangedSignal>().AddListener(ChangeMoney);
            Signals.Get<AddCoinsAction>().AddListener(AddCoins);
            Signals.Get<BestScoreChangeSignal>().AddListener(BestScoreChanged);
            Signals.Get<TargetChipChangeSignal>().AddListener(NumberTargetChange);
            Settings.onClick.AddListener(ClickSettingsButton);
            Store.onClick.AddListener(ClickStoreButton);

            Signals.Get<UIAddCoinsSignal>().AddListener(AddCoins);

        }

     

        protected override void RemoveListeners()
        {
            Signals.Get<OnScoreForNextChangedSignal>().RemoveListener(progressPanel.SetScoreForNextLevel);

            Signals.Get<UIAddCoinsSignal>().RemoveListener(AddCoins);
            Signals.Get<AddCoinsAction>().RemoveListener(AddCoins);

            Signals.Get<BestScoreChangeSignal>().AddListener(BestScoreChanged);

            Signals.Get<TargetChipChangeSignal>().RemoveListener(NumberTargetChange);


            _onClickOpenSettingsSignal.RemoveListener(ClickSettingsButton);
            _onClickOpenStoreSignal.RemoveListener(ClickStoreButton);

            _comboPanelViewController.Free();
        }

        private void NumberTargetChange(string value)
        {
            TargetSymbolView.SetNumber(value);
        }

        private void AddCoins(int coins)
        {
            var mainSeq = DOTween.Sequence();
            var delay = 0.02f;
            Debug.Log("ADD COINS" + coins);
            for (int i = 0; i < coins; i++)
            {
                var seq = DOTween.Sequence();
                var coin = Instantiate(CoinPrefab, transform);
                coin.transform.localScale = Vector3.zero;
                var rand = UnityEngine.Random.Range(-0.5f, 0.5f);
                var x = Mathf.Sin(i * Radius) + rand;
                var y = Mathf.Cos(i * Radius) + rand;
                coin.transform.position = new Vector3(x, y, 0);

                var finishPos = WalletView.transform.position;

                seq.Append(coin.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));
                seq.Append(coin.transform.DOMove(finishPos, 1).SetEase(Ease.OutSine)).OnComplete(() => AddCash(0));
                seq.Append(coin.transform.DOScale(Vector3.zero, 0.2f));
                mainSeq.Insert(i * delay, seq);
            }

            mainSeq.Play();
        }

        private void AddCoins(int coins, Vector3 globalPosition)
        {
            var mainSeq = DOTween.Sequence();
            var screnPosition = Camera.main.WorldToScreenPoint(globalPosition);
            //Debug.Log("ADD COINS" + coins);
            //for (int i = 0; i < coins; i++)
            //{
            //    var seq = DOTween.Sequence();
            //    var coin = Instantiate(CoinPrefab, transform);
            //    coin.transform.localScale = Vector3.zero;
            //    coin.RectTransform.anchoredPosition = screnPosition;

            //    var finishPos = WalletView.transform.position;

            //    seq.Append(coin.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));
            //    seq.Append(coin.transform.DOMove(finishPos, 1).SetEase(Ease.OutSine)).OnComplete(() => AddCash(1));
            //    seq.Append(coin.transform.DOScale(Vector3.zero, 0.2f));
            //    mainSeq.Insert(i , seq);
            //}

            //mainSeq.Play();
        }

        private void ClickSettingsButton()
        {
            _onClickOpenSettingsSignal.Dispatch();
        }

        private void ClickStoreButton()
        {
            _onClickOpenStoreSignal.Dispatch();
        }

        private void ChangeMoney(int oldValue, int newValue)
        {
            WalletView.SetCash(newValue);
        }
        private void AddCash(int value)
        {
            WalletView.AddCash(value);
        }
        private void SetCash(int value)
        {
            WalletView.SetCash(value);
        }

        private void BestScoreChanged(int value)
        {
            progressPanel.SetBestScore(value);
        }
    }
}