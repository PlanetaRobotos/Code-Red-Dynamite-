using System;
using System.Collections;
using Cinemachine;
using Cinemachine.PostFX;
using Core.Audio;
using Core.UI;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using AudioType = Core.Audio.AudioType;

namespace Core
{
    public class CinemachineAbilities : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        
        [Header("Shake Screen Effect")]
        [SerializeField] private float shakeIntensity = 5f;
        [SerializeField] private float shakeDelay = 0.5f;


        [Header("Freeze Screen Effects")] [SerializeField]
        private float freezeDelay = 0.5f;
        [SerializeField] private float screenBlur = 100f;
        [SerializeField] private float freezeColorIntensity = 0.7f;


        private CinemachineBasicMultiChannelPerlin _perlin;
        private PostProcessProfile _postProcessingProfile;
        private HoldButtonImpulse _impulseBtn;
        private AudioManager _audioManager;
        private AbilitiesUi _abilitiesUi;

        private DepthOfField _depthOfField;
        private Vignette _vignette;

        private void Awake()
        {
            _impulseBtn = GameObject.Find("ImpulseBgBtn").GetComponent<HoldButtonImpulse>();
            _impulseBtn.OnPointerUpEvent += OnCameraShake;
        }

        private void Start()
        {
            _perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _postProcessingProfile = virtualCamera.GetComponent<CinemachinePostProcessing>().m_Profile;

            _audioManager = GameObject.FindWithTag(Tags.s_Core).GetComponent<AudioManager>();
            _abilitiesUi = GameObject.Find("InputCanvas").GetComponent<AbilitiesUi>();

            _postProcessingProfile.TryGetSettings(out DepthOfField depthOfField);
            _postProcessingProfile.TryGetSettings(out Vignette vignette);
            _depthOfField = depthOfField;
            _vignette = vignette;
            
            _vignette.intensity.value = depthOfField.focalLength.value = 0f;
        }

        private void OnCameraShake()
        {
            if (_abilitiesUi.ImpulseFilledAmount < 1f) return;
            _audioManager.PlayAudio(AudioType.SfxUiShakeCamera);
            StartCoroutine(TimerIe());
        }

        private IEnumerator TimerIe()
        {
            var timer = 0f;
            
            while (timer <= shakeDelay)
            {
                _perlin.m_AmplitudeGain = Mathf.Lerp(shakeIntensity, 0f, timer / shakeDelay);
                timer += Time.deltaTime;
                yield return null;
            }

            // impulseFilled.fillAmount = 0;
            _perlin.m_AmplitudeGain = 0f;
        }

        /// <summary>
        /// Freeze of camera
        /// </summary>
        public void OnCameraFreeze()
        {
            if (_abilitiesUi.FreezeFilledAmount < 1f) return;

            _audioManager.PlayAudio(AudioType.SfxUiFreezeCamera);
            Time.timeScale = 1;
            StartCoroutine(FreezeIe());
        }

        private IEnumerator FreezeIe()
        {
            var timer = 0f;

            while (timer <= freezeDelay)
            {
                _depthOfField.focalLength.value = Mathf.Lerp(screenBlur, 0f, timer / freezeDelay);
                _vignette.intensity.value = Mathf.Lerp(freezeColorIntensity, 0f, timer / freezeDelay);
                timer += Time.deltaTime;
                yield return null;
            }

            _vignette.intensity.value = _depthOfField.focalLength.value = 0f;
        }
    }
}