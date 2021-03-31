using System;
using System.Collections;
using Core.Data;
using DG.Tweening;
using Game_Stuff;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI
{
    public class AbilitiesUi : MonoBehaviour
    {
        [SerializeField] private Image impulseFilled;
        [SerializeField] private Image freezeFilled;

        [Header("Recovery Image filled speed")] [SerializeField] [Range(.5f, 30f)]
        private float impulseFillSpeed = 5f;

        [SerializeField] [Range(.5f, 30f)] private float freezeFillSpeed = 5f;
        
        public float ImpulseFilledAmount
        {
            get => _impulseFilled;
            set => impulseFilled.fillAmount = _impulseFilled = value;
        }

        public float FreezeFilledAmount
        {
            get => _freezeFilled;
            set => freezeFilled.fillAmount = _freezeFilled = value;
        }

        private void Awake() => ImpulseFilledAmount = FreezeFilledAmount = 1f;

        private float _impulseFilled;
        private float _freezeFilled;


        private void Start()
        {
            impulseFilled.fillAmount = freezeFilled.fillAmount = 1;
        }

        private void Update()
        {
            if (impulseFilled.fillAmount < 1)
                ImpulseFilledAmount += .1f * impulseFillSpeed*Time.unscaledDeltaTime;
            if (freezeFilled.fillAmount < 1)
                FreezeFilledAmount += .1f * freezeFillSpeed*Time.unscaledDeltaTime;
        }
    }
}