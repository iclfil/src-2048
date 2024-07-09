using Assets.Game.Code.Game;
using Client.GameSignals;
using DG.Tweening;
using Markins.Runtime.Game.Controllers;
using Markins.Runtime.Game.Feature;
using Markins.Runtime.Game.Storage;
using Markins.Runtime.Game.Storage.Models;
using Markins.Runtime.Game.Utils;
using Supyrb;
using UnityEngine;

namespace Markins.Runtime.Game
{
    public delegate void OnPlayerCollectedCrystal();

    public delegate void OnPlayerCollectedCoin();

    // Получает информацию о том, что нужно создать награду.
    // И сообщает информацию о том, что награда была собрана, удаляет награду.
    public class RewardController : Singleton<RewardController>
    {
        public OnPlayerCollectedCrystal OnPlayerCollectedCrystal;

        private RewardConfig _config;

        private UIAddCoinsSignal _uiAddCoinsSignal;

        private InventoryController _inventoryController;

        public void Init(RewardConfig config)
        {
            _config = config;
            _uiAddCoinsSignal = Signals.Get<UIAddCoinsSignal>();
            _inventoryController = InventoryController.instance;
            Signals.Get<OnComboCollectedSignal>().AddListener(CollectComboHandler);
        }

        private void CollectComboHandler(int countCombos, Vector3 worldPosCombo)
        {
            if (countCombos >= 5)
            {
                AddMoney(worldPosCombo);
            }
        }

        private void AddMoney(Vector3 worldPosCombo)
        {
            _inventoryController.AddMoney(1);
            _uiAddCoinsSignal.Dispatch(1, worldPosCombo);
        }

        public void AddMoney(int count)
        {
            _inventoryController.AddMoney(count);
            Signals.Get<AddCoinsAction>().Dispatch(count);
        }

        public Sequence ExplodeChipWithCrystalls(Vector3 explodePosition, ChipController chip, int countCrystalls)
        {
            chip.DisablePhysics();

            TweenCallback callback = () =>
            {
                for (int i = 0; i < countCrystalls; i++)
                {
                    SpawnCrystal(explodePosition);
                }
            };

            var sequence = DOTween.Sequence();
            var Duration = 1;
            var ForceJump = 5;


            sequence.Append(chip.transform.DOJump(explodePosition + new Vector3(0, 2, 0), ForceJump, 1, Duration));
            sequence.Insert(1, chip.transform.DOPunchScale(new Vector3(0.3f, 0, 0.3f), 0.5f, 4, 1.5f).SetLoops(6));
            sequence.InsertCallback(3, callback);
            sequence.InsertCallback(3, () =>
            {
            });
            sequence.InsertCallback(3, chip.Destroy);
            return sequence;
        }

        public Sequence ExplodeChipWithCrystalls(ChipController chip, int countCrystalls)
        {
            var chipPosition = chip.transform.position;

            TweenCallback spawnCrystalls = () =>
            {
                for (int i = 0; i < countCrystalls; i++)
                {
                    SpawnCrystal(chipPosition);
                }
            };

            var sequence = DOTween.Sequence();
            sequence.InsertCallback(0, chip.Destroy);
            sequence.InsertCallback(0, spawnCrystalls);
            sequence.InsertCallback(0, () =>
            {
                EffectsManager.instance.Play(EffectCollection.EffectsEvent.EXPLODE_TARGET_CHIP, chipPosition);
            });
            return sequence;
        }


        private void SpawnCrystal(Vector3 position)
        {
            var crystal = Instantiate(_config.CrystalViewPrefab, transform);
            crystal.transform.position = position + RandomVector();
            crystal.transform.localScale = Vector3.one;
            crystal.Rigidbody.isKinematic = false;
            crystal.OnCollision = RewardCollisionCallback;
        }

        private void RewardCollisionCallback(CollisionEnterNotifier cur, string collisionTag)
        {
            var pieceTag = GameConstants.PieceTag;

            if (pieceTag != collisionTag) //TODO
                return;

            cur.EnableCollision = false;
            cur.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f, 4, 5).OnComplete(() =>
            {
                cur.gameObject.SetActive(false);
                SpawnGoldBlastFX(cur.transform.position);
            });

            AddMoney(cur.transform.position);
            OnPlayerCollectedCrystal?.Invoke();
        }

        private void SpawnCoin(Vector3 position)
        {
            var coin = Instantiate(_config.CoinViewPrefab);
            coin.transform.position = position;
        }

        private void SpawnGoldBlastFX(Vector3 position)
        {
            var fx = Instantiate(_config.CoinsFX.EffectPrefab);
            fx.transform.position = position;
            Destroy(fx, 1);
        }

        #region Utils
        private Vector3 RandomVector()
        {
            var x = UnityEngine.Random.Range(-0.01f, 0.01f);
            var y = UnityEngine.Random.Range(-0.01f, 0.01f);
            var z = UnityEngine.Random.Range(-0.01f, 0.01f);
            return new Vector3(x, y, z);
        }
        #endregion
    }
}
