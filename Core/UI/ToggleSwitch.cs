using Core.Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AudioType = Core.Audio.AudioType;

namespace Core.UI
{
    public class ToggleSwitch : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private RectTransform toggleIndicator;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI switchText;

        [SerializeField] private Color onColor;
        [SerializeField] private Color offColor;
        
        public string key;
        public string onText = "On";
        public string offText = "Off";

        [SerializeField] private float tweenTime = 0.25f;

        private bool IsOn { get; set; }
        public bool IsInteractable { get; set; } = true;
        
        private float _offX;
        private float _onX;

        private AudioManager _audioManager;

        public delegate void ValueChanged(bool value);

        public event ValueChanged ValueEvent;

        private void Start()
        {
            _offX = toggleIndicator.anchoredPosition.x; // start position
            _onX = backgroundImage.rectTransform.rect.width - toggleIndicator.rect.width;
            _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }

        /// <summary>
        /// Settings and toggle switch
        /// </summary>
        /// <param name="value"></param>
        /// <param name="playSfx"></param>
        public void Toggle(bool value, bool playSfx = true)
        {
            if (value == IsOn) return;
            if(!IsInteractable) return;
            
            IsOn = value;

            ToggleColor(IsOn);
            ToggleText(IsOn);
            MoveIndicator(IsOn);

            if (playSfx)
                _audioManager.PlayAudio(AudioType.SfxUiSwitch);

            ValueEvent?.Invoke(IsOn);
        }

        private void ToggleColor(bool value) =>
            backgroundImage.DOColor(value ? onColor : offColor, tweenTime);

        private void ToggleText(bool value) =>
            switchText.text = value ? onText : offText;

        private void MoveIndicator(bool value) =>
            toggleIndicator.DOAnchorPosX(value ? _onX : _offX, tweenTime);


        public void OnPointerDown(PointerEventData eventData) =>
            Toggle(!IsOn); // Flips the switch when clicked
    }
}