using System;
using System.Collections;
using Core.Data;
using Core.DataObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core.UI
{
    public class PlayerUi : MonoBehaviour
    {
        [Header("Freeze Ability Properties")] [SerializeField]
        private float freeRotationSpeed = 1f;

        [Tooltip("When Freeze Ability hold")] [SerializeField]
        private float freezeRotationSpeed = 5f;

        [SerializeField] private float freezeDelay = 1.5f;

        [Header("Impulse Ability Properties")] [SerializeField]
        private float increaseImpulseForceSpeed = 1f;

        [SerializeField] private ValuesRange impulseForceRange;

        private AbilitiesUi _abilitiesUi;
        private Image _impulseForceFilledImage;
        private GameObject _impulseForceBar;

        private Rigidbody2D _rb;
        private Transform _endOfGun;

        private readonly float[] _values = {-.3f, .3f};

        private float _rotationSpeed;
        private int _sign = 1;

        // When User holding impulse btn this value increasing
        private float _impulseForce;

        // private HoldButtonImpulse _impulseBtn;
        private HoldButton _rotationBtn;

        private void Start()
        {
            Init();
            AddListenersForButtons();
        }

        private void Init()
        {
            _rb = GetComponent<Rigidbody2D>();
            _endOfGun = transform.GetChild(0);

            _abilitiesUi = GameObject.Find("InputCanvas").GetComponent<AbilitiesUi>();

            _impulseForceBar = GameObject.Find("ImpulseForceBgImg");
            _impulseForceFilledImage = _impulseForceBar.transform.GetChild(0).GetComponent<Image>();
            _impulseForceFilledImage.fillAmount = 0;
            _impulseForceBar.SetActive(false);

            _rotationSpeed = freeRotationSpeed;
        }

        private void AddListenersForButtons()
        {
            GameObject.Find("ImpulseBgBtn").TryGetComponent<HoldButtonImpulse>(out var holdButtonImpulse);
            var freezeClick = GameObject.Find("FreezeBgBtn").GetComponent<Button>();
            GameObject.Find("RotateLeftBtn").TryGetComponent<HoldButton>(out var holdButtonLeft);
            GameObject.Find("RotateRightBtn").TryGetComponent<HoldButton>(out var holdButtonRight);

            holdButtonLeft.OnHold += () => OnPlayerRotation("left");
            holdButtonRight.OnHold += () => OnPlayerRotation("right");
            
            freezeClick.onClick.AddListener(OnFreezePlayerRotation);

            holdButtonImpulse.OnHoldEvent += OnIncreaseImpulseForce;
            holdButtonImpulse.OnPointerUpEvent += OnCreateImpulse;
            holdButtonImpulse.OnPointerDownEvent += OnActivateImpulseBar;
        }

        /// <summary>
        /// Rotation of Mr. Stick. If now is hardcore - user can rotate stick when freeze btn isn't clicked 
        /// </summary>
        /// <param name="direction"></param>
        private void OnPlayerRotation(string direction)
        {
            _sign = direction is "left" ? 1 : -1;
            _rotationSpeed = _rb.constraints is RigidbodyConstraints2D.None
                ? DataController.IsHardcoreMode
                    ? freeRotationSpeed
                    : 0f
                : freezeRotationSpeed;
            // Debug.Log(_rotationSpeed);
            transform.Rotate(new Vector3(0, 0, _rotationSpeed * _sign * Time.unscaledDeltaTime));
        }

        private void OnActivateImpulseBar()
        {
            if (_abilitiesUi.ImpulseFilledAmount < 1f) return;
            _impulseForceBar.SetActive(true);
        }

        /// <summary>
        /// Button^ impulse // increase
        /// </summary>
        private void OnIncreaseImpulseForce()
        {
            if (_abilitiesUi.ImpulseFilledAmount < 1f) return;

            if (!_impulseForceBar.activeSelf)
                _impulseForceBar.SetActive(true);
            _impulseForce = Mathf.Clamp(_impulseForce + increaseImpulseForceSpeed * Time.unscaledDeltaTime,
                impulseForceRange.MinValue,
                impulseForceRange.MaxValue);
            _impulseForceFilledImage.fillAmount = (_impulseForce - impulseForceRange.MinValue) /
                                                  (impulseForceRange.MaxValue - impulseForceRange.MinValue);
        }

        /// <summary>
        /// Work when finger is up
        /// </summary>
        private void OnCreateImpulse()
        {
            // if _impulseFilled.fillAmount is full ( == 1 )
            if (_abilitiesUi.ImpulseFilledAmount < 1f) return;

            // Debug.Log(_impulseFilled.fillAmount);
            _rb.constraints = RigidbodyConstraints2D.None;
            var temp = _endOfGun.transform.position - transform.position;
            // Add weak blur for direction
            temp += new Vector3(Random.Range(_values[0], _values[1]), Random.Range(_values[0], _values[1]), 0);
            // Debug.Log(_impulseForce);
            _rb.AddForce(temp * _impulseForce, ForceMode2D.Impulse);
            // Reset value to min force
            _impulseForce = impulseForceRange.MinValue;
            _impulseForceFilledImage.fillAmount = 0;
            _impulseForceBar.SetActive(false);
            _abilitiesUi.ImpulseFilledAmount = 0;
            // StartCoroutine(WaitIe());
        }

        public void OnFreezePlayerRotation()
        {
            if (_abilitiesUi.FreezeFilledAmount < 1f) return;

            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
            _abilitiesUi.FreezeFilledAmount = 0;
            StartCoroutine(UnfreezePlayer());
        }

        private IEnumerator UnfreezePlayer()
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(freezeDelay);
            _rb.constraints = RigidbodyConstraints2D.None;
            Time.timeScale = 1;
        }


#if UNITY_EDITOR

        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) return;
            if (Mathf.Abs(_abilitiesUi.FreezeFilledAmount - 1) > 0.01f) return;

            if (Input.GetKey(KeyCode.A))
            {
                // var tempAngel = transform.rotation.z - 1f;
                // _rb.MoveRotation(tempAngel);
                transform.Rotate(new Vector3(0, 0, freeRotationSpeed * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.D))
                transform.Rotate(new Vector3(0, 0, -freeRotationSpeed * Time.deltaTime));
        }

#endif
    }
}