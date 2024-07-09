using System;
using System.Collections.Generic;
using System.Text;
using Markins.Runtime.Game.Controllers;

namespace Markins.Runtime.Game.Storage.Converter
{
    public class DataConverter
    {
        // Глобальный словарь для преобразования данных
        private static Dictionary<char, string> chipMapping = new Dictionary<char, string>
        {
            { '2', "Heart" },
            { '3', "Hexagon" },
            { '4', "Pentho" },
            { '5', "Box" },
            { '6', "Star" }
        };

        private static Dictionary<char, string> effectMapping = new Dictionary<char, string>
        {
            { '1', "Toxic" },
            { '2', "Love" },
            { '3', "Clouds" },
        };

        private static Dictionary<char, string> fieldMapping = new Dictionary<char, string>
        {
            { '1', "Football" },
            { '2', "Hockey" },
            { '3', "Minecraft" },
        };

        private static Dictionary<char, string> gameThemesMapping = new Dictionary<char, string>
        {
            { '1', "Forest" },
            { '2', "UFO" },
            { '3', "Mine" },
        };

        public static int DataToTargetChip(string data)
        {
            return int.Parse(data);
        }

        public static int DataToBestScore(string data)
        {
            return int.Parse(data);
        }

        public static int DataToScore(string data)
        {
            return int.Parse(data);
        }

        public static int DataToMoney(string data)
        {
            return int.Parse(data);
        }

        public static int DataToLevel(string data)
        {
            return int.Parse(data);
        }

        public static List<string> DataToGameThemes(string data)
        {
            var themes = new List<string>();

            foreach (var character in data)
            {
                if (gameThemesMapping.ContainsKey(character))
                {
                    themes.Add(gameThemesMapping[character]);
                }
            }

            return themes;
        }

        public static string GameThemesToData(List<string> gameThemes)
        {
            var str = "";

            foreach (var fieldName in gameThemes)
            {
                foreach (var kvp in gameThemesMapping)
                {
                    if (kvp.Value == fieldName)
                    {
                        str += kvp.Key + ",";
                        break;
                    }
                }
            }

            return str.TrimEnd(','); // Убираем последнюю запятую, если она есть
        }

        public static List<string> DataToEffectSkins(string data)
        {
            var effects = new List<string>();

            foreach (var character in data)
            {
                if (effectMapping.ContainsKey(character))
                {
                    effects.Add(effectMapping[character]);
                }
            }

            return effects;
        }

        public static string EffectsToData(List<string> effects)
        {
            var str = "";

            foreach (var fieldName in effects)
            {
                foreach (var kvp in effectMapping)
                {
                    if (kvp.Value == fieldName)
                    {
                        str += kvp.Key + ",";
                        break;
                    }
                }
            }

            return str.TrimEnd(','); // Убираем последнюю запятую, если она есть
        }

        public static List<string> DataToFieldsSkins(string data)
        {
            var fields = new List<string>();

            foreach (var character in data)
            {
                if (fieldMapping.ContainsKey(character))
                {
                    fields.Add(fieldMapping[character]);
                }
            }

            return fields;
        }

        public static string FieldSkinsToData(List<string> fields)
        {
            var str = "";

            foreach (var fieldName in fields)
            {
                foreach (var kvp in fieldMapping)
                {
                    if (kvp.Value == fieldName)
                    {
                        str += kvp.Key + ",";
                        break;
                    }
                }
            }

            return str.TrimEnd(','); // Убираем последнюю запятую, если она есть
        }

        public static List<string> DataToChips(string data)
        {
            var chips = new List<string>();

            foreach (var character in data)
            {
                if (chipMapping.ContainsKey(character))
                {
                    chips.Add(chipMapping[character]);
                }
            }

            return chips;
        }

        public static string ChipsSkinsToData(List<string> chips)
        {
            var str = "";

            foreach (var chip in chips)
            {
                foreach (var kvp in chipMapping)
                {
                    if (kvp.Value == chip)
                    {
                        str += kvp.Key + ",";
                        break;
                    }
                }
            }

            return str.TrimEnd(','); // Убираем последнюю запятую, если она есть
        }

        public static string BestScoreToData(int bestScore)
        {
            return bestScore.ToString();
        }

        public static string LevelToData(int level)
        {
            return level.ToString();
        }

        public static string MoneyToData(int money)
        {
            return money.ToString();

        }

        public static string ScoreToData(int score)
        {
            return score.ToString();
        }

        public static string TargetChipToData(int targetChip)
        {
            return targetChip.ToString();
        }

        public static string FieldChipsToData(IEnumerable<ChipController> chips)
        {
            StringBuilder data = new StringBuilder();
            foreach (var chip in chips)
                data.Append(chip.Power + ":" + Math.Round(chip.transform.position.x, 3) + "+" +
                            Math.Round(chip.transform.position.z, 3) + ";");

            return data.ToString();
        }

        public static IEnumerable<ChipData> DataToFieldChips(string data)
        {
           var chips = new List<ChipData>();

            // Получили все элементы отдельно.
            string[] piecesStr = null;
            piecesStr = data.Split(';');
            char[] ext = { ':', '+' };


            for (int i = 0; i < piecesStr.Length - 1; i++)
            {
                var chipData = new ChipData();
                string[] d = null;
                d = piecesStr[i].Split(ext);
                chipData.Power = int.Parse(d[0]);
                chipData.PosX = float.Parse(d[1]);
                chipData.PosZ = float.Parse(d[2]);
                chips.Add(chipData);
            }

            return chips;
        }

        public struct ChipData
        {
            public int Power;
            public float PosZ;
            public float PosX;
        }

    }
}