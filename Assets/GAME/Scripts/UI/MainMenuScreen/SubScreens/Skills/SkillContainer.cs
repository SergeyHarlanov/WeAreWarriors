using System;
using System.Resources;
using System.Threading;
using Assets.SimpleLocalization.Scripts;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Skills {
    public class SkillContainer : MonoBehaviour {
        [SerializeField] private Button _choseButton;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Image _skillIconImage;
        [SerializeField] private TextMeshProUGUI _skillNameLabel;
        [SerializeField] private TextMeshProUGUI _skillLevelLabel;
        [SerializeField] private GameObject _outlineObject;
        
        

        public Button ChoseButton => _choseButton;
        public Button UpgradeButton => _upgradeButton;
        public int SkillId { get; private set; }
        public int SkillLevel { get; private set; }


        private SignalBus _signalBus;

        private string _skillname;
        public void Init(ResourcesService resourcesService, int id, int level, AssetReference spriteLink, string skillName, SignalBus signalBus) {
            SkillId = id;
            SkillLevel = level;
            _skillname = skillName;
           
            _signalBus = signalBus;
            OnLocalization();
            SetSprite(resourcesService, spriteLink).Forget();
         
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<LocalizationCloneObjectsSignal>(OnLocalization);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<LocalizationCloneObjectsSignal>(OnLocalization);
        }

        public void SetActiveStatus(bool active) {
            _outlineObject.SetActive(active);
        }

        private void OnLocalization()
        {
            _skillNameLabel.text = LocalizationManager.Localize(_skillname);
            _skillLevelLabel.text = LocalizationManager.Localize("skill_level")+ $" {(SkillLevel + 1).ToString()}";
        }

        private async UniTaskVoid SetSprite(ResourcesService resourcesService, AssetReference spriteLink) {
            Sprite skillSprite = await resourcesService.LoadSprite(spriteLink);
            _skillIconImage.sprite = skillSprite;
        }
    }
}
