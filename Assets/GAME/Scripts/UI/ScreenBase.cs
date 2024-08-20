using System;
using DG.Tweening;
using UnityEngine;

namespace GAME.Scripts.UI {
    public enum ScreenType {
        None = 0,
        MainMenuScreen = 1,
        GameScreen = 2,
        LoadingScreen = 3,
        Settings = 4,
        Cheats = 5,
        WinScreen = 6,
    }

    public class ScreenBase : MonoBehaviour {
        [SerializeField] protected ScreenType _type;
        [SerializeField] protected CanvasGroup _canvasGroup;


        public ScreenType Type => _type;
        

        public virtual ScreenBase Show(bool instant = false, float timeDoFade = 0.3f) {
            if (instant) {
                _canvasGroup.alpha = 1;
                gameObject.SetActive(true);
            }
            else {
                _canvasGroup.alpha = 0;
                gameObject.SetActive(true);
                _canvasGroup.DOFade(1, 0.5f);
            }
            return this;
        }

        public virtual void Hide(bool instant, Action onComplete, float timeDoFade = 0.3f) {
            if (instant) {
                _canvasGroup.alpha = 0;
                gameObject.SetActive(false);
                onComplete?.Invoke();
            }
            else {
                _canvasGroup.DOFade(0, 0.5f).OnComplete(() => {
                   gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
            }
        }
    }
}
