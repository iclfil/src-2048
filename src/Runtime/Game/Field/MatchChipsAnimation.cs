using System;
using DG.Tweening;
using Markins.Runtime.Game.Controllers;
using Markins.Runtime.Game.Storage;
using Markins.Runtime.Game.Storage.Models;
using Supyrb;
using UnityEngine;

public class MatchChipsAnimation : IMatchChipsAnimation
{
    public Sequence MatchAnimation(ChipController firstChip, ChipController secondChip, Vector3 matchPosition, Action nextSpawnChip)
    {
        var mergeAnimation = DOTween.Sequence();

        mergeAnimation.Insert(0, secondChip.transform.DOMove(matchPosition, 0.20f));
        mergeAnimation.Insert(0, firstChip.transform.DOMove(matchPosition, 0.20f));

        mergeAnimation.Insert(0.2f, secondChip.transform.DOScale(0, 0.3f).SetEase(Ease.InBack));
        mergeAnimation.InsertCallback(0.5f, firstChip.Destroy);
        mergeAnimation.Insert(0.2f, firstChip.transform.DOScale(0, 0.3f).SetEase(Ease.InBack));
        mergeAnimation.InsertCallback(0.5f, secondChip.Destroy);

        mergeAnimation.InsertCallback(0.33f, () => nextSpawnChip());

        mergeAnimation.InsertCallback(
            0.35f,
            () => Signals.Get<PlayEffectSignal>().Dispatch(EffectCollection.EffectsEvent.MATCH_CHIPS, matchPosition));

        AudioSystem.Instance.PlaySound("DM-CGS-32");
        MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);

        return mergeAnimation;
    }
}