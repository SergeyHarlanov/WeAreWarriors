using System;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using GAME.Scripts.UI.SettingsScreen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.UI.Cheats {
    public class CheatsScreen : ScreenBase {
        [SerializeField] private Button _resetButton, _setMeatButton, _winButton, _evolitionButton;
        [SerializeField] private SettingsToggle _gainMeatToggle;
        [SerializeField] private WebGLNativeInputField _inputField;
        
        [Inject] private SignalBus _signalBus;
        [Inject] private SharedData _sharedData;
        
        private void OnEnable() {
            _resetButton.onClick.AddListener(OnResetButtonClicked);
            _gainMeatToggle.ToggleButton.onClick.AddListener(OnCheatMeatToggleClicked);
            _setMeatButton.onClick.AddListener(OnSetMeat);
            _winButton.onClick.AddListener(OnWin);
            _evolitionButton.onClick.AddListener(OnEvolution);
        }

        private void OnDisable() {
            _resetButton.onClick.RemoveListener(OnResetButtonClicked);
            _gainMeatToggle.ToggleButton.onClick.RemoveListener(OnCheatMeatToggleClicked);
            _setMeatButton.onClick.RemoveListener(OnSetMeat);
            _winButton.onClick.RemoveListener(OnWin);
            _evolitionButton.onClick.RemoveListener(OnEvolution);
        }

        public override ScreenBase Show(bool instant = false, float timeDoFade = 0.3f) {
            UpdateToggles();
            return base.Show(instant);
        }

        private void Update()
        {
            float scale = 0;
            float.TryParse(_inputField.text, out scale);
            Time.timeScale = scale;
        }

        private void OnResetButtonClicked() {
            _signalBus.Fire<ResetProgressSignal>();
        }

        private void OnSetMeat()
        {
          _signalBus.Fire<MeatRewardAddSignal>( new MeatRewardAddSignal(){MeatAmount = 100});
        }
        private void UpdateToggles() {
            _gainMeatToggle.SetToggle(_sharedData.CheatMeatProduction, true);
        }

        private void OnCheatMeatToggleClicked() {
            _sharedData.CheatMeatProduction = !_sharedData.CheatMeatProduction;
            _gainMeatToggle.SetToggle(_sharedData.CheatMeatProduction, false);
        }

        private void OnWin()
        {
            _signalBus.Fire(new BaseDestroySignal { IsPlayerBase = false });
        }

        private void OnEvolution()
        {
            _signalBus.Fire(new EvoulitionSheatPanelSignal() {});
        }
    }
}
