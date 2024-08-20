using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.BaseUpgrade {
    public class BaseUpgradeContainer : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _profitLabel;
        [SerializeField] private TextMeshProUGUI _costLabel;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Sprite _buttonActiveSprite;
        [SerializeField] private Sprite _buttonInActiveSprite;
        [SerializeField] private Color _costLabelActiveColor;
        [SerializeField] private Color _costLabelInActiveColor;


        public Button UpgradeButton => _upgradeButton;
        

        public void Init(string profitText, string costText, bool canBuy) {
            _profitLabel.text = profitText;
            _costLabel.text = costText;
            _upgradeButton.image.sprite = canBuy ? _buttonActiveSprite : _buttonInActiveSprite;
            _costLabel.color = canBuy ? _costLabelActiveColor : _costLabelInActiveColor;
        }
    }
}
