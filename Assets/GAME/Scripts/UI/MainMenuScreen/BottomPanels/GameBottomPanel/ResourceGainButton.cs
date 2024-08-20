using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels.GameBottomPanel {
    public class ResourceGainButton : MonoBehaviour {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _countLabel;
        
        public void UpdateValues(float progressValue, int countValue) {
            _fillImage.fillAmount = progressValue;
            _countLabel.text = countValue.ToString();
        }
    }
}
