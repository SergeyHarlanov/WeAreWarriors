using System;
using System.Collections.Generic;
using System.Linq;
using GAME.Scripts.Signals;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.UI {
    public class UIService : MonoBehaviour {
        [SerializeField] private List<ScreenBase> _screens;
 
        private Dictionary<ScreenType, ScreenBase> _screensDict;
        private Dictionary<ScreenType, ScreenBase> _activeScreens = new();

        [Inject] private SignalBus _signalBus;
        
        
        private Dictionary<ScreenType, ScreenBase> ScreensDict {
            get {
                if (_screensDict == null) {
                    _screensDict = new();
                    foreach (ScreenBase screen in _screens) {
                        _screensDict.Add(screen.Type, screen);
                    }
                }

                return _screensDict;
            }
        }

        public void Init() {
            _signalBus.Subscribe<ShowScreenSignal>(OnShowScreenSignalReceived);
            _signalBus.Subscribe<HideScreenSignal>(OnHideScreenSignalReceived);
        }

        public void Dispose() {
            _signalBus.Unsubscribe<ShowScreenSignal>(OnShowScreenSignalReceived);
            _signalBus.Unsubscribe<HideScreenSignal>(OnHideScreenSignalReceived);
        }

        public ScreenBase ShowScreen(ScreenType type, bool instant = false) {
            ScreenBase curScreen = ScreensDict[type];
            if (curScreen.gameObject.activeSelf) return curScreen;
            if (type == ScreenType.LoadingScreen)
            {
                curScreen.Show(instant, 1f);
            }
            else
            {
                curScreen.Show(instant);
            }
            _activeScreens.Add(type, curScreen);
            return curScreen;
        }

        public void HideScreen(ScreenType type, bool instant = false, Action onComplete = null) {
            if (_activeScreens.TryGetValue(type, out ScreenBase curScreen))
            {
                if (type == ScreenType.MainMenuScreen)
                {
                    curScreen.Hide(instant, onComplete, 1f);
                }
                else
                {
                    curScreen.Hide(instant, onComplete, 1f);
                }
       
                _activeScreens.Remove(curScreen.Type);
            }
        }

        public void CloseAllActiveScreens(bool instant) {
            List<ScreenBase> screensToClose = _activeScreens.Values.ToList(); 
            foreach (ScreenBase curScreen in screensToClose) {
                HideScreen(curScreen.Type, instant);
            }
        }

        public T GetActiveScreen<T>(ScreenType type) where T: class{
            if (_activeScreens.TryGetValue(type, out ScreenBase screen)) {
                return screen as T;
            }

            return null;
        }

        public void SwitchCheatsScreen() {
            if (_activeScreens.ContainsKey(ScreenType.Cheats)) {
                HideScreen(ScreenType.Cheats, true);
            }
            else {
                ShowScreen(ScreenType.Cheats, true);
            }
        }

        private void OnShowScreenSignalReceived(ShowScreenSignal args) {
            ShowScreen(args.ScreenToShow);
            Debug.Log($"Show Screen {args.ScreenToShow.ToString()}");
        }

        private void OnHideScreenSignalReceived(HideScreenSignal args) {
            HideScreen(args.ScreenToHide);
        }
    }
}
