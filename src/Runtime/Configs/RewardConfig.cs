using UnityEngine;

namespace Markins.Runtime.Game
{
    [CreateAssetMenu(fileName = "Collectables", menuName = "Game/Create Collectables Settings")]
    public class RewardConfig : ScriptableObject
    {
        public int CountRewardCrystals = 10;
        public int Count2XRewardCrystals = 20;
        public CollisionEnterNotifier CrystalViewPrefab;

        public int RewardCoinsForBounceChips = 1;
        public CollisionEnterNotifier CoinViewPrefab;
        [Header("FXs")]
        public EffectConfig SelectTargetFx;
        public EffectConfig DestroyTargetFx;
        public EffectConfig CoinsFX;

    }
}