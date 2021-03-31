using System;
using System.Collections;
using Core.Data;
using LionStudios;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.UI
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        private Animator _panelAnim;
        private static readonly int GoOut = Animator.StringToHash("GoOut");

        private void Start()
        {
            _panelAnim = pausePanel.GetComponent<Animator>();
            pausePanel.SetActive(false);
        }

        public void OnPauseGame()
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }

        public void OnResumeGame() => 
            StartCoroutine(OutIe(false));
        
        public void OnRestartGame() => 
            StartCoroutine(OutIe(true, SceneManager.GetActiveScene().name));
        
        public void OnLevelRestartLion() =>
            Analytics.Events.LevelRestart(SceneManager.GetActiveScene().buildIndex, DataController.StarsCount);

        public void OnGoHome() => 
            StartCoroutine(OutIe(true, "MainMenuScene"));

        private IEnumerator OutIe(bool isLoadScene, string sceneName = "")
        {
            _panelAnim.SetTrigger(GoOut);
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(1f));
            Time.timeScale = 1;
            if (isLoadScene)
            {
                var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
                while (!asyncOperation.isDone) yield return null;
            }
            else
                pausePanel.SetActive(false);
        }
    }
}