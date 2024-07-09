using deVoid.UIFramework;

namespace Markins.Runtime.Game.GUI.Contexts
{
    [System.Serializable]
    public class WinLevelWindowProps : WindowProperties
    {
        public int Level;
        public int CountReward;
        public bool HasReward;
    }
}