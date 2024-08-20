using System;
using Assets.SimpleLocalization.Scripts;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.Top {
    public class TopPanel : MonoBehaviour {
        [SerializeField] private Button _settingsButton;
        [SerializeField] private TextMeshProUGUI _coinsLabel;
        [SerializeField] private TextMeshProUGUI _crystalsLabel;
        [SerializeField] private TextMeshProUGUI _mainLabel;
        [SerializeField] private Button _plusCoinsButton;
        [SerializeField] private Button _plusCrystalsButton;


        [Inject] private SignalBus _signalBus;
        [Inject] private SharedData _sharedData;
        [Inject] private SaveLoadService _saveLoadService;
        
        
        public Button SettingsButton => _settingsButton;

        private string _localizationkeyHeader;
        private void OnEnable() {
            _plusCoinsButton.onClick.AddListener(OnPlusCoinsClicked);
            _plusCrystalsButton.onClick.AddListener(OnPlusCrystalsClicked);
            _signalBus.Subscribe<UpdateCurrencySignal>(UpdateCurrencyLabels);
            _signalBus.Subscribe<LocalizationCloneObjectsSignal>(OnLocalizationTitle);
        }

        private void OnDisable() {
            _plusCoinsButton.onClick.RemoveListener(OnPlusCoinsClicked);
            _plusCrystalsButton.onClick.RemoveListener(OnPlusCrystalsClicked);
            _signalBus.Unsubscribe<UpdateCurrencySignal>(UpdateCurrencyLabels);
            _signalBus.Unsubscribe<LocalizationCloneObjectsSignal>(OnLocalizationTitle);
        }

        public void UpdateCurrencyLabels() {
            _coinsLabel.text = _sharedData.SaveData.coins.ToString();
            _crystalsLabel.text = _sharedData.SaveData.crystals.ToString();
        }

       
        public void SetTitleLabel(string value)
        {
            _localizationkeyHeader = value;
            _mainLabel.text = LocalizationManager.Localize(value);
        }

        private void OnPlusCoinsClicked() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
            _sharedData.SaveData.coins += 1000000;
            _saveLoadService.SaveData();
            _signalBus.Fire<UpdateCurrencySignal>();
        }

        private void OnPlusCrystalsClicked() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
            _sharedData.SaveData.crystals += 1000;
            _saveLoadService.SaveData();
            _signalBus.Fire<UpdateCurrencySignal>();
        }
        private void OnLocalizationTitle()
        {
            SetTitleLabel(_localizationkeyHeader);
        }
    }
}
