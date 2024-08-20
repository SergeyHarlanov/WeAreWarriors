using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using GAME.Scripts.Skills;
using GAME.Scripts.Sounds;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Skills {
    public class SkillsSubScreen : MainMenuSubScreenBase {
        [SerializeField] private Image _chosenSkillImage;
        [SerializeField] private Transform _skillsContainersParentTransform;
        [SerializeField] private Transform _skillEquippedTimeLineContainer;
        [SerializeField] private Transform _skillNotEquippedTimeLineContainer;
        [SerializeField] private SkillContainer _skillContainerPrefab;
        [SerializeField] private SkillUpgradePopup _skillUpgradePopup;


        [Inject] private SharedData _sharedData;
        [Inject] private ResourcesService _resourcesService;
        [Inject] private SkillsDataSO _skillsDataSO;
        [Inject] private SaveLoadService _saveLoadService;
        [Inject] private SignalBus _signalBus;

        private List<SkillContainer> _skillContainers = new();
        private SkillContainer _activeCkillContainer;

        
        public override void Show() {
            Init();
            base.Show();
        }

        private void OnDisable() {
            foreach (SkillContainer curSkillContainer in _skillContainers) {
                curSkillContainer.ChoseButton.onClick.RemoveAllListeners();
                curSkillContainer.UpgradeButton.onClick.RemoveAllListeners();
                
                Destroy(curSkillContainer.gameObject);
            }
            
            _skillContainers.Clear();
            _activeCkillContainer = null;
        }

        private void Init() {
            bool hasOpenedSkill = _sharedData.SaveData.skillsUpgradesData.Count > 0;
            _skillEquippedTimeLineContainer.gameObject.SetActive(hasOpenedSkill);
            _skillNotEquippedTimeLineContainer.gameObject.SetActive(!hasOpenedSkill);

            if (hasOpenedSkill) {
                SetEquippedSkillSprite().Forget();

                foreach (SkillUpgradeSaveData skillUpgrade in _sharedData.SaveData.skillsUpgradesData) {
                    SkillContainer curSkillContainer =
                        Instantiate(_skillContainerPrefab, _skillsContainersParentTransform);
                    
                    curSkillContainer.Init(_resourcesService, skillUpgrade.id, skillUpgrade.level, 
                        _skillsDataSO.skills[skillUpgrade.id].uiSpriteLink, _skillsDataSO.skills[skillUpgrade.id].localizationKey, _signalBus);
                    curSkillContainer.ChoseButton.onClick.AddListener(() => OnChoseSkillClicked(curSkillContainer));
                    curSkillContainer.UpgradeButton.onClick.AddListener(() => OnUpgradeSkillClicked(curSkillContainer));
                    bool isActiveSkill = skillUpgrade.id == _sharedData.SaveData.equippedSkillId;
                    curSkillContainer.SetActiveStatus(isActiveSkill);
                    if (isActiveSkill) _activeCkillContainer = curSkillContainer;
                    _skillContainers.Add(curSkillContainer);
                }
            }
        }

        private async UniTaskVoid SetEquippedSkillSprite() {
            Sprite skillSprite =
                await _resourcesService.LoadSprite(_skillsDataSO.skills[_sharedData.SaveData.equippedSkillId]
                    .uiSpriteLink);

            _chosenSkillImage.sprite = skillSprite;
        }

        private void OnChoseSkillClicked(SkillContainer curContainer) {
            if (_sharedData.SaveData.equippedSkillId == curContainer.SkillId) return;
            
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.SfxSet});
            
            _activeCkillContainer.SetActiveStatus(false);
            _activeCkillContainer = curContainer;
            _activeCkillContainer.SetActiveStatus(true);
            
            _sharedData.SaveData.equippedSkillId = curContainer.SkillId;
            _saveLoadService.SaveData();
            SetEquippedSkillSprite().Forget();
        }

        private void OnUpgradeSkillClicked(SkillContainer curContainer) {
            if (_skillUpgradePopup.gameObject.activeSelf) return;
            
            _skillUpgradePopup.Show(curContainer.SkillId, OnUpgradePopupHide);
        }

        private void OnUpgradePopupHide() {
            foreach (SkillContainer curContainer in _skillContainers) {
                Skill skillInfo = _skillsDataSO.skills[curContainer.SkillId];
                int skillLevel = _sharedData.SaveData.skillsUpgradesData[curContainer.SkillId].level;
                bool isEquipped = curContainer.SkillId == _sharedData.SaveData.equippedSkillId;
                if (isEquipped) _activeCkillContainer = curContainer;
                curContainer.Init(_resourcesService, skillInfo.id, skillLevel, skillInfo.uiSpriteLink, skillInfo.name, _signalBus);
                curContainer.SetActiveStatus(isEquipped);
            }
        }
    }
}
