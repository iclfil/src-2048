using System.Collections;
using Client.GameSignals;
using DG.Tweening;
using Markins.Runtime.Game.Controllers;
using Markins.Runtime.Game.GUI.Views;
using Supyrb;
using UnityEngine;

namespace Markins.Runtime.Game.GUI
{
    public class RewardedCoinsFlyManager : MonoBehaviour
    {
        public RectTransform MainCanvas;
        public WalletView WalletView;
        public UICoin CoinPrefab;
        public Vector2 OffsetSpawnCoin = new Vector2(0,6);    

        public void Awake()
        {
            Signals.Get<UIAddCoinsSignal>().AddListener(CreateAndFlyCoins);
        }

        private void CreateAndFlyCoins(int count, Vector3 chipPosition)
        {
            StartCoroutine(_Fly(count, chipPosition));
        }

        private IEnumerator _Fly(int count, Vector3 worldPosition)
        {
            for (int c = 0; c < count; c++)
            {
                var coin = Instantiate(CoinPrefab, transform);
                var coinPosInScreen = GetLocalPosition(worldPosition);
                coin.transform.localPosition = coinPosInScreen + OffsetSpawnCoin;
                coin.transform.DOJump(WalletView.transform.position, 1, 1, 1f).SetEase(Ease.InSine).OnComplete(()=>Destroy(coin.gameObject));
                yield return new WaitForSeconds(0.1f);
            }
        }

        private Vector2 GetLocalPosition(Vector3 worldPosition)
        {
            // Convert world position to screen space
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);

            // Convert screen space to Canvas local position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(MainCanvas, screenPos,
                UIManager.instance.UICamera, out Vector2 localPos);

            // Set UI local position based on Canvas local position
            return localPos;
        }

    
    }

}

