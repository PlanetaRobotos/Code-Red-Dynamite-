using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using Core.Audio;
using Core.Data;
using Game_Stuff;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioType = Core.Audio.AudioType;
using Debug = UnityEngine.Debug;

namespace Core.UI
{
    public class LevelsManager : MonoBehaviour
    {
        private AudioManager _audioManager;

        [SerializeField] private GameObject firstGamePanel;
        [SerializeField] private Button levelNumberButton;
        [SerializeField] private Image levelStarFilledImg;
        [SerializeField] private Text levelNumberText;
        [SerializeField] private Slider levelSlider;
        [SerializeField] private Animator guideBtnAnim;
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private TextMeshProUGUI hardcoreText;
        [SerializeField] private TextMeshProUGUI levelTimeText;
        [SerializeField] private Sprite[] levelBtnBGs;
        [SerializeField] private string[] unitNames;

        // [SerializeField] private GameObject bulletPrefab;

        [SerializeField] private Color levelTimeColor;
        [SerializeField] private Color levelTimeAllStarsColor;

        private int _unitIndex;
        // private bool _canClick;

        // private static readonly int ShouldWork = Animator.StringToHash("ShouldWork");


        #region Unity Functions

        private void Awake()
        {
            firstGamePanel.SetActive(PlayerPrefs.GetInt("HasPlayed") == 0);
            DataController.FirstGame();
        }

        private void Start() => Init();

        #endregion


        #region Public Functions

        /// <summary>
        /// When I change slider value
        /// </summary>
        public void ChangeLevelSlider()
        {
            // if (!_canClick) return;

            float value;
            const int unit = DataController.MaxLevelsInUnit;
            levelNumberText.text = ((value = levelSlider.value) - unit *
                (DataController.MaxUnitLevel / unit - 1)).ToString(CultureInfo.InvariantCulture);
            levelNumberButton.interactable = value <= DataController.Level;
            var valNorm = SaveGameManager.LevelStarsCount[(int) value - 1];
            StartCoroutine(SmoothFillingImage(valNorm, levelStarFilledImg));
            levelTimeText.text = Math.Abs(SaveGameManager.LevelTimes[(int) value - 1]) < 0.01
                ? "0:00"
                : SaveGameManager.LevelTimes[(int) value - 1].ToString(CultureInfo.InvariantCulture);
            levelTimeText.color = valNorm is 1 ? levelTimeAllStarsColor : levelTimeColor;
            _audioManager.PlayAudio(AudioType.SfxUiLevelChange);
        }

        public void OnStartGame() => StartCoroutine(StartGameIe());

        public void OnChangeUnit(string direction)
        {
            // if (!_canClick) return;

            switch (direction)
            {
                case "Left":
                    // _unitIndex = --_unitIndex < 0 ? levelBtnBGs.Length - 1 : _unitIndex;
                    if (_unitIndex <= 0) return;
                    --_unitIndex;
                    DataController.SetUnit(levelSlider, -1);
                    break;
                case "Right":
                    // _unitIndex = ++_unitIndex >= levelBtnBGs.Length ? 0 : _unitIndex;
                    if (_unitIndex >= levelBtnBGs.Length - 1) return;
                    ++_unitIndex;
                    DataController.SetUnit(levelSlider, 1);
                    break;
                default:
                    Debug.LogWarning("Wrong Direction");
                    break;
            }

            levelNumberButton.GetComponent<Image>().sprite = levelBtnBGs[_unitIndex];

            hardcoreText.enabled = DataController.IsHardcoreMode;
            unitNameText.text = unitNames[_unitIndex];
        }

        public void OnGoToGuide() =>
            StartCoroutine(GoToGuide());

        public void OnOpenLevelPanelInit()
        {
            float value;
            const int unit = DataController.MaxLevelsInUnit;
            levelNumberText.text = ((value = levelSlider.value) - unit *
                (DataController.MaxUnitLevel / unit - 1)).ToString(CultureInfo.InvariantCulture);
            levelNumberButton.GetComponent<Image>().sprite = levelBtnBGs[_unitIndex];

            hardcoreText.enabled = DataController.IsHardcoreMode;
            unitNameText.text = unitNames[_unitIndex];

            var valNorm = SaveGameManager.LevelStarsCount[(int) value - 1];
            StartCoroutine(SmoothFillingImage(valNorm, levelStarFilledImg));
            levelTimeText.text = Math.Abs(SaveGameManager.LevelTimes[(int) value - 1]) < 0.01
                ? "0:00"
                : SaveGameManager.LevelTimes[(int) value - 1].ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Private Functions

        private static IEnumerator GoToGuide()
        {
            yield return new WaitForSeconds(.4f);
            var asyncOperation = SceneManager.LoadSceneAsync("GuideScene2");
            while (!asyncOperation.isDone) yield return null;
        }

        private void Init()
        {
            // levelSlider.interactable = _canClick;

            _audioManager = GameObject.FindWithTag(Tags.s_Core).GetComponent<AudioManager>();

            // To Change Unit. For ex. it's a 7 level - we need to change unit to second
            if (DataController.Level > DataController.MaxUnitLevel)
                DataController.SetUnit(levelSlider, 1);

            _unitIndex = DataController.UnitNumber - 1;

            levelSlider.value = DataController.Level;
        }

        /// <summary>
        /// Work when I click on active level. 
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartGameIe()
        {
            // if (!_canClick)
            // {
            //     guideBtnAnim.SetTrigger(ShouldWork);
            //     yield break;
            // }

            yield return new WaitForSeconds(.4f);
            var levelNumber = levelSlider.value;
            if (levelNumber > DataController.Level) yield break;
            DataController.LevelToLoad = (int) levelNumber;
            var asyncOperation = SceneManager.LoadSceneAsync("StartLevel");
            while (!asyncOperation.isDone) yield return null;
        }

        private static IEnumerator SmoothFillingImage(float valueNormalized, Image fillImage)
        {
            fillImage.fillAmount = valueNormalized;
            yield return null;
        }

        #endregion
    }
}