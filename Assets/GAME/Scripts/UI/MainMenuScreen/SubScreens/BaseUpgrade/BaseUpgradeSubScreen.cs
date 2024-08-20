using System;
using System.Globalization;
using GAME.Scripts.Core;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.BaseUpgrade {
    public class BaseUpgradeSubScreen : MainMenuSubScreenBase {
        [SerializeField] private BaseUpgradeContainer _foodUpgradeContainer;
        [SerializeField] private BaseUpgradeContainer _healthUpgradeContainer;

        [Inject] private LevelTimeLinesDataSO _timeLinesDataSO;
        [Inject] private SharedData _sharedData;
        [Inject] private SignalBus _signalBus;
        [Inject] private SaveLoadService _saveLoadService;

        private LevelTimeLineSaveData _curTimeLineSaveData;
        private LevelStage _curStage;
        
        
        public override void Show() {
            Init();
            base.Show();
        }

        private void Init() {
            _curTimeLineSaveData = _sharedData.SaveData.currentTimeLine;
            _curStage = _timeLinesDataSO.timeLines[_curTimeLineSaveData.timeLineId]
                .stages[_curTimeLineSaveData.enemyStageId];

            InitContainers();
        }

        private void OnEnable() {
            _foodUpgradeContainer.UpgradeButton.onClick.AddListener(OnFoodUpgradeClicked);
            _healthUpgradeContainer.UpgradeButton.onClick.AddListener(OnHealthUpgradeClicked);
        }

        private void OnDisable() {
            _foodUpgradeContainer.UpgradeButton.onClick.RemoveListener(OnFoodUpgradeClicked);
            _healthUpgradeContainer.UpgradeButton.onClick.RemoveListener(OnHealthUpgradeClicked);
        }

        private void InitContainers() {
            InitFoodUpgradeContainer();
            InitHealthUpgradeContainer();
        }

        private void InitFoodUpgradeContainer() {
            float gainResourceSpeed = _curStage.gainResourcesData.gainResourceBaseSpeed + 
                                      _curStage.gainResourcesData.gainResourcePerUpgradeIncrease * 
                                      _curTimeLineSaveData.gainResourceUpgradeLevel;
            string gainResourceSpeedString = $"{gainResourceSpeed}/sec";
            int cost = _sharedData.FoodGenerationUpgradeCost;
            bool isEnoughMoney = cost <= _sharedData.SaveData.coins;
            _foodUpgradeContainer.Init(gainResourceSpeedString, cost.ToString(), isEnoughMoney);
        }

        private void InitHealthUpgradeContainer() {
            float health = _curStage.stageBase.basePlayerHealth +
                           _curStage.stageBase.playerHealthPerLevel * 
                           _curTimeLineSaveData.stageBaseLevel;
            int cost = _sharedData.BaseHealthUpgradeCost;
            bool isEnoughMoney = cost <= _sharedData.SaveData.coins;
            _healthUpgradeContainer.Init(health.ToString(CultureInfo.InvariantCulture), 
                cost.ToString(), isEnoughMoney);
        }

        private void OnFoodUpgradeClicked() {
            int nextLevel = _curTimeLineSaveData.gainResourceUpgradeLevel + 1;
            int cost = (int)_curStage.gainResourcesData.gainResourceBaseUpgradeCost + 
                       (int)MathF.Exp(1.94f * nextLevel + 1.64f);
            if (cost > _sharedData.SaveData.coins)
            {
                _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBlock});
                return;
            }
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBuy});
            
            _sharedData.SaveData.coins -= cost;
            _sharedData.SaveData.currentTimeLine.gainResourceUpgradeLevel++;
            _saveLoadService.SaveData();
            _signalBus.Fire<UpdateCurrencySignal>();
            _signalBus.Fire<UpdateAlertImagesSignal>();
            InitContainers();
        }

        private void OnHealthUpgradeClicked() {
            int nextLevel = _curTimeLineSaveData.stageBaseLevel + 1;
            int cost = (int)_curStage.stageBase.basePlayerHealth + (int)MathF.Exp(1.94f * nextLevel + 1.64f);
            if (cost > _sharedData.SaveData.coins)
            {
                _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBlock});
                return;
            }
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBuy});
            
            _sharedData.SaveData.coins -= cost;
            _sharedData.SaveData.currentTimeLine.stageBaseLevel++;
            _saveLoadService.SaveData();
            _signalBus.Fire<UpdateCurrencySignal>();
            _signalBus.Fire<UpdateAlertImagesSignal>();
            _signalBus.Fire<BaseHealthUpgradeSignal>();
            InitContainers();
        }
    }
}
