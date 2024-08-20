using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Battle {
    public class RewardedMeatButton : MonoBehaviour {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _countLabel;

        public Button Button => _button;
    }
}
