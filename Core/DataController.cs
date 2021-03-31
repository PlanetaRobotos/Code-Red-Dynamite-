using System;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    namespace Data
    {
        public static class DataController
        {
            // private const string DataScore = "score";
            // private const string DataHighscore = "highscore";

            private const string DataLevel = "level";
            private const string DataLevelToLoad = "levelToLoad";
            private const string DataUnit = "unit";
            private const string DataIsMusicOn = "music";
            private const string DataIsSoundsOn = "sounds";
            private const string DataHardcoreMode = "hardcore";
            private const string DataIsNotFirstHardcore = "isNotFirstHardcore";
            private const string DataLanguage = "language";
            private const string DataDeathCounter = "deathCounter";
            private const int DefaultInt = 0;

            public const int MaxLevel = 6;
            public static string Language
            {
                get => GetString(DataLanguage);
                set => SaveString(DataLanguage, value);
            }

            // How many levels per Unit 
            public const int MaxLevelsInUnit = 6;

            // Number of Unit
            public static int UnitNumber { get; private set; } = 1;

            // How many stars is Player collected
            public static int StarsCount { get; set; }
            public static float LevelTime { get; set; }
            
            public static float Health { get; set; }
            
            //Player death count in hardcore mode
            public static int DeathCounter
            {
                get => GetInt(DataDeathCounter);
                set => SaveInt(DataDeathCounter, value);
            }


            #region Properties

            public static int Level
            {
                get => GetInt(DataLevel);
                set
                {
                    value = Mathf.Clamp(value, MinUnitLevel, MaxLevel);
                    SaveInt(DataLevel, value);
                }
            }
            
            public static int LevelToLoad
            {
                get => GetInt(DataLevelToLoad);
                set => SaveInt(DataLevelToLoad, value);
            }


            public static bool IsMusicOn
            {
                get => GetInt(DataIsMusicOn) == 1;
                set => SaveInt(DataIsMusicOn, value ? 1 : 0);
            }

            public static bool IsSoundsOn
            {
                get => GetInt(DataIsSoundsOn) == 1;
                set => SaveInt(DataIsSoundsOn, value ? 1 : 0);
            }
            
            public static bool IsNotFirstHardcore
            {
                get => GetInt(DataIsNotFirstHardcore) == 1;
                set => SaveInt(DataIsNotFirstHardcore, value ? 1 : 0);
            }

            /// <summary>
            /// If its first game - reset all values
            /// </summary>
            public static void FirstGame()
            {
                var hasPlayed = GetInt("HasPlayed");
                // if it's NOT first game
                if (hasPlayed != 0)
                {
                    UnitNumber = GetInt(DataUnit);
                }
                // if it's first game
                else
                {
                    Level = UnitNumber = 1;
                    IsMusicOn = IsSoundsOn = true;
                    SaveInt(DataUnit, UnitNumber);
                    SaveInt("HasPlayed", 1);
                    SaveInt(DataHardcoreMode, 0);
                    IsNotFirstHardcore = false;

                    var startLanguage = "English";
                    var systemLanguage = Application.systemLanguage.ToString();
                    foreach (var language in Localization.Languages)
                        if (language == systemLanguage)
                            startLanguage = systemLanguage;

                    Language = startLanguage;
                    Debug.Log(startLanguage);
                }
            }

            public static bool IsHardcoreMode
            {
                get => GetInt(DataHardcoreMode) == 1;
                set => SaveInt(DataHardcoreMode, value ? 1 : 0);
            }

            public static int MinUnitLevel => (UnitNumber - 1) * MaxLevelsInUnit + 1;
            public static int MaxUnitLevel => MinUnitLevel + MaxLevelsInUnit - 1;

            public static void SetUnit(Slider levelSlider, int unitChangeDelta)
            {
                UnitNumber = Mathf.Clamp(UnitNumber + unitChangeDelta, 1, 2);
                SaveInt("data", UnitNumber);
                levelSlider.minValue = MinUnitLevel;
                levelSlider.maxValue = MaxUnitLevel;
                levelSlider.value = unitChangeDelta > 0 ? MinUnitLevel : MaxUnitLevel;
            }

            // public int Highscore
            // {
            //     get => GetInt(DataHighscore);
            //     private set => SaveInt(DataHighscore, value);
            // }

            // public int Score
            // {
            //     get => GetInt(DataScore);
            //     set
            //     {
            //         // soft clamp value at 0
            //         if (value < 0) value = 0;
            //
            //         SaveInt(DataScore, value);
            //         var score = Score;
            //         if (score > Highscore) 
            //             Highscore = score;
            //     }
            // }

            #endregion


            #region Private Functions

            private static void SaveInt(string data, int value) =>
                PlayerPrefs.SetInt(data, value);

            private static int GetInt(string data) =>
                PlayerPrefs.GetInt(data, DefaultInt);
            
            private static void SaveFloat(string data, float value) =>
                PlayerPrefs.SetFloat(data, value);

            private static float GetFloat(string data) =>
                PlayerPrefs.GetFloat(data, DefaultInt);
            
            private static void SaveString(string data, string value) =>
                PlayerPrefs.SetString(data, value);

            private static string GetString(string data) =>
                PlayerPrefs.GetString(data);

            #endregion
        }
    }
}