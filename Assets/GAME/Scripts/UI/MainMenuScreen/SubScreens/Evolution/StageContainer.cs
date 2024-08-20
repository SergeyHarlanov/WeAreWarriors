using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Evolution {
    public class StageContainer : MonoBehaviour {
        [SerializeField] private Image _stageImage;
        [SerializeField] private TextMeshProUGUI _stageNameLabel;
        

        public void Init(Sprite stageSprite, string stageName) {
            _stageImage.sprite = stageSprite;
            _stageNameLabel.text = stageName;
        }
    }
}
