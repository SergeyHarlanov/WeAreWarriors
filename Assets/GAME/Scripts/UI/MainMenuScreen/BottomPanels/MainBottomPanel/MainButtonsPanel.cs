using System;
using System.Collections.Generic;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels.MainBottomPanel {
    public class MainButtonsPanel : MonoBehaviour {
        [SerializeField] private MainButton[] _buttons;

        [Inject] private SharedData _sharedData;
        [Inject] private SignalBus _signalBus;
        
        
        private MainButton _activeButton;
        private Dictionary<MainButtonType, MainButton> _buttonsMap;

        private Dictionary<MainButtonType, MainButton> ButtonsMap {
            get {
                if (_buttonsMap == null) {
                    _buttonsMap = new();

                    foreach (MainButton curButton in _buttons) {
                        _buttonsMap.Add(curButton.ButtonType, curButton);
                    }
                }

                return _buttonsMap;
            }
        }


        public event Action<MainButton> OnMainButtonClicked; 
        
        
        public void Show() {
            Init();
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void Init() {
            foreach (MainButton curButton in _buttons) {
                curButton.SetStatus(false, true);
            }
            InitButtonsAlertStatus();

            _activeButton = ButtonsMap[MainButtonType.Battle];
            _activeButton.SetStatus(true, true);
        }

        private void OnEnable() {
            foreach (MainButton curButton in _buttons) {
                curButton.Button.onClick.AddListener(() => OnButtonClicked(curButton));
            }
            _signalBus.Subscribe<UpdateAlertImagesSignal>(InitButtonsAlertStatus);
         
        }

        private void OnDisable() {
            foreach (MainButton curButton in _buttons) {
                curButton.Button.onClick.RemoveAllListeners();
            }
            _signalBus.Unsubscribe<UpdateAlertImagesSignal>(InitButtonsAlertStatus);

            _activeButton = null;
        }

        private void OnButtonClicked(MainButton curButton) {
            if (curButton == _activeButton) return;
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickToBottomMenu});
            _activeButton.SetStatus(false, false);
            _activeButton = curButton;
            _activeButton.SetStatus(true, false);
            OnMainButtonClicked?.Invoke(curButton);
        }

        private void InitButtonsAlertStatus() {
            foreach (MainButton curButton in _buttons) {
                switch (curButton.ButtonType) {
                    case MainButtonType.Shop:
                        curButton.SetAlertStatus(false);
                        break;
                    case MainButtonType.Upgrades:
                        curButton.SetAlertStatus(_sharedData.CanBuyCharacter || _sharedData.CanEvaluate);
                        break;
                    case MainButtonType.Battle:
                        curButton.SetAlertStatus(false);
                        break;
                    case MainButtonType.BaseUpgrades:
                        curButton.SetAlertStatus(_sharedData.CanUpgradeFoodGeneration || 
                                                 _sharedData.CanUpgradeBaseHealth || _sharedData.CanUpgradeSkill);
                        break;
                    case MainButtonType.Dungeons:
                        curButton.SetAlertStatus(false);
                        break;
                }
            }
        }
    }
}
