using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Characters;
using GAME.Scripts.Core;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels.GameBottomPanel {
    public class GameBottomPanel : BottomPanelBase {
        [SerializeField] private ResourceGainButton _resourceGainButton;
        [SerializeField] private Transform _spawnCharactersButtonContainerTransform;
        [SerializeField] private SpawnCharacterButton _spawnCharacterButtonPrefab;


        [Inject] private SharedData _sharedData;
        [Inject] private CharactersDataSO _charactersDataSO;
        [Inject] private LevelTimeLinesDataSO _timeLinesDataSO;
        [Inject] private ResourcesService _resourcesService;
        [Inject] private SignalBus _signalBus;

        private List<Character> _charactersData;
        private List<SpawnCharacterButton> _spawnCharacterButtons;
        private int _mainResourceCount;
        
        
        private void OnEnable() {
            _sharedData.LevelController.OnResourceFillChanged += OnResourceFillChanged;
        }

        private void OnDisable() {
            _sharedData.LevelController.OnResourceFillChanged -= OnResourceFillChanged;
            Dispose();
        }

        public override void Show(SignalBus signalBus) {
            Init();
            base.Show(signalBus);
        }

        private void Init() {
            _mainResourceCount = 0;
            CreateCharactersButtons();
            UpdateResourceGainButton(0f, 0);
        }

        private void Dispose() {
            _mainResourceCount = 0;
            _charactersData.Clear();
            
            foreach (SpawnCharacterButton curButton in _spawnCharacterButtons) {
                curButton.SpawnButton.onClick.RemoveAllListeners();
                curButton.Dispose();
                Destroy(curButton.gameObject);
            }
            _spawnCharacterButtons.Clear();
        }

        private void OnResourceFillChanged(float resourceGainValue, int mainResourceCount) {
           
            _mainResourceCount = mainResourceCount;
            UpdateSpawnCharacterButtonsStatus();
            UpdateResourceGainButton(resourceGainValue, mainResourceCount);
        }

        private void UpdateResourceGainButton(float resourceGainValue, int mainResourceCount) {
            _resourceGainButton.UpdateValues(resourceGainValue, mainResourceCount);
        }

        private void CreateCharactersButtons() {
            _charactersData = new();
            _spawnCharacterButtons = new();
            
            foreach (int openedCharTimeLineId in _sharedData.SaveData.currentTimeLine.openedCharacters) {
                int charId = _timeLinesDataSO.timeLines[_sharedData.SaveData.currentTimeLine.timeLineId]
                    .stages[_sharedData.SaveData.currentTimeLine.playerStageId].characters[openedCharTimeLineId];
                Character curChar = _charactersDataSO.CharactersData[charId];
                _charactersData.Add(curChar);
            }

            foreach (Character curCharData in _charactersData) {
                SpawnCharacterButton(curCharData).Forget();
            }
        }

        private async UniTaskVoid SpawnCharacterButton(Character curChar) {
            SpawnCharacterButton curButton =
                Instantiate(_spawnCharacterButtonPrefab, _spawnCharactersButtonContainerTransform);
            curButton.gameObject.SetActive(false);
            Sprite buttonSprite = await _resourcesService.LoadSprite(curChar.uiSpriteReference);
            curButton.Init(curChar, buttonSprite);
            _spawnCharacterButtons.Add(curButton);
            curButton.SpawnButton.onClick.AddListener(() => OnSpawnCharacterButtonClicked(curButton));
        }

        private void OnSpawnCharacterButtonClicked(SpawnCharacterButton curButton) {
            _signalBus.Fire(new SpawnCharacterSignal() { CharacterData = curButton.CharacterData});
        }

        private void UpdateSpawnCharacterButtonsStatus() {
            foreach (SpawnCharacterButton curButton in _spawnCharacterButtons) {
                curButton.SetActiveStatus(_mainResourceCount >= curButton.CharacterData.spawnCost);
            }
        }
    }
}
