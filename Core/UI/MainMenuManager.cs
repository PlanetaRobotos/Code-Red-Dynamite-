using System.Collections;
using Core.Audio;
using Core.Data;
using Game_Stuff;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioType = Core.Audio.AudioType;

namespace Core.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject playPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject levelsPanel;

        [Header("Unity Toggles")] [SerializeField]
        private Toggle musicToggle;

        [SerializeField] private Toggle soundsToggle;

        [Header("Switch Toggles")] [SerializeField]
        private ToggleSwitch gameModeSwitch;
        
        [SerializeField] private Color textIsInteractableColor;
        [SerializeField] private Color textIsNotInteractableColor;
        
        [SerializeField] private GameObject bulletPrefab;

        private readonly AudioSource[] _soundsSource = new AudioSource[2];

        private Animator _settingsAnim;
        private static readonly int Close = Animator.StringToHash("Close");

        private AudioManager _audioManager;

        private void Start()
        {
            GetComponent<Canvas>().enabled = true;
            
            _settingsAnim = settingsPanel.GetComponent<Animator>();
            playPanel.SetActive(true);
            settingsPanel.SetActive(false);
            levelsPanel.SetActive(false);

            _audioManager = GameObject.FindWithTag(Tags.s_Core).GetComponent<AudioManager>();
            _soundsSource[0] = _audioManager.transform.Find("SFX").GetComponent<AudioSource>();
            _soundsSource[1] = _audioManager.transform.Find("UI SFX").GetComponent<AudioSource>();
            musicToggle.isOn = DataController.IsMusicOn;
            soundsToggle.isOn = DataController.IsSoundsOn;
            foreach (var soundsSource in _soundsSource)
                soundsSource.mute = !soundsToggle.isOn;

            var isMode = DataController.IsHardcoreMode;
            var canMode = SaveGameManager.CanBeHardcore();
            
            if (!DataController.IsNotFirstHardcore && canMode)
            {
                DataController.IsNotFirstHardcore = true;
                SaveGameManager.OnChangeHardcore();
                gameModeSwitch.Toggle(true, false);
                OnChangeAudioTrack();
            }
            else
                gameModeSwitch.Toggle(isMode, false);

            gameModeSwitch.GetComponent<Button>().interactable =
                gameModeSwitch.IsInteractable = canMode;
            gameModeSwitch.GetComponentInChildren<TextMeshProUGUI>().color =
                canMode ? textIsInteractableColor : textIsNotInteractableColor;

            OnChangeHardcoreBullet();
        }

        /// <summary>
        /// When we clicked on switch toggle
        /// </summary>
        public void OnChangeAudioTrack()
        {
            _audioManager.PlayAudio(DataController.IsHardcoreMode ? AudioType.St02 : AudioType.St01);
            
            if(DataController.IsMusicOn) return;
            _audioManager.StopAudio(DataController.IsHardcoreMode ? AudioType.St02 : AudioType.St01);
        }

        public void OnChangeHardcoreBullet() => 
            bulletPrefab.layer = DataController.IsHardcoreMode ? 9 : 0;

        public void OnQuit() =>
            StartCoroutine(QuitIe());

        public void OnOpenSettings() =>
            StartCoroutine(OpenSettingsIe());

        public void OnCloseSettings() =>
            StartCoroutine(CloseSettingsIe());

        public void OnChangeActiveLevelWindow()
        {
            levelsPanel.SetActive(!levelsPanel.activeSelf);
            playPanel.SetActive(!playPanel.activeSelf);
        }

        /// <summary>
        /// For slider // Changing properties
        /// </summary>
        /// <param name="musicOrSounds"></param>
        public void OnChangeValue(string musicOrSounds)
        {
            _audioManager.PlayAudio(AudioType.SfxUiSwitch);
            switch (musicOrSounds)
            {
                case "Music":
                    DataController.IsMusicOn = musicToggle.isOn;
                    if (musicToggle.isOn)
                        _audioManager.PlayAudio(DataController.IsHardcoreMode ? AudioType.St02 : AudioType.St01, true,
                            1f);
                    else
                        _audioManager.StopAudio(DataController.IsHardcoreMode ? AudioType.St02 : AudioType.St01, true,
                            1f);
                    break;
                case "Sounds":
                    StartCoroutine(LateChangeIe());
                    break;
            }
        }

        private IEnumerator QuitIe()
        {
            // Time.timeScale = 1;
            yield return null;
            Application.Quit();
        }

        private IEnumerator OpenSettingsIe()
        {
            settingsPanel.SetActive(true);
            yield return null;
            playPanel.SetActive(false);
        }

        private IEnumerator CloseSettingsIe()
        {
            _settingsAnim.SetTrigger(Close);
            yield return new WaitForSeconds(.7f);
            settingsPanel.SetActive(false);
            playPanel.SetActive(true);
        }

        private IEnumerator LateChangeIe()
        {
            yield return new WaitForSecondsRealtime(.2f);
            DataController.IsSoundsOn = soundsToggle.isOn;
            foreach (var soundsSource in _soundsSource)
                soundsSource.mute = !soundsToggle.isOn;
        }
    }
}