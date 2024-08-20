using DG.Tweening;
using GAME.Scripts.UI.MainMenuScreen.SubScreens;
using UnityEngine;
using UnityEngine.UI;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels.MainBottomPanel {
    public enum MainButtonType {
        Battle = 0,
        Dungeons = 1,
        Upgrades = 2,
        Cards = 3,
        Shop = 4,
        BaseUpgrades = 5,
    }
    
    public class MainButton : MonoBehaviour {
        [SerializeField] private MainButtonType _buttonType;
        [SerializeField] private Button _button;
        [SerializeField] private Image _activeImage;
        [SerializeField] private Image _inActiveImage;
        [SerializeField] private RectTransform _moveContainer;
        [SerializeField] private float _yMoveDelta;
        [SerializeField] private bool _openSubButtonsPanel;
        [SerializeField] private MainMenuSubScreenType _subsScreenToOpen;
        [SerializeField] private Image _alertImage;

        public MainButtonType ButtonType => _buttonType;
        public Button Button => _button;
        public bool OpenSubButtonsPanel => _openSubButtonsPanel;
        public MainMenuSubScreenType SubScreenToOpen => _subsScreenToOpen;

        public void SetStatus(bool active, bool instant) {
            _activeImage.gameObject.SetActive(active);
            _inActiveImage.gameObject.SetActive(!active);

            float yPos = active ? _yMoveDelta : -_yMoveDelta;
            if (instant) {
                //_moveContainer.anchoredPosition = new Vector2(_moveContainer.anchoredPosition.x, yPos);
            }
            else {
              //  _moveContainer.DOAnchorPosY(yPos, 0.2f);
            }
        }

        public void SetAlertStatus(bool active) {
            _alertImage.gameObject.SetActive(active);
        }
    }
}
