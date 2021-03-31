using System;
using System.Collections;
using Core;
using Core.Audio;
using Core.Data;
using Game_Stuff;
using LionStudios;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioType = Core.Audio.AudioType;

namespace Mehanics.Objects
{
    public class Finish : MonoBehaviour
    {
        [SerializeField] private float starsDelay = 5f;

        [SerializeField] private GameObject starsCanvas;
        [SerializeField] private GameObject flashCanvasPrefab;

        [SerializeField] private Color levelTimeColor;
        [SerializeField] private Color levelTimeNewRecordColor;

        private static readonly int PlayerFinish = Animator.StringToHash("Finish");
        private const int TimeToAd = 340;

        private float _timer;
        private float _adTimer;

        private int _startStarsCount;
        private bool _isLevelPass;
        private float _currentLevelTime;

        // Start and End level effects
        private GameObject _flash;
        private Animator _flashAnim;
        private TextMeshProUGUI _timerText;
        private TextMeshProUGUI _levelPassTimeText;
        private TextMeshProUGUI _levelTimeText;

        private AudioManager _audioManager;

        private void Start()
        {
            Analytics.Events.LevelStarted(SceneManager.GetActiveScene().buildIndex, 3);
            
            // foreach (var level in SaveGameManager.LevelTimes)
            // Debug.Log(level);

            var core = GameObject.Find("CoreManagers").transform;
            // _saveGameManager = core.Find("SaveGameManager").GetComponent<SaveGameManager>();
            _audioManager = core.Find("AudioManager").GetComponent<AudioManager>();
            _startStarsCount = GameObject.FindGameObjectsWithTag(Tags.s_Star).Length;
            _levelTimeText = GameObject.Find("LevelTimeText").GetComponent<TextMeshProUGUI>();

            _currentLevelTime = SaveGameManager.LevelTimes[SceneManager.GetActiveScene().buildIndex - 1];

            StartCoroutine(WhiteScreenInStartGameIe());
        }

        private void Update()
        {
            // ToDo Advertisement bugs
            // if (Advertisement.isShowing) return;

            if (_isLevelPass)
            {
                DataController.LevelTime = (float) Math.Round(_timer, 2);
                SaveGameManager.AddValue(DataController.LevelTime, SaveGameManager.LevelTimes);
            }
            else
            {
                _timer += Time.deltaTime;
                _adTimer += Time.deltaTime;
                _levelTimeText.text = $"{_timer:0.00}";

                if (_adTimer < TimeToAd) return;
                _adTimer = 0f;
                // ToDo Advertisement bugs
                // AdsManager.OnShowAd();
            }
        }

        /// <summary>
        /// When play mode is on
        /// </summary>
        /// <returns></returns>
        private IEnumerator WhiteScreenInStartGameIe()
        {
            _flash = Instantiate(flashCanvasPrefab, transform.position, Quaternion.identity);
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            _flashAnim = _flash.GetComponentInChildren<Animator>();
            yield return new WaitForSeconds(1f);
            _flash.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tags.s_Player)) return;
            
            Analytics.Events.LevelComplete(SceneManager.GetActiveScene().buildIndex, DataController.StarsCount);

            if (SceneManager.GetActiveScene().buildIndex + 1 > DataController.Level)
                DataController.Level++;

            StartCoroutine(OpenStarsPanel());
        }

        /// <summary>
        /// Star panel open? when process of triggering is ended
        /// </summary>
        /// <returns></returns>
        private IEnumerator OpenStarsPanel()
        {
            var starsCanvasObject = Instantiate(starsCanvas, transform.position, Quaternion.identity);
            var starsImageBg = starsCanvasObject.transform.GetChild(0).GetChild(0);
            var starsCountNormalized = DataController.StarsCount / (float) _startStarsCount;
            starsCountNormalized = Mathf.Clamp(starsCountNormalized, 0f, 1f);
            _timerText = starsCanvasObject.transform.GetChild(0).Find("TimerText").GetComponent<TextMeshProUGUI>();
            _levelPassTimeText = starsCanvasObject.transform.GetChild(0).Find("LevelPassTime")
                .GetComponent<TextMeshProUGUI>();

            _isLevelPass = true;
            yield return new WaitForSecondsRealtime(Constants.FixedTime);
            _levelPassTimeText.text = $"{DataController.LevelTime:0.00}";
            _levelPassTimeText.color =
                _currentLevelTime > DataController.LevelTime ? levelTimeNewRecordColor : levelTimeColor;

            DataController.LevelTime = 0f;

            SaveGameManager.AddValue(starsCountNormalized, SaveGameManager.LevelStarsCount);
            SaveGameManager.SaveObjects();

            starsImageBg.GetComponentInChildren<TextMeshProUGUI>().text = $"{(int) (starsCountNormalized * 100)}%";
            var fillStar = starsImageBg.GetChild(0).GetComponent<Image>();
            _audioManager.PlayAudio(AudioType.SfxUiStarFilling);
            
            var countDown = 0f;
            while (countDown < starsCountNormalized)
            {
                fillStar.fillAmount = countDown;
                yield return new WaitForEndOfFrame();
                countDown += Time.deltaTime;
            }

            fillStar.fillAmount = starsCountNormalized;
            _audioManager.StopAudio(AudioType.SfxUiStarFilling);
            
            _levelTimeText.transform.parent.gameObject.SetActive(false);

            DataController.StarsCount = 0;

            yield return StartCoroutine(StartCountdown());
        }

        private IEnumerator StartCountdown()
        {
            var currCountdownValue = starsDelay;
            while (currCountdownValue > 0)
            {
                _timerText.text = $"0 : {currCountdownValue}";
                yield return new WaitForSeconds(1.0f);
                currCountdownValue--;
            }

            yield return StartCoroutine(GoToEndLevelIe());
        }

        private IEnumerator GoToEndLevelIe()
        {
            _flash.SetActive(true);
            _flashAnim.SetTrigger(PlayerFinish);
            yield return new WaitForSeconds(.7f);

            // ToDo Advertisement bugs
            // AdsManager.OnShowAd();
            // yield return new WaitForSeconds(.7f);
            // yield return new WaitUntil(() => !Advertisement.isShowing);
            // yield return new WaitForSeconds(.6f);

            DataController.LevelToLoad = SceneManager.GetActiveScene().buildIndex + 1;
            
            // Victory Scene
            var nextLevel = 9;
            // Debug.Log($"nextLevel {nextLevel}");
            var asyncOperation = SceneManager.LoadSceneAsync(nextLevel);
            while (!asyncOperation.isDone) yield return null;
        }
    }
}