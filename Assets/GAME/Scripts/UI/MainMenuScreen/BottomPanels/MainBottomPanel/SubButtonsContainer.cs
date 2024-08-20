using System.Collections.Generic;
using System.Collections.ObjectModel;
using GAME.Scripts.UI.MainMenuScreen.SubScreens;
using UnityEngine;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels.MainBottomPanel {
    public class SubButtonsContainer : MonoBehaviour {
        [SerializeField] private MainMenuSubScreenType _subScreenType;
        [SerializeField] private List<SubButton> _subButtons;

        public MainMenuSubScreenType SubScreenType => _subScreenType;
        public ReadOnlyCollection<SubButton> SubButtons => _subButtons.AsReadOnly();


        public void Show() {
            for (int i = 0; i < _subButtons.Count; i++) {
                _subButtons[i].SetStatus(i == 0, true);
            }
            
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}
