using deVoid.UIFramework;
using Markins.Runtime.Game.GUI.MySignals;
using Supyrb;

public class TutorialWindowController : AWindowController
{
    public void OnClickCloseTutorial()
    {
        Signals.Get<OnClickCloseTutorialSignal>().Dispatch();
    }
}
