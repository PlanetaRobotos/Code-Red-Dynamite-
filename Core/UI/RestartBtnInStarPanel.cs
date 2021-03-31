using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.UI
{
    public class RestartBtnInStarPanel : MonoBehaviour
    {
        private PauseManager _pauseManager;

        private void Start() => 
            _pauseManager = GameObject.Find("PauseCanvas").GetComponent<PauseManager>();

        public void OnRestart() =>
            _pauseManager.OnRestartGame();
    }
}