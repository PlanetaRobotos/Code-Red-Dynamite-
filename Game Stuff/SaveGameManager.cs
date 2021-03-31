using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Core.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game_Stuff
{
    public class SaveGameManager : MonoBehaviour
    {
        public static float[] LevelStarsCount { get; } = new float[DataController.MaxLevel];
        public static float[] LevelTimes { get; } = new float[DataController.MaxLevel];

        private void OnEnable() => LoadObjects();

        private void OnDisable() => SaveObjects();

        public static void SaveObjects()
        {
            for (int i = 0; i < DataController.MaxLevel; i++)
            {
                var file = File.Open($"{Application.persistentDataPath}/{i}.data", FileMode.OpenOrCreate);
                var timeFile = File.Open($"{Application.persistentDataPath}/{i}.timeData", FileMode.OpenOrCreate);
                var binary = new BinaryFormatter();
                binary.Serialize(file, LevelStarsCount[i]);
                binary.Serialize(timeFile, LevelTimes[i]);
                file.Close();
                timeFile.Close();
            }
        }

        private static void LoadObjects()
        {
            for (int i = 0; i < DataController.MaxLevel; i++)
            {
                if (File.Exists($"{Application.persistentDataPath}/{i}.data"))
                {
                    var file = File.Open($"{Application.persistentDataPath}/{i}.data", FileMode.Open);
                    var binary = new BinaryFormatter();
                    LevelStarsCount[i] = (float) binary.Deserialize(file);
                    file.Close();
                }

                if (File.Exists($"{Application.persistentDataPath}/{i}.timeData"))
                {
                    var timeFile = File.Open($"{Application.persistentDataPath}/{i}.timeData", FileMode.Open);
                    var binary = new BinaryFormatter();
                    LevelTimes[i] = (float) binary.Deserialize(timeFile);
                    timeFile.Close();
                }
            }
        }

        /// <summary>
        /// Logic for added values (player storage)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="array"></param>
        /// <param name="isSaveMax"></param>
        public static void AddValue(float value, float[] array)
        {
            var ind = SceneManager.GetActiveScene().buildIndex - 1;
            var currValue = array[ind];

            if (currValue <= 0 && array == LevelTimes || value <= currValue && array == LevelTimes ||
                value > currValue && array == LevelStarsCount)
                array[ind] = value;
        }

        /// <summary>
        /// If all levels pass with all stars - Hardcore Mode can be use
        /// </summary>
        /// <returns></returns>
        public static bool CanBeHardcore() =>
            !LevelStarsCount.Any(levelCount => Mathf.Abs(levelCount) < 0.01f);

        public static void OnChangeHardcore()
        {
            if (!CanBeHardcore()) return;
            DataController.IsHardcoreMode = !DataController.IsHardcoreMode;
        }
    }
}