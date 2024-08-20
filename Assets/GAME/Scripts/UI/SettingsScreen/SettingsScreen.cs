using System;
using Assets.SimpleLocalization.Scripts;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

namespace GAME.Scripts.UI.SettingsScreen {
    public class SettingsScreen : ScreenBase {
        [SerializeField] private Button _closeButton;
        [SerializeField] private SettingsToggle _soundsToggle;
        [SerializeField] private SettingsToggle _musicToggle;
        [SerializeField] private Button _languageButton;
        
        [SerializeField] private Button _resetProgressButton;

        [Inject] private SignalBus _signalBus;
        [Inject] private SharedData _sharedData;
        [Inject] private SaveLoadService _saveLoadService;

        private void OnEnable() {
            _closeButton.onClick.AddListener(OnCloseClicked);
            _musicToggle.ToggleButton.onClick.AddListener(OnMusicToggleClicked);
            _soundsToggle.ToggleButton.onClick.AddListener(OnSoundToggleClicked);
            _languageButton.onClick.AddListener(OnLanguageButtonClicked);
            _resetProgressButton.onClick.AddListener(OnResetProgressButtonClicked);
        }

        private void OnDisable() {
            _closeButton.onClick.RemoveListener(OnCloseClicked);
            _musicToggle.ToggleButton.onClick.RemoveListener(OnMusicToggleClicked);
            _soundsToggle.ToggleButton.onClick.RemoveListener(OnSoundToggleClicked);
            _languageButton.onClick.RemoveListener(OnLanguageButtonClicked);
            _resetProgressButton.onClick.RemoveListener(OnResetProgressButtonClicked);
        }

        public override ScreenBase Show(bool instant = false, float timeDoFade = 0.3f) {
            Init();
            return base.Show(instant, 0.3f);
        }

        private void Init() {
            UpdateToggles();
        }

        private void UpdateToggles() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
            _musicToggle.SetToggle(_sharedData.SaveData.music, true);
            _soundsToggle.SetToggle(_sharedData.SaveData.sounds, true);
        }

        private void OnCloseClicked() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickToClose});
            _signalBus.Fire(new HideScreenSignal { ScreenToHide = ScreenType.Settings });
        }

        private void OnMusicToggleClicked() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
            _sharedData.SaveData.music = !_sharedData.SaveData.music;
            _saveLoadService.SaveData();
            _musicToggle.SetToggle(_sharedData.SaveData.music, false);
        }
        
        private void OnSoundToggleClicked() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
            _sharedData.SaveData.sounds = !_sharedData.SaveData.sounds;
            _saveLoadService.SaveData();
            _soundsToggle.SetToggle(_sharedData.SaveData.sounds, false);
        }

        private void OnLanguageButtonClicked() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
            string newYandexLang = YandexGame.lang == "ru" ? "en" : "ru";
            YandexGame.SwitchLanguage(newYandexLang);
            LocalizationManager.Language = YandexGame.lang switch {
                "en" => "English",
                "ru" => "Russian",
                _ => "English",
            };
            _signalBus.Fire<LocalizationCloneObjectsSignal>();
        }

        private void OnResetProgressButtonClicked() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
            _signalBus.Fire<ResetProgressSignal>();
        }
    }
}
