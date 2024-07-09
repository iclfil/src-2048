using System;
using DG.Tweening;
using Markins.Runtime.Game.Controllers;
using UnityEngine;

public interface IMatchChipsAnimation
{
    Sequence MatchAnimation(ChipController firstChip, ChipController secondChip, Vector3 matchPosition, Action nextSpawnChip);
}