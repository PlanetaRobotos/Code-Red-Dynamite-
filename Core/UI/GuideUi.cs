using System.Collections;
using Core.Data;
using LionStudios;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.UI
{
    public class GuideUi : MonoBehaviour
    {
        public void OnStartGame() =>
            StartCoroutine(StartGame());

        private static IEnumerator StartGame()
        {
            Analytics.Events.TutorialComplete();
            yield return new WaitForSeconds(.4f);
            var asyncOperation = SceneManager.LoadSceneAsync($"Level{DataController.Level}");
            while (!asyncOperation.isDone) yield return null;
        }
    }
}