using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Characters;
using GAME.Scripts.Core;
using GAME.Scripts.EnemySpawn;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using GAME.Scripts.VFX;
using NavMeshPlus.Components;
using UnityEngine;
using Zenject;
using CharacterController = GAME.Scripts.Characters.CharacterController;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GAME.Scripts.Level {
    public class LevelViewController: ITickable {
        private LevelView _view;
        private LevelTimeLine _curTimeLine;
        private bool _isGainingResources;
        private float _resourceGainFillAmount;
        private float _coinCollectedAmount;
        private float _curTime;
        private float _gainResourceTime;
        private Dictionary<int, CharacterView> _characterPrefabs;
        private Dictionary<int, Queue<CharacterController>> _charactersPool;
        private List<CharacterController> _activePlayerCharacters;
        private List<CharacterController> _activeEnemyCharacters;
        private EnemySpawner _enemySpawner;
        private float _cheatsGainResourceTime;
        private int _amountCoinsCollectedBattle;
        private bool _isWasFirstAttackPlayer;

        [Inject] private SignalBus _signalBus;
        [Inject] private SharedData _sharedData;
        [Inject] private CharactersDataSO _charactersDataSO;
        [Inject] private LevelTimeLinesDataSO _timeLinesDataSO;
        [Inject] private ResourcesService _resourcesService;
        [Inject] private DiContainer _container;
        [Inject] private TickableManager _tickableManager;
        [Inject] private VFXService _vfxService;
        [Inject] private SoundsService _soundsService;

        public List<CharacterController> ActiveEnemyCharacters => _activeEnemyCharacters;
        public List<CharacterController> ActivePlayerCharacters => _activePlayerCharacters;
        public SharedData SharedData => _sharedData;
        public event Action<float, int> OnResourceFillChanged; 
        public int MainResourceCount { get; private set; }
        public int AmountCoinsCollectedBattle => _amountCoinsCollectedBattle;


        public LevelViewController(LevelView view) {
            _view = view;
        }

        public async UniTask Init(NavMeshSurface navMeshSurface) {
            
            LevelTimeLineSaveData curTimeLineSaveData = _sharedData.SaveData.currentTimeLine;
            LevelStage curStage = _timeLinesDataSO.timeLines[curTimeLineSaveData.timeLineId]
                .stages[curTimeLineSaveData.enemyStageId];
            
            _activePlayerCharacters = new();
            _activeEnemyCharacters = new();
            _isGainingResources = false;
            MainResourceCount = curStage.gainResourcesData.gainStartResources;
            _resourceGainFillAmount = 0f;
            _cheatsGainResourceTime = curStage.gainResourcesData.cheatGainResourceTime;
            _gainResourceTime =  Mathf.Max(curStage.gainResourcesData.gainResourceBaseTime - 
                                            curStage.gainResourcesData.gainResourceBaseTimePerLevel * 
                                            curTimeLineSaveData.gainResourceUpgradeLevel, 
                curStage.gainResourcesData.minGainResourceTime);
            _curTimeLine = _timeLinesDataSO.timeLines[_sharedData.SaveData.currentTimeLine.timeLineId];
            LevelBackground levelBackgroundPrefab = await _resourcesService.LoadAsset<LevelBackground>(_curTimeLine
                .stages[_sharedData.SaveData.currentTimeLine.enemyStageId].levelBackgroundPrefabLink);
            LevelBaseBuilding playerBasePrefab = await _resourcesService.LoadAsset<LevelBaseBuilding>(_curTimeLine
                .stages[_sharedData.SaveData.currentTimeLine.playerStageId].levelBaseBuildingPrefabLink);
            LevelBaseBuilding enemyBasePrefab = await _resourcesService.LoadAsset<LevelBaseBuilding>(_curTimeLine
                .stages[_sharedData.SaveData.currentTimeLine.enemyStageId].levelBaseBuildingPrefabLink);
            float playerBaseHealth = _curTimeLine.stages[_sharedData.SaveData.currentTimeLine.enemyStageId].stageBase.basePlayerHealth + 
                                     _sharedData.SaveData.currentTimeLine.stageBaseLevel;
            float enemyBaseHealth = _curTimeLine.stages[_sharedData.SaveData.currentTimeLine.enemyStageId].stageBase.baseEnemyHealth;
            _view.Init(levelBackgroundPrefab, playerBasePrefab, enemyBasePrefab, playerBaseHealth, enemyBaseHealth, _signalBus, _vfxService);
            navMeshSurface.BuildNavMesh();
            await PreloadCharactersPrefabs();
            CreateCharactersPools();
            _signalBus.Subscribe<SpawnCharacterSignal>(OnSpawnCharacterSignalReceived);
            _signalBus.Subscribe<BaseHealthUpgradeSignal>(OnBaseHealthUpgraded);
            _signalBus.Subscribe<MeatRewardAddSignal>(OnMeatReward);
            _signalBus.Subscribe<PlaySoundSignal>(OnPlaySound);
            
            _enemySpawner = new();
            _container.Inject(_enemySpawner);
            _enemySpawner.Init(_curTimeLine.stages[_sharedData.SaveData.currentTimeLine.enemyStageId]);
            _enemySpawner.SpawnEnemy += SpawnEnemy;
            
        }
        

        public void Dispose() {
            StopGainResource();
            _characterPrefabs.Clear();
            foreach (KeyValuePair<int, Queue<CharacterController>> charKVP in _charactersPool) {
                foreach (CharacterController curChar in charKVP.Value) {
                    _tickableManager.Remove(curChar);
                    curChar.Dispose();
                }
                charKVP.Value.Clear();
            }
            _charactersPool.Clear();

            foreach (CharacterController curChar in _activePlayerCharacters) {
                _tickableManager.Remove(curChar);
                curChar.Dispose();
            }
            _activePlayerCharacters.Clear();

            foreach (CharacterController curChar in _activeEnemyCharacters) {
                _tickableManager.Remove(curChar);
                curChar.Dispose();
            }
            _activeEnemyCharacters.Clear();
            
            _enemySpawner.SpawnEnemy -= SpawnEnemy;
            _enemySpawner.Dispose();
            _curTimeLine = null;
            _view.Dispose();
            _signalBus.Unsubscribe<SpawnCharacterSignal>(OnSpawnCharacterSignalReceived);
            _signalBus.Unsubscribe<BaseHealthUpgradeSignal>(OnBaseHealthUpgraded);
            _signalBus.Unsubscribe<MeatRewardAddSignal>(OnMeatReward);
            _signalBus.Unsubscribe<PlaySoundSignal>(OnPlaySound);
            Object.Destroy(_view.gameObject);
            _view = null;
        }

        private async UniTask PreloadCharactersPrefabs() {
            _characterPrefabs = new();
            
            foreach (int charId in _curTimeLine.stages[_sharedData.SaveData.currentTimeLine.playerStageId].characters) {
                CharacterView curCharView =
                    await _resourcesService.LoadAsset<CharacterView>(_charactersDataSO.CharactersData[charId]
                        .viewPrefabReference);
                
                _characterPrefabs.Add(charId, curCharView);
            }
            
            foreach (int charId in _curTimeLine.stages[_sharedData.SaveData.currentTimeLine.enemyStageId].characters) {
                if (_characterPrefabs.ContainsKey(charId)) continue;
                
                CharacterView curCharView =
                    await _resourcesService.LoadAsset<CharacterView>(_charactersDataSO.CharactersData[charId]
                        .viewPrefabReference);
                
                _characterPrefabs.Add(charId, curCharView);
            }
        }
        
        private void CreateCharactersPools() {
            _charactersPool = new();
            foreach (KeyValuePair<int, CharacterView> prefabKVP in _characterPrefabs) {
                _charactersPool.Add(prefabKVP.Key, new Queue<CharacterController>());
                for (int i = 0; i < 5; i++) {
                    SpawnNewCharacter(prefabKVP.Key);
                }
            }
        }

        private void SpawnNewCharacter(int id) {
            CharacterView curView = Object.Instantiate(_characterPrefabs[id], _view.CharacterParentTransform);
            CharacterController curChar = new CharacterController(_charactersDataSO.CharactersData[id], curView, _resourcesService, _vfxService);
            curChar.Init(OnCharacterDeath, _signalBus);
        
            _charactersPool[id].Enqueue(curChar);
            _tickableManager.Add(curChar);
        }

      

        public void StartGame() {
            _isGainingResources = true;
            _enemySpawner.StartSpawn();
            ActiveBattleSound();
        }
        
        public void Tick() {
            if (_isGainingResources) {
                _curTime += Time.deltaTime;
                float gainResourceTime = _sharedData.CheatMeatProduction ? _cheatsGainResourceTime : _gainResourceTime;
                _resourceGainFillAmount = _curTime / gainResourceTime;
                

                // update ui here
                OnResourceFillChanged?.Invoke(_resourceGainFillAmount, MainResourceCount);
                
                
                if (_curTime > gainResourceTime) {
                    _curTime = 0f;
                    MainResourceCount += 1;
                }
            }
        }
        public void SetCoinsCollect(int minesCoin)
        {
            
            Debug.Log("CPMPLE");
            _soundsService.PlaySound(SoundType.DropCoin);
            _amountCoinsCollectedBattle+=minesCoin;
            _signalBus.Fire(new CoinCollectedBattleSignal() {TotalCoin = _amountCoinsCollectedBattle, MinedCoin = minesCoin});
          
        }

        public void ResetCoinCollected()
        {
            _amountCoinsCollectedBattle = 0;
        }
        private void StopGainResource() {
            _isGainingResources = false;
        }

        private void OnSpawnCharacterSignalReceived(SpawnCharacterSignal args) {
            Character charData = args.CharacterData;
            if (MainResourceCount < charData.spawnCost)
            {
                _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBlock});
                return;
            }
            _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.ClickBuy});
            MainResourceCount -= charData.spawnCost;
            OnResourceFillChanged?.Invoke(_resourceGainFillAmount, MainResourceCount);
            CreateNewPlayerCharacter(charData);
        }

        private void CreateNewPlayerCharacter(Character charData) {
            if (_charactersPool[charData.id].Count == 0) SpawnNewCharacter(charData.id);

            CharacterController curChar = _charactersPool[charData.id].Dequeue();
            curChar.Activate(_view.PlayerBase.SpawnPointTransform.position, Vector3.right, 
                LayerMask.NameToLayer("Player"), LayerMask.GetMask("Enemy"), 
                _activeEnemyCharacters, _view.EnemyBase, false);
            _activePlayerCharacters.Add(curChar);
            Debug.Log("Name+++");
        }

        private void SpawnEnemy(int id, int count) {
            Character charData = _charactersDataSO.CharactersData[id];
            SpawnEnemiesAsync(charData, count).Forget();
        }

        private async UniTaskVoid SpawnEnemiesAsync(Character charData, int count)
        {
            for (int i = 0; i < count; i++)
            {
                CreateNewEnemyCharacter(charData);
              
                await UniTask.DelayFrame(1);

            }
        }
        private void CreateNewEnemyCharacter(Character charData) {
            if (_charactersPool[charData.id].Count == 0) SpawnNewCharacter(charData.id);

            if (!_isWasFirstAttackPlayer)
            {
                _signalBus.Fire(new PlaySoundSignal(){SoundType = SoundType.Enemy});
            }
        
            CharacterController curChar = _charactersPool[charData.id].Dequeue();

            curChar.Activate(_view.EnemyBase.SpawnPointTransform.position, Vector3.left, 
                LayerMask.NameToLayer("Enemy"), LayerMask.GetMask("Player"), 
                _activePlayerCharacters, _view.PlayerBase, true, _isWasFirstAttackPlayer ? false : true);
          
            _isWasFirstAttackPlayer = true;
            _activeEnemyCharacters.Add(curChar);
        }

        private void OnCharacterDeath(CharacterController character) {
            bool isEnemy = character.IsEnemy;
            List<CharacterController> activeList = isEnemy ? _activeEnemyCharacters : _activePlayerCharacters;
            activeList.Remove(character);
            //_charactersPool[character.CharData.id].Enqueue(character);
            _tickableManager.Remove(character);
            character = null;
        }

        private void OnBaseHealthUpgraded() {
            float playerBaseHealth = _curTimeLine.stages[_sharedData.SaveData.currentTimeLine.enemyStageId].stageBase.basePlayerHealth + 
                                     _sharedData.SaveData.currentTimeLine.stageBaseLevel;
            _view.PlayerBase.UpdateBaseHealthOnUpgrade(playerBaseHealth);
        }

        private void OnMeatReward(MeatRewardAddSignal args)
        {
            MainResourceCount += args.MeatAmount;
        }
        
        private void ActiveBattleSound()
        {
            _soundsService.PlayMusic();
            _soundsService.PlaySound(SoundType.StartWar);
        }

        private void OnPlaySound(PlaySoundSignal args)
        {
            _soundsService.PlaySound(args.SoundType);
        }
    }
}
