using System;
using UnityEngine;

namespace Markins.Runtime.Game.Controllers
{
    [Serializable]
    public class Match
    {
        public ChipController First;
        public ChipController Second;
        public Vector3 MatchPosition;
        public bool Calculate;
    }
}