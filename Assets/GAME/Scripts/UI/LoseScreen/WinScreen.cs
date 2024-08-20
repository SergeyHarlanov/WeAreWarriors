using Assets.SimpleLocalization.Scripts;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using GAME.Scripts.UI.SettingsScreen;
using GAME.Scripts.VFX;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

namespace GAME.Scripts.UI.LoseScreen
{
    public class WinScreen : ScreenBase
    {
        [SerializeField] private Button _closeButton, _collectADSButton;
        [SerializeField] private TextMeshProUGUI _amountCollectText;
        
        [Inject] private SignalBus _signalBus;
        [Inject] private SharedData _sharedData;


        private int _coinsCollectedAmount;
        private void OnEnable() {
            _closeButton.onClick.AddListener(OnCloseClicked);
            _collectADSButton.onClick.AddListener(OnCollectADS);
        }

        private void OnDisable() {
            _closeButton.onClick.RemoveListener(OnCloseClicked);
            _collectADSButton.onClick.RemoveListener(OnCollectADS);
        }

        public override ScreenBase Show(bool instant = false, float timeDoFade = 0.3f) {
            Init();
            
            return base.Show(instant, timeDoFade);
        }

        private void Init() {
            _coinsCollectedAmount = _sharedData.LevelController.AmountCoinsCollectedBattle;
            _amountCollectText.text = _sharedData.LevelController.AmountCoinsCollectedBattle.ToString();
        }
        private void OnCloseClicked() {
            
       
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickToClose});
         _signalBus.Fire(new QuitBattleSignal() {ScreenTypeForHide = new [] { ScreenType.MainMenuScreen , ScreenType.WinScreen}, 
             MinedCoinForBattle = _coinsCollectedAmount});
        }

        private void OnCollectADS()
        {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
        }
    
    }
}