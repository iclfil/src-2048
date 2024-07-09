using System.Collections;
using deVoid.UIFramework;
using Supyrb;
using UnityEngine;
using UnityEngine.UI;

public class LostLevelWindowController : AWindowController
{

    public Button AgainButton;

    public void OnClickAgainButton()
    {
        Signals.Get<OnClickAgainButtonSignal>().Dispatch();
    }
}

public class OnClickAgainButtonSignal : Signal
{
}
