using System;
using UnityEngine;

namespace Assets.markins._2048.Runtime.Game.Services
{
    public class PowerToStringFormatterService : IPowerToStringFormatterService
    {
        public string FormatPowerOfTwo(int power)
        {
            if (power < 23)
            {
                return Mathf.Pow(2, power).ToString();
            }

            string[] prefixes = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "W", "Z", "KI", "LO" }; // и так далее
            int prefixIndex = (power - 23) / 14;

            if (prefixIndex >= prefixes.Length) prefixIndex = prefixes.Length - 1;

            double magnitude = power * Math.Log10(2);

            double reducedNumber = Math.Pow(10, magnitude % 3);
            return $"{reducedNumber:0.#}{prefixes[prefixIndex]}";

        }
    }
}
