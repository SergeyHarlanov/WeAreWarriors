using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using GAME.Scripts.UI.MainMenuScreen.BottomPanels.GameBottomPanel;
using GAME.Scripts.UI.MainMenuScreen.BottomPanels.MainBottomPanel;
using GAME.Scripts.UI.MainMenuScreen.SubScreens;
using GAME.Scripts.UI.MainMenuScreen.Top;
using UnityEngine;
using YG;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen {
    public class MainMenuScreen : ScreenBase {
        [SerializeField] private MainMenuSubScreenBase[] _subScreens;
        [SerializeField] private MainBottomPanel _mainBottomPanel;
        [SerializeField] private GameBottomPanel _gameBottomPanel;
        [SerializeField] private TopPanel _topPanel;
        
        [Inject] private SignalBus _signalBus;

        
        private MainMenuSubScreenBase _activeSubScreen;
        private Dictionary<MainMenuSubScreenType, MainMenuSubScreenBase> _subScreensMap;
        

        private Dictionary<MainMenuSubScreenType, MainMenuSubScreenBase> SubScreensMap {
            get {
                if (_subScreensMap == null) {
                    _subScreensMap = new();
                    foreach (MainMenuSubScreenBase curSubScreen in _subScreens) {
                        _subScreensMap.Add(curSubScreen.SubScreenType, curSubScreen);
                    }
                }

                return _subScreensMap;
            }
        }

        public override ScreenBase Show(bool instant = false, float timeDoFade = 0.3f) {
            Init();
            return base.Show(instant, 1f);
        }

        private void Init() {
            LocalizationManager.Read();
            LocalizationManager.Language = YandexGame.lang switch {
                "en" => "English",
                "ru" => "Russian",
                _ => "English",
            };
            _gameBottomPanel.Hide();
            _mainBottomPanel.Show(_signalBus);
            foreach (MainMenuSubScreenBase curSubScreen in _subScreens) {
                curSubScreen.Hide();
            }
            
            _activeSubScreen = SubScreensMap[MainMenuSubScreenType.Battle];
            _activeSubScreen.Show();
            _topPanel.UpdateCurrencyLabels();
            _topPanel.SetTitleLabel(_activeSubScreen.SubScreenType.ToString());
        }

        private void OnEnable() {
            _signalBus.Subscribe<StartGameSignal>(OnStartGameSignalReceived);
            _mainBottomPanel.OnScreenChangeButtonClicked += OnScreenChangeButtonClicked;
            _topPanel.SettingsButton.onClick.AddListener(OnSettingsClicked);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<StartGameSignal>(OnStartGameSignalReceived);
            _mainBottomPanel.OnScreenChangeButtonClicked -= OnScreenChangeButtonClicked;
            _topPanel.SettingsButton.onClick.RemoveListener(OnSettingsClicked);
        }

        private void OnStartGameSignalReceived() {
            _mainBottomPanel.Hide();
            _gameBottomPanel.Show(_signalBus);
        }

        private void OnScreenChangeButtonClicked(MainMenuSubScreenType screenType) {
            _activeSubScreen.Hide();
            _activeSubScreen = SubScreensMap[screenType];
            _activeSubScreen.Show();
                 _topPanel.SetTitleLabel(_activeSubScreen.SubScreenType.ToString());
        }

        private void OnSettingsClicked() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
            _signalBus.Fire(new ShowScreenSignal { ScreenToShow = ScreenType.Settings });
        }
    }
}
