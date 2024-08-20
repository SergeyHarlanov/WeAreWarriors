using System;
using Assets.SimpleLocalization.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GAME.Scripts.Ads;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using GAME.Scripts.Skills;
using GAME.Scripts.Sounds;
using GAME.Scripts.UI;
using GAME.Scripts.VFX;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Zenject;

namespace GAME.Scripts.Core {
    public class AppCore : MonoBehaviour {
        [SerializeField] private Transform _soundsParentTransform;
        [SerializeField] private Transform _coinParentTransform;
        [SerializeField] private Transform _endPosCoinCollectedBattleMenu;
        [SerializeField] private Transform _endPosCoinCollectedMainMenu;
            
        [Inject] private DiContainer _container;
        [Inject] private SharedData _sharedData;
        [Inject] private SaveLoadService _saveLoadService;
        [Inject] private SignalBus _signalBus;
        [Inject] private UIService _uiService;
        [Inject] private ResourcesService _resourcesService;
        [Inject] private TickableManager _tickableManager;
        [Inject] private AdsService _adsService;
        [Inject] private SoundsService _soundsService;
        [Inject] private VFXService _vfxService;
        [Inject] private LevelTimeLinesDataSO _levelTimeLinesDataSO;
        [Inject] private NavMeshSurface _navMeshSurface;
        [Inject] private SkillsController _skillsController;
        [Inject] private SkillsDataSO _skillsDataSO;


        private LevelView _levelView;
        private void Start() {
            _canSwitchCheats = true;
            LocalizationManager.Read();
            LocalizationManager.Language = YandexGame.lang switch {
                "en" => "English",
                "ru" => "Russian",
                _ => "English",
            };
            Application.targetFrameRate = 60;
            DOTween.SetTweensCapacity(200, 50);
            
            YandexGame.ResetSaveProgress();
            YandexGame.SaveProgress();
            _resourcesService.Init();
            _adsService.Init();
            _uiService.Init();
            _soundsService.Init(_soundsParentTransform);
            _vfxService.Init(_coinParentTransform, _endPosCoinCollectedMainMenu, _endPosCoinCollectedBattleMenu);
            _saveLoadService.LoadData();
            
            _uiService.ShowScreen(ScreenType.LoadingScreen, true);

            LoadGame().Forget();

     
        }

        private void OnEnable() {
            _signalBus.Subscribe<StartGameSignal>(OnStartGameSignalReceived);
            _signalBus.Subscribe<RestartGameSignal>(OnRestartGameSignalReceived);
            _signalBus.Subscribe<ResetProgressSignal>(ResetGame);
            _signalBus.Subscribe<StageEvaluateSignal>(OnStageEvaluateSignalReceived);
            _signalBus.Subscribe<BaseDestroySignal>(OnBaseDestroySignalReceived); 
            _signalBus.Subscribe<QuitBattleSignal>(OnQuitBattle);
            _signalBus.Subscribe<MainScreenAddCoinSignal>(OnAddCoin);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<StartGameSignal>(OnStartGameSignalReceived);
            _signalBus.Unsubscribe<RestartGameSignal>(OnRestartGameSignalReceived);
            _signalBus.Unsubscribe<ResetProgressSignal>(ResetGame);
            _signalBus.Unsubscribe<StageEvaluateSignal>(OnStageEvaluateSignalReceived);
            _signalBus.Unsubscribe<BaseDestroySignal>(OnBaseDestroySignalReceived); 
            _signalBus.Unsubscribe<QuitBattleSignal>(OnQuitBattle);
            _signalBus.Unsubscribe<MainScreenAddCoinSignal>(OnAddCoin);

            
            _uiService.Dispose();
            _adsService.Dispose();
            _vfxService.Dispose();
        }

        private void OnStartGameSignalReceived() {
            
            StartGame();
        
        }

        private void OnAddCoin(MainScreenAddCoinSignal args)
        {
            _soundsService.PlaySound(SoundType.DropCoin);
            _sharedData.SaveData.coins += args.coin;
            _saveLoadService.SaveData();
            _signalBus.Fire<UpdateCurrencySignal>();
        }

        private async UniTaskVoid LoadGame(float secondLoad = 0.3f, Action actionAfterTimeLoad = null)
        {
            
            await UniTask.Delay(TimeSpan.FromSeconds(secondLoad-0.1f));
            actionAfterTimeLoad?.Invoke();
        
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            await SpawnLevel();
            _uiService.HideScreen(ScreenType.LoadingScreen);
            _uiService.ShowScreen(ScreenType.MainMenuScreen, true);
            Debug.Log("Quiting------------");
        }

        private void StartGame() {
         
            if (_sharedData.SaveData.equippedSkillId > -1) {
                Skill curSkill = _skillsDataSO.skills[_sharedData.SaveData.equippedSkillId];
                _skillsController.UpdateSkill(curSkill);
            }

           
            
         
            _sharedData.LevelController.StartGame();
      
        }

