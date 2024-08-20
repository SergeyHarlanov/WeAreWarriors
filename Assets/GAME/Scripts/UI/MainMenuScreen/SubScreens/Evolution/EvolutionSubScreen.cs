    using System;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Core;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using GAME.Scripts.Skills;
    using GAME.Scripts.Sounds;
    using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Evolution {
    public class EvolutionSubScreen : MainMenuSubScreenBase {
        [SerializeField] private TextMeshProUGUI _timeLineNameLabel;
        [SerializeField] private TextMeshProUGUI _difficultyLabel;
        [SerializeField] private StageContainer _curStageContainer;
        [SerializeField] private StageContainer _nextStageContainer;
        [SerializeField] private Transform _betweenContainerTransform;
        [SerializeField] private TextMeshProUGUI _messageLabel;
        [SerializeField] private string _messageLabelHasNextStageLocalizationKey;
        [SerializeField] private string _messageLabelNoNextStageLocalizationKey;
        [SerializeField] private Button _evaluateButton;
        [SerializeField] private TextMeshProUGUI _costLabel;
        [SerializeField] private Sprite _evaluateButtonActiveSprite;
        [SerializeField] private Sprite _evaluateButtonInActiveSprite;
        [SerializeField] private Color _costLabelActiveColor;
        [SerializeField] private Color _costLabelInActiveColor;


        [Inject] private LevelTimeLinesDataSO _timeLinesDataSO;
        [Inject] private SharedData _sharedData;
        [Inject] private ResourcesService _resourcesService;
        [Inject] private SignalBus _signalBus;
        [Inject] private SaveLoadService _saveLoadService;
        [Inject] private SkillsDataSO _skillsDataSO;

        private int _evaluateCost;
        private List<Sprite> _stageSprite = new List<Sprite>();
        private List<LevelStage> _stage = new List<LevelStage>();
        private List<StageContainer> _container = new List<StageContainer>();
        private LevelTimeLine _curTimeLine;

        private bool _hasNextStage;
        
        public override void Show() {
            Init();
            base.Show();
        }

        private void Init() {
            LevelTimeLineSaveData curTimeLineSaveData = _sharedData.SaveData.currentTimeLine;
            LevelTimeLine curTimeLine = _timeLinesDataSO.timeLines[curTimeLineSaveData.timeLineId];
            LevelStage curStage = curTimeLine.stages[curTimeLineSaveData.playerStageId];
            bool hasNextStage = curTimeLineSaveData.playerStageId < curTimeLine.stages.Count - 1;
            LevelStage nextStage = hasNextStage ? curTimeLine.stages[curTimeLineSaveData.playerStageId + 1] : null;
            _nextStageContainer.gameObject.SetActive(hasNextStage);
            _betweenContainerTransform.gameObject.SetActive(hasNextStage);
            _timeLineNameLabel.text = LocalizationManager.Localize(curTimeLine.name);
            _curTimeLine = (curTimeLine);
            InitStageContainer(curStage, _curStageContainer).Forget();
          //  string messageText = hasNextStage
          //      ? LocalizationManager.Localize(_messageLabelHasNextStageLocalizationKey)
          //      : LocalizationManager.Localize(_messageLabelNoNextStageLocalizationKey);
           // _messageLabel.text = messageText;
            _evaluateButton.gameObject.SetActive(hasNextStage);
            _hasNextStage = hasNextStage;
            if (hasNextStage) {
                InitStageContainer(nextStage, _nextStageContainer).Forget();
                _evaluateCost = nextStage.cost;
                bool isEnoughMoney = _evaluateCost <= _sharedData.SaveData.coins;
                _evaluateButton.image.sprite =
                    isEnoughMoney ? _evaluateButtonActiveSprite : _evaluateButtonInActiveSprite;
                _costLabel.text = _evaluateCost.ToString();
                _costLabel.color = isEnoughMoney ? _costLabelActiveColor : _costLabelInActiveColor;
            }
            OnLocalization();
        }

        private void OnEnable() {
            _evaluateButton.onClick.AddListener(OnEvaluateClicked);
//            _signalBus.Subscribe<EvoulitionSheatPanelSignal>(OnEvaluateCheatPanel);
            _signalBus.Subscribe<LocalizationCloneObjectsSignal>(OnLocalization);
        }

        private void OnDisable() {
            _evaluateButton.onClick.RemoveListener(OnEvaluateClicked);
         //   _signalBus.Unsubscribe<EvoulitionSheatPanelSignal>(OnEvaluateClicked);
            _signalBus.Unsubscribe<LocalizationCloneObjectsSignal>(OnLocalization);
            _evaluateCost = 0;
        }

        private void OnLocalization()
        {
            for (int i = 0; i < _container.Count; i++)
            {
                _container[i].Init(_stageSprite[i], LocalizationManager.Localize(_stage[i].localizationKey));
            
             
            
           
            }

            _difficultyLabel.text = LocalizationManager.Localize("Difficulti") +"x1.73";
            string messageText = _hasNextStage
                ? LocalizationManager.Localize(_messageLabelHasNextStageLocalizationKey)
                : LocalizationManager.Localize(_messageLabelNoNextStageLocalizationKey);
            _messageLabel.text = messageText;
            _timeLineNameLabel.text = LocalizationManager.Localize(_curTimeLine.name);
        }
        private async UniTaskVoid InitStageContainer(LevelStage stage, StageContainer container) {
            Sprite stageSprite = await _resourcesService.LoadSprite(stage.stageUiSpriteLink);
            container.Init(stageSprite, LocalizationManager.Localize(stage.localizationKey));
            _stage.Add(stage);
            _stageSprite.Add(stageSprite);
            _container.Add(container);
        }

        private void OnEvaluateClicked() {
            bool isEnoughMoney = _evaluateCost <= _sharedData.SaveData.coins;
            if (!isEnoughMoney)
            {
                _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBlock});
                return;
            }
            
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBuy});
        //    _sharedData.SaveData.coins = 0;
            _sharedData.SaveData.currentTimeLine.playerStageId++;
            _sharedData.SaveData.currentTimeLine.enemyStageId = 0;
            _sharedData.SaveData.currentTimeLine.openedCharacters.Clear();
            _sharedData.SaveData.currentTimeLine.openedCharacters.Add(0);
            _sharedData.SaveData.currentTimeLine.gainResourceUpgradeLevel = 0;
            CheckSkills();
            _saveLoadService.SaveData();
            _signalBus.Fire<UpdateCurrencySignal>();
            Init();
            _signalBus.Fire<StageEvaluateSignal>();
            _signalBus.Fire<UpdateAlertImagesSignal>();
        }
        

        private void OnEvaluateCheatPanel()
        {
            _sharedData.SaveData.coins = _evaluateCost;
            OnEvaluateClicked();
        }
        private void CheckSkills() {
            int timeLineId = _sharedData.SaveData.currentTimeLine.timeLineId;
            int stageId = _sharedData.SaveData.currentTimeLine.playerStageId;

            foreach (Skill curSkill in _skillsDataSO.skills) {
                if (curSkill.openTimeLineId == timeLineId && curSkill.openStageId == stageId) {
                    if (_sharedData.SaveData.skillsUpgradesData.Count == 0) _sharedData.SaveData.equippedSkillId = 0;
                    
                    SkillUpgradeSaveData skillData = new SkillUpgradeSaveData {
                        id = curSkill.id,
                        level = 0,
                    };
                    if (!_sharedData.SaveData.skillsUpgradesData.Exists(sk => sk.id == skillData.id)) 
                        _sharedData.SaveData.skillsUpgradesData.Add(skillData);
                }
            }
        }
    }
}
