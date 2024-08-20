using UnityEngine;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens {
    public enum MainMenuSubScreenType {
        Battle = 0, 
        Dungeons = 1,
        Upgrades = 2,
        Evolution = 3,
        Cards = 4,
        Shop = 5,
        Skills = 6,
        BaseUpgrade = 7,
    }
    
    public class MainMenuSubScreenBase : MonoBehaviour {
        [SerializeField] private MainMenuSubScreenType _subScreenType;

        public MainMenuSubScreenType SubScreenType => _subScreenType;
        
        public virtual void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}
