using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Battle {
    public class SpeedBoostButton : MonoBehaviour {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _rewardedCountLabel;


        public Button Button => _button;
    }
}
