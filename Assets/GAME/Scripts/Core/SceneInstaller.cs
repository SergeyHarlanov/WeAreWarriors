using GAME.Scripts.Ads;
using GAME.Scripts.Characters;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using GAME.Scripts.Skills;
using GAME.Scripts.Sounds;
using GAME.Scripts.UI;
using GAME.Scripts.VFX;
using NavMeshPlus.Components;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.Core {
    public class SceneInstaller : MonoInstaller {
        [SerializeField] private UIService _uiService;
        [SerializeField] private ResourcesLinksDataSO _resourcesLinksData;
        [SerializeField] private InitialDataSO _initialSaveDataSO;
        [SerializeField] private SoundsDataSO _soundsDataSO;
        [SerializeField] private LevelTimeLinesDataSO _levelTimeLinesDataSO;
        [SerializeField] private CharactersDataSO _charactersDataSO;
        [SerializeField] private NavMeshSurface _navMeshSurface;
        [SerializeField] private SkillsDataSO _skillsDataSO;

        
        public override void InstallBindings() {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<QuitBattleSignal>();
            Container.DeclareSignal<StartGameSignal>();
            Container.DeclareSignal<RestartGameSignal>();
            Container.DeclareSignal<ShowScreenSignal>();
            Container.DeclareSignal<HideScreenSignal>();
            Container.DeclareSignal<ShowVFXSignal>();
            Container.DeclareSignal<SpawnCharacterSignal>();
            Container.DeclareSignal<UpdateCurrencySignal>();
            Container.DeclareSignal<ResetProgressSignal>();
            Container.DeclareSignal<StageEvaluateSignal>();
            Container.DeclareSignal<UpdateAlertImagesSignal>();
            Container.DeclareSignal<BaseDestroySignal>();
            Container.DeclareSignal<UseSkillSignal>();
            Container.DeclareSignal<EndSkillSignal>();
            Container.DeclareSignal<BaseHealthUpgradeSignal>();
            Container.DeclareSignal<CoinCollectedBattleSignal>();
            Container.DeclareSignal<LocalizationCloneObjectsSignal>();
            Container.DeclareSignal<MeatRewardAddSignal>();
            Container.DeclareSignal<EvoulitionSheatPanelSignal>();
            Container.DeclareSignal<MainScreenAddCoinSignal>();
            Container.DeclareSignal<PlaySoundSignal>();
            Container.DeclareSignal<ShowAdsRewardSignal>();
            
            Container.Bind<SharedData>().AsSingle().NonLazy();
            Container.Bind<LevelTimeLinesDataSO>().FromInstance(_levelTimeLinesDataSO).AsSingle().NonLazy();
            Container.Bind<CharactersDataSO>().FromInstance(_charactersDataSO).AsSingle().NonLazy();
            Container.Bind<SkillsDataSO>().FromInstance(_skillsDataSO).AsSingle().NonLazy();
            Container.Bind<AdsService>().FromNew().AsSingle().NonLazy();
            Container.Bind<SoundsDataSO>().FromInstance(_soundsDataSO).AsSingle().NonLazy();
            Container.Bind<SoundsService>().FromNew().AsSingle().NonLazy();
            Container.Bind<ResourcesLinksDataSO>().FromInstance(_resourcesLinksData).AsSingle().NonLazy();
            Container.Bind<InitialDataSO>().FromInstance(_initialSaveDataSO).AsSingle().NonLazy();
            Container.Bind<SaveLoadService>().AsSingle().NonLazy();
            Container.Bind<UIService>().FromInstance(_uiService).AsSingle().NonLazy();
            Container.Bind<ResourcesService>().FromNew().AsSingle().WithArguments(_resourcesLinksData).NonLazy();
            Container.Bind<VFXService>().FromNew().AsSingle().NonLazy();
            Container.Bind<NavMeshSurface>().FromInstance(_navMeshSurface).AsSingle().NonLazy();
            Container.Bind<SkillsController>().FromNew().AsSingle().NonLazy();
        }
    }
}
