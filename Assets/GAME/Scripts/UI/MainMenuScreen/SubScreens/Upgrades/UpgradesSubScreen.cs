using System.Collections.Generic;
using GAME.Scripts.Characters;
using GAME.Scripts.Core;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Upgrades {
    public class UpgradesSubScreen : MainMenuSubScreenBase {
        [SerializeField] private CharacterUpgradeContainer _characterUpgradeContainerPrefab;
        [SerializeField] private Transform _characterUpgradeContainersParent1;
        [SerializeField] private Transform _characterUpgradeContainersParent2;

        private List<CharacterUpgradeContainer> _characterUpgradeContainers = new();

        [Inject] private SharedData _sharedData;
        [Inject] private CharactersDataSO _charactersDataSO;
        [Inject] private LevelTimeLinesDataSO _timeLinesDataSO;
        [Inject] private ResourcesService _resourcesService;
        [Inject] private SignalBus _signalBus;
        [Inject] private SaveLoadService _saveLoadService;
        
        
        public override void Show() {
            Init();
            base.Show();
        }
       

        private void Init() {
            LevelTimeLine curTimeLine = _timeLinesDataSO.timeLines[_sharedData.SaveData.currentTimeLine.timeLineId];
            LevelStage curStage = curTimeLine.stages[_sharedData.SaveData.currentTimeLine.playerStageId];
            List<Character> curCharacters = new();
            foreach (int charId in curStage.characters) {
                curCharacters.Add(_charactersDataSO.CharactersData[charId]);
            }

            for (int i = 0; i < curCharacters.Count; i++) {
                Character curChar = curCharacters[i];
                CharacterUpgradeContainer curCharContainer = Instantiate(_characterUpgradeContainerPrefab,
                    i < 3 ? _characterUpgradeContainersParent1 : _characterUpgradeContainersParent2);
                curCharContainer.Init(curChar, _sharedData, _resourcesService, curStage, _signalBus);
                curCharContainer.BuyButton.onClick.AddListener(() => OnCharBuyClicked(curCharContainer));
                _characterUpgradeContainers.Add(curCharContainer);
            }
        }

        private void OnDisable() {
            foreach (CharacterUpgradeContainer curContainer in _characterUpgradeContainers) {
                curContainer.BuyButton.onClick.RemoveAllListeners();
                Destroy(curContainer.gameObject);
            }
            _characterUpgradeContainers.Clear();
        }

        private void OnCharBuyClicked(CharacterUpgradeContainer curContainer) {
            bool isEnoughMoney = _sharedData.SaveData.coins >= curContainer.Character.unlockCost;
            if (!isEnoughMoney)
            {
                _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBlock});
                return;
            }
            
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBuy});
            
            LevelTimeLine curTimeLine = _timeLinesDataSO.timeLines[_sharedData.SaveData.currentTimeLine.timeLineId];
            LevelStage curStage = curTimeLine.stages[_sharedData.SaveData.currentTimeLine.playerStageId];
            _sharedData.SaveData.currentTimeLine.openedCharacters.Add(curStage.characters.IndexOf(curContainer.Character.id));
            _sharedData.SaveData.coins -= curContainer.Character.unlockCost;
            foreach (CharacterUpgradeContainer container in _characterUpgradeContainers) {
                container.UpdateView();
            }
            _signalBus.Fire<UpdateCurrencySignal>();
            _signalBus.Fire<UpdateAlertImagesSignal>();
            _saveLoadService.SaveData();
        }
    }
}
