using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game_Stuff
{
    public class LocalizationManager : MonoBehaviour
    {
        [SerializeField] private Localization localization;
        [SerializeField] private ToggleSwitch languageSwitch;
        [SerializeField] private ToggleSwitch modeSwitch;

        private Text[] _texts;
        private TextMeshProUGUI[] _uTexts;

        private void Awake() =>
            localization.Init();

        private void Start()
        {
            Localization.SetLanguage(DataController.Language);
            if (languageSwitch != null)
                languageSwitch.Toggle(DataController.Language is "Russian", false);
            SearchAllLocalizations();
            ApplyLocalisations();
        }


        public void OnChangeLanguage()
        {
            var lang = DataController.Language is "English" ? "Russian" : "English";

            Localization.SetLanguage(lang);
            DataController.Language = lang;
            ApplyLocalisations();
            // Debug.Log(lang);
        }

        private void SearchAllLocalizations()
        {
            _texts = FindObjectsOfType<Text>();
            _uTexts = FindObjectsOfType<TextMeshProUGUI>();
        }

        /// <summary>
        /// Lokalize project (only two languages)
        /// </summary>
        private void ApplyLocalisations()
        {
            var objsWithoutDetail = "";
            foreach (var textObj in _texts)
            {
                var hasDetail = textObj.TryGetComponent<TextLocalizationDetail>(out var textLocalizationDetail);
                if(hasDetail)
                    textObj.text = Localization.For(textLocalizationDetail.Key);
                else objsWithoutDetail += $" || {textObj.name}";
            }
            foreach (var textObj in _uTexts)
            {
                var hasDetail = textObj.TryGetComponent<TextLocalizationDetail>(out var textLocalizationDetail);
                if(hasDetail)
                    textObj.text = Localization.For(textLocalizationDetail.Key);
                else objsWithoutDetail += $" || {textObj.name}";
            }
        }
    }
}