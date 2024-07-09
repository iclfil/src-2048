using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Markins.Runtime.Game.GUI.Views
{
    public class WalletView : MonoBehaviour
    {
        public TextMeshProUGUI CashCount;
        private int _curValue = 0;

        public void SetCash(int cash)
        {
            _curValue = cash;
            CashCount.text = cash.ToString();
        }

        public void AddCash(int count)
        {
            _curValue += count;
            CashCount.text = _curValue.ToString();
        }

        public void ChangeMoney(int old, int newValue)
        {
            SetCash(newValue);
        }
    }
}

