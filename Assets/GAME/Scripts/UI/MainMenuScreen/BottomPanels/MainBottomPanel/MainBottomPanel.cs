using System;
using GAME.Scripts.UI.MainMenuScreen.SubScreens;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels.MainBottomPanel {
    public class MainBottomPanel : BottomPanelBase {
        [SerializeField] private MainButtonsPanel _mainButtonsPanel;
        [SerializeField] private SubButtonsPanel _subButtonsPanel;

        public event Action<MainMenuSubScreenType> OnScreenChangeButtonClicked;

        private SignalBus _signalBus;
        public override void Show(SignalBus signalBus) {
            Init(signalBus);
            base.Show(signalBus);
        }

        private void Init(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _mainButtonsPanel.Show();
            _subButtonsPanel.Hide();
        }

        private void OnEnable() {
            _mainButtonsPanel.OnMainButtonClicked += OnMainButtonClicked;
            _subButtonsPanel.OnChangeSubScreenClicked += OnChangeSubScreenClicked;
        }

        private void OnDisable() {
            _mainButtonsPanel.OnMainButtonClicked -= OnMainButtonClicked;
            _subButtonsPanel.OnChangeSubScreenClicked -= OnChangeSubScreenClicked;
        }

        private void OnMainButtonClicked(MainButton curButton) {
            if (curButton.OpenSubButtonsPanel) {
                _subButtonsPanel.Show(curButton.SubScreenToOpen, _signalBus);
            }
            else {
                _subButtonsPanel.Hide();
            }
            
            OnScreenChangeButtonClicked?.Invoke(curButton.SubScreenToOpen);
        }

        private void OnChangeSubScreenClicked(MainMenuSubScreenType type) {
            OnScreenChangeButtonClicked?.Invoke(type);
        }
    }
}
