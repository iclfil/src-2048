using deVoid.UIFramework;
using DG.Tweening;
using Markins.Runtime.Game.GUI.Contexts;
using Markins.Runtime.Game.GUI.MySignals;
using UnityEngine;
using Supyrb;
using TMPro;
using UnityEngine.UI;

namespace Markins.Runtime.Game.GUI
{
    public class WinLevelWindowController : AWindowController<WinLevelWindowProps>
    {
        [SerializeField] private Button _rewardButton;
        [SerializeField] private Button _continueButton;

        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _rewardCountText;

        private UserSelectNextLevelSignal userSelectNext;

        protected override void Awake()
        {
            base.Awake();
            userSelectNext = Signals.Get<UserSelectNextLevelSignal>();
            _rewardButton.transform.localScale = Vector3.zero;
            _continueButton.transform.localScale = Vector3.zero;
        }

        protected override void OnPropertiesSet()
        {
            LevelChanged(Properties.Level);
            EnableReward(Properties.HasReward, Properties.CountReward);
        }

        protected override void AddListeners()
        {
            Debug.Log("ADD LISTNERS");
            _rewardButton.onClick.AddListener(ClickRewardButton);
            _continueButton.onClick.AddListener(ClickContinueButton);
            OnShow += ShowHandler;
            OnHide += HideHandler;
        }

        private void HideHandler()
        {
            _rewardButton.transform.localScale = Vector3.zero;
            _continueButton.transform.localScale = Vector3.zero;
        }

        private void ShowHandler()
        {
            _rewardButton.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce).SetDelay(1f);
            _continueButton.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce).SetDelay(1.5f);
        }

        protected override void RemoveListeners()
        {
            _rewardButton.onClick.RemoveListener(ClickRewardButton);
            _continueButton.onClick.RemoveListener(ClickContinueButton);
            OnShow -= ShowHandler;
            OnHide -= HideHandler;
        }

        private void ClickRewardButton()
        {
            userSelectNext.Dispatch(true);
        }

        private void ClickContinueButton()
        {
            userSelectNext.Dispatch(false);
        }

        private void LevelChanged(int level)
        {
            _levelText.text = level.ToString();
        }

        private void EnableReward(bool enable, int count)
        {
            if (enable == false)
                return;

            _rewardButton.enabled = true;
            _rewardCountText.text = count.ToString();
        }
    }
}
