using DG.Tweening;
using GAME.Scripts.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels.GameBottomPanel {
    public class SpawnCharacterButton : MonoBehaviour {
        [SerializeField] private Image _activeImage;
        [SerializeField] private Image _inactiveImage;
        [SerializeField] private Image _charImage;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _costLabel;
        [SerializeField] private Transform _mainContainerTransform;

        private bool _isInitialized;
        private bool _isActive;

        public Button SpawnButton => _button;
        public Character CharacterData { get; private set; }
        
        
        public void Init(Character curCharData, Sprite charSprite) {
            CharacterData = curCharData;
            _charImage.sprite = charSprite;
            _isActive = false;
            SetActiveStatus(_isActive);
            _costLabel.text = curCharData.spawnCost.ToString();
            gameObject.SetActive(true);
            _isInitialized = true;
        }

        public void Dispose() {
            _isInitialized = false;
            CharacterData = null;
        }
        
        public void SetActiveStatus(bool isActive) {
            if (_isActive == isActive) return;

            _isActive = isActive;
         //   _button.interactable = _isActive;
            _activeImage.gameObject.SetActive(_isActive);
            _inactiveImage.gameObject.SetActive(!_isActive);

            if (_isActive) {
                _mainContainerTransform.localScale = Vector3.one;
                _mainContainerTransform.DOShakeScale(0.2f, 0.2f);
            }
        }
    }
}