        private async UniTask SpawnLevel() {
            LevelView levelPrefab = await _resourcesService.LoadResource<LevelView>(0);
            LevelView levelView = Instantiate(levelPrefab);
            _levelView = levelView;
            _sharedData.LevelController = new LevelViewController(levelView);
            _container.Inject(_sharedData.LevelController);
            _tickableManager.Add(_sharedData.LevelController);
            await _sharedData.LevelController.Init(_navMeshSurface);
            
    
            Debug.Log(_sharedData.SaveData.equippedSkillId+"equiplevel");
            if (_sharedData.SaveData.equippedSkillId > -1) {
                Skill curSkill = _skillsDataSO.skills[_sharedData.SaveData.equippedSkillId];
                _skillsController.Init(curSkill, _levelView, _sharedData.LevelController);
            }
         
        }

        private bool _canSwitchCheats;
        private void Update() {
            if (_canSwitchCheats && (Input.GetKeyDown(KeyCode.C) || Input.touches.Length == 3)) {
                _canSwitchCheats = false;
                _uiService.SwitchCheatsScreen();
            }

            if (Input.touches.Length == 0) _canSwitchCheats = true;
        }

        private void ResetGame() {
            _uiService.CloseAllActiveScreens(true);
            Dispose();
            YandexGame.ResetSaveProgress();
            YandexGame.savesData.saves = String.Empty;
            _saveLoadService.LoadData();
            _uiService.ShowScreen(ScreenType.LoadingScreen, true);
            LoadGame().Forget();
        }

        private void Dispose() {
            if (_sharedData.LevelController != null) {
                _tickableManager.Remove(_sharedData.LevelController);
                _sharedData.LevelController.Dispose();
                _sharedData.LevelController = null;
            }

            _skillsController.Dispose();
            _resourcesService.Dispose();
        }

        private void OnRestartGameSignalReceived() {
            Dispose();
            _uiService.CloseAllActiveScreens(true);
            _uiService.ShowScreen(ScreenType.LoadingScreen, true);
            LoadGame().Forget();
        }

        private void OnStageEvaluateSignalReceived() {
            Dispose();
            SpawnLevel().Forget();
        }

        private void OnQuitBattle(QuitBattleSignal args)
        {
           
             /*
            _soundsService.StopMusic();
            
            if (args.MinedCoinForBattle>0)
            {
                Vector2 startPos = new Vector2(Screen.width / 2, Screen.height / 2);
                _vfxService.ShowVFXCoin( startPos, VFXType.EnemyDeath,  VEffect.TypeCoinEffect.Menu
                    , args.MinedCoinForBattle, 1.5f).Forget();
                
            }
            
            _uiService.CloseAllActiveScreens(true);


            
            Dispose();

            _uiService.ShowScreen(ScreenType.LoadingScreen, true);
            
            Action action = () =>
            {
                for (int i = 0; i < args.ScreenTypeForHide.Length; i++)
                {
                    _uiService.HideScreen(args.ScreenTypeForHide[i], true);
                }
                Dispose();
            };
            LoadGame(0.5f, action).Forget();
       */
       
           
            _soundsService.StopMusic();
            
            if (args.MinedCoinForBattle>0)
            {
                Vector2 startPos = new Vector2(Screen.width / 2, Screen.height / 2);
                  _vfxService.ShowVFXCoin( startPos, VFXType.EnemyDeath,  VEffect.TypeCoinEffect.Menu
                , args.MinedCoinForBattle, 1.5f).Forget();
                
            }
            
            _uiService.CloseAllActiveScreens(true);

            
            Dispose();

            _uiService.ShowScreen(ScreenType.LoadingScreen, true);

             Action  action = () =>
            {
                for (int i = 0; i < args.ScreenTypeForHide.Length; i++)
                {
                    _uiService.HideScreen(args.ScreenTypeForHide[i], true);
                }
                Dispose();
            };
            LoadGame(0.5f, action).Forget();
        }

        private void OnNextStage(BaseDestroySignal args)
        {
            if (!args.IsPlayerBase)
            {

                _sharedData.SaveData.currentTimeLine.enemyStageId++;
                //_sharedData.SaveData.currentTimeLine.openedCharacters.Clear();
               // _sharedData.SaveData.currentTimeLine.openedCharacters.Add(0);
               // _sharedData.SaveData.currentTimeLine.gainResourceUpgradeLevel = 0;
                //   CheckSkills();
                _saveLoadService.SaveData();
                    //     _signalBus.Fire<UpdateCurrencySignal>();
                // Init();
                //_signalBus.Fire<StageEvaluateSignal>();
                //_signalBus.Fire<UpdateAlertImagesSignal>();
            }
    
        }
        private void OnBaseDestroySignalReceived(BaseDestroySignal args) {
       
            if (!args.IsPlayerBase)
            {
                OnNextStage(args);
                _soundsService.PlaySound(SoundType.Victory);
                
                _signalBus.Fire(new ShowScreenSignal() { ScreenToShow = ScreenType.WinScreen });
            }
            else
            {
                _soundsService.PlaySound(SoundType.Defeat);
                _signalBus.Fire(new QuitBattleSignal() {ScreenTypeForHide = new [] { ScreenType.MainMenuScreen}, 
                    MinedCoinForBattle = _sharedData.LevelController.AmountCoinsCollectedBattle});
            }
          
       
   
        }

      

   
    
    }
}
