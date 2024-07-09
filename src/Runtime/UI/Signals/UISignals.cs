using Supyrb;

namespace Markins.Runtime.Game.GUI.MySignals
{
    public class UserSelectNextLevelSignal : Signal<bool> { }
    public class UserExplodeTargetChipSignal : Signal<bool> { }
    public class OnClickBackButtonSignal : Signal { }
    public class OnClickOpenSettingsSignal : Signal { }
    public class OnClickCloseTutorialSignal : Signal { }
}