using System;
using DG.Tweening;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using GAME.Scripts.UI.MainMenuScreen.SubScreens;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.BottomPanels.MainBottomPanel {
    public class SubButton : MonoBehaviour {
        [SerializeField] private Button _button;
        [SerializeField] private Image _activeImage;
        [SerializeField] private Image _inActiveImage;
        [SerializeField] private MainMenuSubScreenType _subScreenType;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _alertImage;


        [Inject] private SignalBus _signalBus;
        [Inject] private SharedData _sharedData;
        

        public Button Button => _button;
        public MainMenuSubScreenType ScreenType => _subScreenType;

        public void SetStatus(bool active, bool instant) {
            _activeImage.gameObject.SetActive(true);
            _inActiveImage.gameObject.SetActive(false);
            if (instant) {
                transform.localScale = Vector3.one * (active ? 1.2f : 1f);
            }
            else {
                transform.DOScale(active ? 1.2f : 1f, 0.2f);
            }

            _canvas.sortingOrder = active ? 2 : 1;
        }

        private void OnEnable() {
            InitAlertStatus();
            _signalBus.Subscribe<UpdateAlertImagesSignal>(InitAlertStatus);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<UpdateAlertImagesSignal>(InitAlertStatus);
        }

        private void InitAlertStatus() {
            switch (_subScreenType) {
                case MainMenuSubScreenType.Upgrades:
                    SetAlertStatus(_sharedData.CanBuyCharacter);
                    break;
                case MainMenuSubScreenType.Evolution:
                    SetAlertStatus(_sharedData.CanEvaluate);
                    break;
                case MainMenuSubScreenType.BaseUpgrade:
                    SetAlertStatus(_sharedData.CanUpgradeFoodGeneration || _sharedData.CanUpgradeBaseHealth);
                    break;
                case MainMenuSubScreenType.Skills:
                    SetAlertStatus(_sharedData.CanUpgradeSkill);
                    break;
                case MainMenuSubScreenType.Cards:
                    SetAlertStatus(false);
                    break;
            }
        }

        public void SetAlertStatus(bool active) {
            _alertImage.gameObject.SetActive(active);
        }
    }
}
