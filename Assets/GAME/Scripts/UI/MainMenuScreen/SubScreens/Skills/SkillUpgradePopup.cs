using System;
using Assets.SimpleLocalization.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using GAME.Scripts.Skills;
using GAME.Scripts.Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Skills {
    public class SkillUpgradePopup : MonoBehaviour {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Image _skillImage;
        [SerializeField] private TextMeshProUGUI _levelLabel;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Sprite _upgradeActiveSprite;
        [SerializeField] private Sprite _buttonInActiveSprite;
        [SerializeField] private Sprite _equipActiveSprite;
        [SerializeField] private Color _upgradeActiveColor;
        [SerializeField] private Color _upgradeInActiveColor;
        [SerializeField] private Button _equipButton;
        [SerializeField] private TextMeshProUGUI _profitLabel;
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] private TextMeshProUGUI _costLabel;
        [SerializeField] private TextMeshProUGUI _equipButtonLabel;


        [Inject] private ResourcesService _resourcesService;
        [Inject] private SkillsDataSO _skillsDataSO;
        [Inject] private SaveLoadService _saveLoadService;
        [Inject] private SharedData _sharedData;
        [Inject] private SignalBus _signalBus;

        private int _skillId = -1;
        private Action _onHide;


        private int _skillLevel;
        private bool _isSkillEquipped;
        private Skill _curSkillInfo;
        public void Show(int skillId, Action onHide) {
            Init(skillId, onHide);
            _canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1, 0.3f);
        }

        private void Init(int skillId, Action onHide) {
            _closeButton.interactable = true;
            _skillId = skillId;
            _onHide = onHide;
            Skill curSkillInfo = _skillsDataSO.skills[skillId];
            _curSkillInfo = curSkillInfo;
            SetSkillImage(curSkillInfo.uiSpriteLink).Forget();

            _skillLevel = _sharedData.SaveData.skillsUpgradesData[skillId].level;
        
            int upgradeCost = curSkillInfo.upgradeCost;
            bool canUpgrade = _sharedData.SaveData.crystals >= upgradeCost;
          //  _upgradeButton.interactable = canUpgrade;
            _upgradeButton.image.sprite = canUpgrade ? _upgradeActiveSprite : _buttonInActiveSprite;
            _costLabel.text = upgradeCost.ToString();
            _costLabel.color = canUpgrade ? _upgradeActiveColor : _upgradeInActiveColor;
            bool isSkillEquipped = _sharedData.SaveData.equippedSkillId == skillId;
            _isSkillEquipped = isSkillEquipped;
            _equipButton.image.sprite = isSkillEquipped ? _buttonInActiveSprite : _equipActiveSprite;
            
        
            _equipButton.interactable = !isSkillEquipped;
            _profitLabel.text = curSkillInfo.upgradeDescription;
            OnLocalization();
            
  
        }

        private void OnEnable() {
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            _upgradeButton.onClick.AddListener(OnUpgradeClicked);
            _equipButton.onClick.AddListener(OnEquipClicked);
            _signalBus.Subscribe<LocalizationCloneObjectsSignal>(OnLocalization);
        }

        private void OnDisable() {
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            _upgradeButton.onClick.RemoveListener(OnUpgradeClicked);
            _equipButton.onClick.RemoveListener(OnEquipClicked);
            _skillId = -1;
            _onHide = null;
            _signalBus.Unsubscribe<LocalizationCloneObjectsSignal>(OnLocalization);
        }

        private void OnLocalization()
        {
            _nameLabel.text = LocalizationManager.Localize(_curSkillInfo.localizationKey);

            _levelLabel.text = LocalizationManager.Localize("skill_level")+
                               $" {_sharedData.SaveData.skillsUpgradesData[_skillId].level + 1}";
            _equipButtonLabel.text = _isSkillEquipped ? LocalizationManager.Localize("skill_Equipped") :
                LocalizationManager.Localize("skill_equip");
        }
        private void Hide() {
            _onHide?.Invoke();
            _closeButton.interactable = false;
            _canvasGroup.DOFade(0, 0.3f).OnComplete(() => gameObject.SetActive(false));
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickToClose});
        }

        private void OnCloseButtonClicked() {
            Hide();
        }

        private async UniTaskVoid SetSkillImage(AssetReference reference) {
            Sprite curSprite = await _resourcesService.LoadSprite(reference);
            _skillImage.sprite = curSprite;
        }

        private void OnUpgradeClicked() {
            Skill skillInfo = _skillsDataSO.skills[_skillId];
            int upgradeCost = skillInfo.upgradeCost;
            bool canUpgrade = upgradeCost <= _sharedData.SaveData.crystals;
            if (!canUpgrade)
            {
                _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBlock});
                return;
            }
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBuy});
            _sharedData.SaveData.crystals -= upgradeCost;
            _sharedData.SaveData.skillsUpgradesData[_skillId].level++;
            _saveLoadService.SaveData();
            _signalBus.Fire<UpdateCurrencySignal>();
            _signalBus.Fire<UpdateAlertImagesSignal>();
            OnLocalization();
                //  _levelLabel.text = $"Level { _sharedData.SaveData.skillsUpgradesData[_skillId].level + 1 }";
        }

        private void OnEquipClicked() {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.SfxSet});
            _sharedData.SaveData.equippedSkillId = _skillId;
            Hide();
        }
    }
}
