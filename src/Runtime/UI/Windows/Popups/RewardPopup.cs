using deVoid.UIFramework;
using Markins.Runtime.Game.GUI.MySignals;
using Supyrb;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Markins.Runtime.Game.GUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class RewardPopup : AWindowController<Contexts.RewardPopupProps>
    {
        [SerializeField] private Button _claimRewardButton;
        [SerializeField] private Button _claimMultiplyRewardButton;
        [SerializeField] private TextMeshProUGUI _rewardTxt;

        protected override void OnPropertiesSet()
        {
            _claimMultiplyRewardButton.interactable = Properties.HasAds;
            _rewardTxt.text = Properties.CountReward.ToString();
        }

        protected override void AddListeners()
        {
            _claimRewardButton.onClick.AddListener(ClaimReward);
            _claimMultiplyRewardButton.onClick.AddListener(ClaimMultiplyReward);
        }

        protected override void RemoveListeners()
        {
            _claimRewardButton.onClick.RemoveListener(ClaimReward);
            _claimMultiplyRewardButton.onClick.RemoveListener(ClaimMultiplyReward);
        }

        private void ClaimReward()
        {
            Signals.Get<UserExplodeTargetChipSignal>().Dispatch(false);
        }

        private void ClaimMultiplyReward()
        {
            Signals.Get<UserExplodeTargetChipSignal>().Dispatch(true);
        }
    }
}

