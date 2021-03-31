using System.IO;
using Core.Data;
using Game_Stuff;
using UnityEditor;
using UnityEngine;

namespace Core.Editor
{
    /// <summary>
    /// Tool block// Very useful
    /// </summary>
    public static class ToolsMenuItems
    {
        [MenuItem("Tools/Clear PlayerPrefs")]
        private static void ClearPlayerPrefs()
        {
            Debug.Log("Player Prefs storage is clean like your ass");
            PlayerPrefs.DeleteAll();
        }
        
        [MenuItem("Tools/Level System/Level Up")]
        private static void LevelUp()
        {
            DataController.Level++;
            Debug.Log("Level up. Current level " + DataController.Level);
        }
        [MenuItem("Tools/Level System/Unlock all Level")]
        private static void UnlockAllLevels()
        {
            DataController.Level = DataController.MaxLevel;
            Debug.Log("Level up. Current level" + DataController.Level);
        }
        
        [MenuItem("Tools/Stars System/Add All Data")]
        private static void AddAllStars()
        {
            for (var i = 0; i < SaveGameManager.LevelStarsCount.Length; i++) 
                SaveGameManager.LevelStarsCount[i] = 1f;
            SaveGameManager.SaveObjects();
            Debug.Log("All Stars Added");
        }
        
        [MenuItem("Tools/Stars System/Reset Data")]
        private static void ResetObjects()
        {
            for (var i = 0; i < SaveGameManager.LevelStarsCount.Length; i++)
            {
                if (File.Exists($"{Application.persistentDataPath}/{i}.data"))
                    File.Delete($"{Application.persistentDataPath}/{i}.data");
                if (File.Exists($"{Application.persistentDataPath}/{i}.timeData"))
                    File.Delete($"{Application.persistentDataPath}/{i}.timeData");
            }
        }

        [MenuItem("Tools/Reset All Data")]
        private static void ResetAllData()
        {
            ResetObjects();
            ClearPlayerPrefs();
        }
    }
}