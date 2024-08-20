using System;
using System.Collections.Generic;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using GAME.Scripts.UI.MainMenuScreen.SubScreens;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels.MainBottomPanel {
    public class SubButtonsPanel : MonoBehaviour {
        [SerializeField] private SubButtonsContainer[] subButtonsContainers;


        private Dictionary<MainMenuSubScreenType, SubButtonsContainer> _buttonsContainersMap;
        private SubButton _activeButton;

        private SignalBus _signalBus;

        private Dictionary<MainMenuSubScreenType, SubButtonsContainer> ButtonsContainersMap {
            get {
                if (_buttonsContainersMap == null) {
                    _buttonsContainersMap = new();
                    
                    foreach (SubButtonsContainer curContainer in subButtonsContainers) {
                        _buttonsContainersMap.Add(curContainer.SubScreenType, curContainer);
                    }
                }

                return _buttonsContainersMap;
            }
        }

        public event Action<MainMenuSubScreenType> OnChangeSubScreenClicked; 
        
        public void Show(MainMenuSubScreenType subScreenType, SignalBus signalBus) {
            Init(subScreenType, signalBus);
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void Init(MainMenuSubScreenType subScreenType, SignalBus signalBus) {
            _signalBus = signalBus;
            HideAll();
            ButtonsContainersMap[subScreenType].Show();
            _activeButton = ButtonsContainersMap[subScreenType].SubButtons[0];
        }

        private void OnEnable() {
            foreach (SubButtonsContainer curContainer in subButtonsContainers) {
                foreach (SubButton curButton in curContainer.SubButtons) {
                    curButton.Button.onClick.AddListener(() => OnSubButtonClicked(curButton));
                }
            }
        }

        private void OnDisable() {
            foreach (SubButtonsContainer curContainer in subButtonsContainers) {
                foreach (SubButton curButton in curContainer.SubButtons) {
                    curButton.Button.onClick.RemoveAllListeners();
                }
            }

            _activeButton = null;
        }

        private void HideAll() {
            foreach (SubButtonsContainer curContainer in subButtonsContainers) {
                curContainer.Hide();
            }
            
            _activeButton = null;
        }

        private void OnSubButtonClicked(SubButton curButton) {
            if (_activeButton == curButton)
            {
                return;
            }
            
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickToBottomMenu});
            
            _activeButton.SetStatus(false, false);
            _activeButton = curButton;
            _activeButton.SetStatus(true, false);
            
            OnChangeSubScreenClicked?.Invoke(_activeButton.ScreenType);
        }
    }
}
