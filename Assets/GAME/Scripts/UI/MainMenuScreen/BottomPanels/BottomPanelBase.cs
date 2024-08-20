using UnityEngine;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels {
    public class BottomPanelBase : MonoBehaviour {
        public virtual void Show(SignalBus signalBus) {
            gameObject.SetActive(true);
        }

        public virtual void Hide() {
            gameObject.SetActive(false);
        }
    }
}
