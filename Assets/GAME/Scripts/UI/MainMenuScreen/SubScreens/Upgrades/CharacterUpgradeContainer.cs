using System;
using Assets.SimpleLocalization.Scripts;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Characters;
using GAME.Scripts.Core;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Upgrades {
    public class CharacterUpgradeContainer : MonoBehaviour {
        [SerializeField] private Image _characterImage;
        [SerializeField] private Image _openedImage;
        [SerializeField] private Image _closedImage;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Sprite _buyButtonActiveSprite;
        [SerializeField] private Sprite _buyButtonInActiveSprite;
        [SerializeField] private Transform _buttonContainerTransform;
        [SerializeField] private Transform _charNameContainerTransform;
        [SerializeField] private TextMeshProUGUI _characterNameLabel;
        [SerializeField] private TextMeshProUGUI _costLabel;
        [SerializeField] private Color _buttonActiveAlpha;
        [SerializeField] private Color _buttonInactiveAlpha;
        [SerializeField] private Color _buttonActiveTextColor;
        [SerializeField] private Color _buttonInActiveTextColor;

        private Character _character;
        private SharedData _sharedData;
        private ResourcesService _resourcesService;
        private LevelStage _stageData;
        private SignalBus _signalBus;
        

        public Button BuyButton => _buyButton;
        public Character Character => _character;
        

        public void Init(Character character, SharedData sharedData, ResourcesService resourcesService, LevelStage stageData, SignalBus signalBus) {
            _character = character;
            _sharedData = sharedData;
            _resourcesService = resourcesService;
            _stageData = stageData;
            SetCharSprite().Forget();
            UpdateView();
            _signalBus = signalBus;
            _signalBus.Subscribe<LocalizationCloneObjectsSignal>(Localization);
        }


        public void UpdateView() {
            bool isOpenedChar = _sharedData.SaveData.currentTimeLine.openedCharacters.
                Contains(_stageData.characters.IndexOf(_character.id));
            _openedImage.gameObject.SetActive(isOpenedChar);
            _closedImage.gameObject.SetActive(!isOpenedChar);
            _buttonContainerTransform.gameObject.SetActive(!isOpenedChar);
            _charNameContainerTransform.gameObject.SetActive(isOpenedChar);
            bool isEnoughMoney = _sharedData.SaveData.coins >= _character.unlockCost;
           // _buyButton.interactable = !isOpenedChar && isEnoughMoney;
            _characterImage.color = _buttonActiveAlpha;
            
            _characterNameLabel.text = LocalizationManager.Localize(_character.keyNameLocalization);
            if (!isOpenedChar) {
                _buyButton.image.sprite = isEnoughMoney ? _buyButtonActiveSprite : _buyButtonInActiveSprite;
                _costLabel.text = _character.unlockCost.ToString();
            
                _costLabel.color = isEnoughMoney ? _buttonActiveTextColor : _buttonInActiveTextColor;
                _characterImage.color = isEnoughMoney ? _buttonActiveAlpha : _buttonInactiveAlpha;
                _closedImage.color = isEnoughMoney ? _buttonActiveAlpha : _buttonInactiveAlpha;
           }
        }

        private void OnDisable()
        {
            Dispose();
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<LocalizationCloneObjectsSignal>(Localization);
        }
        private void Localization()
        {
            _characterNameLabel.text = LocalizationManager.Localize(_character.keyNameLocalization);
        }
        private async UniTaskVoid SetCharSprite() {
            Sprite charSprite = await _resourcesService.LoadSprite(_character.uiSpriteReference);
            _characterImage.sprite = charSprite;
        }
    }
}
