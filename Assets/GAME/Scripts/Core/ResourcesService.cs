using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Characters;
using GAME.Scripts.Level;
using GAME.Scripts.VFX;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GAME.Scripts.Core {
    public class ResourcesService {
        private ResourcesLinksDataSO _linksData;

        private bool _disposed;
        private List<AsyncOperationHandle> _loadedResources = new();
        
        
        public ResourcesService(ResourcesLinksDataSO linksData) {
            _linksData = linksData;
        }

        public void Init() {
            _disposed = false;
        }

        public async UniTask<T> LoadResource<T>(int id, bool unloadResourcesOnGameRestart = true) where T: class {
            switch (typeof(T).Name) {
                case nameof(LevelView):
                    AsyncOperationHandle<GameObject> levelLoadHandler = 
                        Addressables.LoadAssetAsync<GameObject>(_linksData.LevelsLinks[id]);
                    await levelLoadHandler;
                    T levelPrefab = levelLoadHandler.Result.GetComponent<T>();
                    if (unloadResourcesOnGameRestart) _loadedResources.Add(levelLoadHandler);
                    return levelPrefab;
                default:
                    return null;
            }
        }
        
        public async UniTask<T> LoadAsset<T>(AssetReference reference) where T: class {
            AsyncOperationHandle<GameObject> levelLoadHandler = Addressables.LoadAssetAsync<GameObject>(reference);
            await levelLoadHandler;
            _loadedResources.Add(levelLoadHandler);
            
            return levelLoadHandler.Result.GetComponent<T>();
        }
        
        public async UniTask<Sprite> LoadSprite(AssetReference reference) {
            AsyncOperationHandle<Sprite> spriteLoadHandler = 
                Addressables.LoadAssetAsync<Sprite>(reference);
            await spriteLoadHandler;
            _loadedResources.Add(spriteLoadHandler);
            
            return spriteLoadHandler.Result;
        }
        
        public async UniTask<AudioClip> LoadSound(AssetReference reference) {
            AsyncOperationHandle<AudioClip> soundLoadHandler = 
                Addressables.LoadAssetAsync<AudioClip>(reference);
            await soundLoadHandler;
            
            return soundLoadHandler.Result;
        }

        
        public async UniTask<VEffect> LoadEffect(VFXType type) {
            AsyncOperationHandle<GameObject> effectLoadHandler = 
                Addressables.LoadAssetAsync<GameObject>(_linksData.EffectsLinks[type]);
            await effectLoadHandler;
            
            Debug.Log(effectLoadHandler.Result.name+"::NAME");
            return effectLoadHandler.Result.GetComponent<VEffect>();
        }
        public async UniTask<GameObject> LoadEffectCoin(VFXType type) {
            AsyncOperationHandle<GameObject> effectLoadHandler = 
                Addressables.LoadAssetAsync<GameObject>(_linksData.EffectsLinks[type]);
            await effectLoadHandler;
            
            Debug.Log(effectLoadHandler.Result.name+"::NAME");
            return effectLoadHandler.Result;
        }
        public void Dispose() {
            foreach (AsyncOperationHandle curResource in _loadedResources) {
                Addressables.Release(curResource);
            }
            _loadedResources.Clear();
            _disposed = true;
        }
    }
}
