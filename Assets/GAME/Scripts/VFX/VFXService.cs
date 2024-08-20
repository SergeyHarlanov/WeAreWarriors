using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GAME.Scripts.VFX {
    public enum VFXType {
        EnemyDeath = 0,
        CoinEffect = 1,
    }
    
    
    public class VFXService {
        private Dictionary<VFXType, Queue<VEffect>> _effectsPool;

        [Inject] private ResourcesService _resourcesService;
        [Inject] private SignalBus _signalBus;
        [Inject] private SharedData _sharedData;
        [Inject] private SaveLoadService _saveLoadService;
        
        private Transform _parentCoins;
        
        private Transform _endPosCoinCollectedMainManu;
        private Transform _endPosCoinCollectedBattleMenu;

        private CancellationTokenSource _cancellationTokenSource;
        public void Init(Transform parentCoins, Transform endPosCoinCollectedMainManu, Transform endPosCoinCollectedBattleMenu) {
            if (_effectsPool == null) _effectsPool = new();
            _parentCoins = parentCoins;
            _endPosCoinCollectedMainManu = endPosCoinCollectedMainManu;
            _endPosCoinCollectedBattleMenu = endPosCoinCollectedBattleMenu;
            _signalBus.Subscribe<ShowVFXSignal>(OnShowVFXSignalReceived);
            _cancellationTokenSource = new();
            _signalBus.Subscribe<StartGameSignal>(()=>_cancellationTokenSource.Cancel());
        }

        public void Dispose() {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
            _signalBus.Unsubscribe<ShowVFXSignal>(OnShowVFXSignalReceived);
        }

        private void OnShowVFXSignalReceived(ShowVFXSignal args) {
            Vector3 effectPosition = args.EffectPosition;
            VFXType effectType = args.EffectType;
            ShowVFX(effectType, effectPosition).Forget();
        }

        public async UniTaskVoid ShowVFXCoin(Vector3 startPosition,  VFXType type, VEffect.TypeCoinEffect typeCoinEffect, int countCoins = 1, float timeDelay = 0)
        {
        
            await UniTask.Delay(TimeSpan.FromSeconds(timeDelay));
            
            
            _cancellationTokenSource = new();

            int countCoinChange = (typeCoinEffect == VEffect.TypeCoinEffect.Battle) ? countCoins : 25;
            for (int j = 0; j < countCoinChange; j++)
            {
                Queue<VEffect> effectPool;
                if (_effectsPool.TryGetValue(type, out effectPool)) {
                    
                }
                else {
                    _effectsPool.Add(type, new Queue<VEffect>());
                    effectPool = _effectsPool[type];
                }

                if (effectPool.Count == 0) 
                    await SpawnEffectCoin(type);
                VEffect effectCoin = _effectsPool[type].Dequeue();
                
           
                effectCoin.Show(startPosition,_signalBus, _sharedData, typeCoinEffect, 
                    typeCoinEffect == VEffect.TypeCoinEffect.Battle ? _endPosCoinCollectedBattleMenu.position : _endPosCoinCollectedMainManu.position);

                EnqueueEffect(effectCoin, type);
            }

            if (typeCoinEffect == VEffect.TypeCoinEffect.Menu)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.6f));
                _signalBus.Fire<MainScreenAddCoinSignal>(new MainScreenAddCoinSignal(){coin = countCoins});
            }
        }
       
     
        public async UniTaskVoid ShowVFX(VFXType type, Vector3 position) {
            Queue<VEffect> effectPool;
            if (_effectsPool.TryGetValue(type, out effectPool)) {
            }
            else {
                _effectsPool.Add(type, new Queue<VEffect>());
                effectPool = _effectsPool[type];
            }

            if (effectPool.Count == 0) await SpawnEffect(type);
            VEffect effect = _effectsPool[type].Dequeue();
            effect.Show(position, _signalBus, _sharedData, VEffect.TypeCoinEffect.Menu, Vector3.zero);
      
            await UniTask.Delay(TimeSpan.FromSeconds(effect.Duration));
            effect.Stop();
            _effectsPool[type].Enqueue(effect);
        }


        private async UniTask SpawnEffect(VFXType type) {
            VEffect effectPrefab = await _resourcesService.LoadEffect(type);
            VEffect effect = Object.Instantiate(effectPrefab);
            effect.gameObject.SetActive(false);
            _effectsPool[type].Enqueue(effect);
            
        }
        private async UniTask SpawnEffectCoin(VFXType type) {
            GameObject effectPrefab = await _resourcesService.LoadEffectCoin(type);
            GameObject effect = Object.Instantiate(effectPrefab);
            effect.transform.parent = _parentCoins;
            effect.gameObject.SetActive(false);
            _effectsPool[type].Enqueue(effect.GetComponent<CoinEffect>());
            
        }

        private async UniTaskVoid EnqueueEffect(VEffect effect, VFXType type)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(effect.Duration));
            effect.Stop();
            _effectsPool[type].Enqueue(effect);
        }
    }
}
