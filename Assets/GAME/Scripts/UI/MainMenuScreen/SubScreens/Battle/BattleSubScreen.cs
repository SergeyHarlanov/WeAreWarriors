using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Ads;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using GAME.Scripts.Skills;
using GAME.Scripts.Sounds;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GAME.Scripts.UI.MainMenuScreen.SubScreens.Battle {
    public class BattleSubScreen : MainMenuSubScreenBase {
        [SerializeField] private Button _playButton, _closeButton, _rewardMeatButton;
        [SerializeField] private List<GameObject> _activeObjectOnPlay = new List<GameObject>();
        [SerializeField] private SpeedBoostButton _speedBoostButton;
        [SerializeField] private RewardedMeatButton _rewardedMeatButton;
        [SerializeField] private SkillButton _skillButton;
        [SerializeField] private CoinsCollected _coinsCollected;
        
        [Inject] private SignalBus _signalBus;
        [Inject] private SharedData _sharedData;
        [Inject] private SkillsDataSO _skillsDataSo;
        [Inject] private SkillsController _skillsController;
        
        private Skill _skill;


        
        public override void Show() {
            Init();
            base.Show();
        }

        private void Init() {
            _playButton.gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(false);
            _rewardedMeatButton.gameObject.SetActive(false);
            int skillId = _sharedData.SaveData.equippedSkillId;
            _skill = null;
            if (skillId > -1) _skill = _skillsDataSo.skills[skillId];
            _skillButton.Init(_skill);

            
            _coinsCollected.Init(_signalBus);
        }

        private void OnEnable() {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _speedBoostButton.Button.onClick.AddListener(OnSpeedBoostClicked);
            _rewardedMeatButton.Button.onClick.AddListener(OnRewardedMeatButtonClicked);
            _skillButton.Button.onClick.AddListener(OnSkillButtonClicked);
            _closeButton.onClick.AddListener(OnEarlyCompletionBattle);
            _rewardMeatButton.onClick.AddListener(OnRewardMeat);
            
            _signalBus.Subscribe<CoinCollectedBattleSignal>(_coinsCollected.UpdateCoinsCollectedText);
            _signalBus.Subscribe<QuitBattleSignal>(OnQuitBattle);
        }

        private void OnDisable() {
            _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            _speedBoostButton.Button.onClick.RemoveListener(OnSpeedBoostClicked);
            _rewardedMeatButton.Button.onClick.RemoveListener(OnRewardedMeatButtonClicked);
            _skillButton.Button.onClick.RemoveListener(OnSkillButtonClicked);
            _closeButton.onClick.RemoveListener(OnEarlyCompletionBattle);
            _rewardMeatButton.onClick.RemoveListener(OnRewardMeat);
            
            _signalBus.Unsubscribe<QuitBattleSignal>(OnQuitBattle);
            _signalBus.Unsubscribe<CoinCollectedBattleSignal>(_coinsCollected.UpdateCoinsCollectedText);
        }

        private void OnPlayButtonClicked()
        {
            _activeObjectOnPlay.ForEach(x=>x.gameObject.SetActive(true));
            _playButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(true);
            _rewardedMeatButton.gameObject.SetActive(true);
            _signalBus.Fire<StartGameSignal>();
            
            int currentStage = _sharedData.SaveData.currentTimeLine.playerStageId;

            if (_skill?.openStageId <= currentStage) {
                _skillButton.Active();
            }
        }

        private void OnQuitBattle()
        {
            
            _activeObjectOnPlay.ForEach(x=>x.gameObject.SetActive(false));
            _closeButton.gameObject.SetActive(false);
         
            DeactiveSkillButton();
        }

   

        private void OnEarlyCompletionBattle() {       
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickToClose});
            _signalBus.Fire(new QuitBattleSignal() {ScreenTypeForHide = new [] { ScreenType.MainMenuScreen}});
        }
  
        private void OnSpeedBoostClicked() {
            
        }

        private void OnRewardedMeatButtonClicked() {
            
        }

        private void OnSkillButtonClicked()
        {

            if (_skillsController.NumbersOfUseSkill<=_skill.numberOfUses)
            {
                DeactiveSkillButton();
            }
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Click});
            _signalBus.Fire(new UseSkillSignal { UsedSkill = _skill });

        }

        private void OnRewardMeat()
        {
            AdReward adRewardMeat = new AdReward();
            adRewardMeat.placementType = RewardedAdPlacementType.Meat;
            
            _signalBus.Fire(new ShowAdsRewardSignal(){adReward =  adRewardMeat});
        }
        private void DeactiveSkillButton()
        {
            _skillButton.Disactive();
        }
    }
}
