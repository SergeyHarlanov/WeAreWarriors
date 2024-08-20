using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GAME.Scripts.UI.SettingsScreen {
    public class SettingsToggle : MonoBehaviour {
        [SerializeField] private Image _activeImage;
        [SerializeField] private Image _inActiveImage;
        [SerializeField] private Image _toggleImage;
        [SerializeField] private float _xDelta;
        [SerializeField] private Button _toggleButton;


        public Button ToggleButton => _toggleButton;

        public void SetToggle(bool isOn, bool instant) {
            _activeImage.gameObject.SetActive(isOn);
            _inActiveImage.gameObject.SetActive(!isOn);
            
            if (instant) {
                _toggleImage.rectTransform.anchoredPosition = new Vector2(isOn ? _xDelta : -_xDelta, 0);
            }
            else {
                _toggleImage.rectTransform.DOAnchorPosX(isOn ? _xDelta : -_xDelta, 0.2f);
            }
        }
    }
}
