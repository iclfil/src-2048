using System.Collections;
using DG.Tweening;
using Markins.Runtime.Game;
using Markins.Runtime.Game.Feature;
using Supyrb;
using UnityEngine;

namespace Assets.markins._2048.Runtime.UI.Widgets
{
    public class ComboPanelViewController : MonoBehaviour
    {
        [SerializeField] private TextComboView _prefabComboView;

        private TextComboView _currentComboView;

        public void Init()
        {
            Signals.Get<OnComboCollectedSignal>().AddListener(ShowCombo);
        }

        public void Free()
        {
            Signals.Get<OnComboCollectedSignal>().RemoveListener(ShowCombo);
        }

        private void ShowCombo(int comboCounts, Vector3 position)
        {
            StartCoroutine(_ShowCombo(comboCounts, position));
        }

        private IEnumerator _ShowCombo(int comboCounts, Vector3 position)
        {
            var view = Instantiate(_prefabComboView, transform);
            var localPos = Camera.main.WorldToViewportPoint(position);

            view.RectTransform.anchoredPosition = localPos;
            view.RectTransform.anchorMin = localPos;
            view.RectTransform.anchorMax = localPos;

            //view.transform.DOScale(1, 0.2f).SetEase(Ease.OutBounce).Play();
            view.RectTransform.DOAnchorPosY(125, 1f).SetEase(Ease.OutSine).Play();
            view.Text.text += $" X{comboCounts}";
            Destroy(view.gameObject, 1f);
            yield break;
        }
    }
}