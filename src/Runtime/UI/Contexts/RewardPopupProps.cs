using System;
using deVoid.UIFramework;

namespace Markins.Runtime.Game.GUI.Contexts
{
    [Serializable]
    public class RewardPopupProps : WindowProperties
    {
        public bool HasAds = false;
        public int CountReward = 10;
    }
}