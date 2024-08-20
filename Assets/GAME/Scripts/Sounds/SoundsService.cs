using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GAME.Scripts.Core;
using GAME.Scripts.Level;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GAME.Scripts.Sounds {
    public enum SoundType {
        None = -1,
        Music = 0,
        Click = 1,
        ClickToClose = 2,
        ClickToBottomMenu = 3,
        SfxSet = 4,
        ClickBuy = 5,
        ClickBlock = 6,
        DropCoin = 7,
        Victory = 8,
        Defeat = 9,
        StartWar = 10,
        Enemy = 11,
        DeathBird = 12,
        DeathMan01 = 13,
        DeathMan02 = 14,
        DeathMeh01 = 15,
        HitBlasterMini = 16,
        HitBlaster = 17,
        HitBow = 18,
        HitCannonModern = 19,
        HitClub = 20,
        HitCrossBow = 21,
        HitEagle = 22,
        HitHandStone = 23,
        Hithand = 24,
        HitMachine = 25,
        HitMehHand= 26,
        HitPterodactyl = 27,
        HitRifle = 28,
        HitSling = 29,
        HitSpear = 30,
        HitSword = 31,
        HitTank = 32,
        EnvLevelBgTimeStage1 = 33,
        EnvLevelBgTimeStage2 = 34,
        EnvLevelBgTimeStage3 = 35,
        EnvLevelBgTimeStage4 = 36,
        EnvLevelBgTimeStage5 = 37,
        EnvLevelBgTimeStage6 = 38,
        MusLevelBg1 = 39,
        MusLevelBg2 = 40,
        MusLevelBg3 = 41,
        MusLevelBg4 = 42
    }

 
    public class SoundsService {
        private SoundsDataSO _soundsDataSO;
        private ResourcesService _resourcesService;
        private SharedData _sharedData;
        
        private Dictionary<SoundType, AudioClip> _soundsMap = new();
        private Queue<AudioSource> _audioSourcesPool = new();
        private AudioSource _evnMusicSource;
        private AudioSource _backMusicSource;
        private Transform _soundsParent;
        private CancellationTokenSource _cts;


        public SoundsService(SoundsDataSO soundsDataSO, ResourcesService resourcesService, SharedData sharedData) {
      
            _soundsDataSO = soundsDataSO;
            _resourcesService = resourcesService;
            _sharedData = sharedData;
        }

        public void Init(Transform soundsParent) {
            _cts = new();
            _soundsParent = soundsParent;
            
            for (int i = 0; i < 10; i++) {
                AudioSource curSource = SpawnNewAudioSource();
                _audioSourcesPool.Enqueue(curSource);
            }

            _evnMusicSource = SpawnNewAudioSource();
            _evnMusicSource.loop = true;
            
            _backMusicSource = SpawnNewAudioSource();
            
            
            _backMusicSource.loop = true;
            

        }

        private AudioSource SpawnNewAudioSource() {
            AudioSource audioObject = new GameObject().AddComponent<AudioSource>();
            audioObject.transform.SetParent(_soundsParent);

            return audioObject;
        }
        
        
        public async UniTaskVoid PlaySound(SoundType type, float delay = 0, float timeIncreaseSound = 0 , bool randomPitch = false) {
            if (!_sharedData.SaveData.sounds || type == SoundType.None) return;
            
            AudioClip clip;
            if (_soundsMap.TryGetValue(type, out AudioClip value)) {
                clip = value;
            } else {
                _soundsMap.Add(type, null);
                clip = await _resourcesService.LoadSound(_soundsDataSO.SoundsDataMap[type]);
                _soundsMap[type] = clip;    
            }

            if (_audioSourcesPool.Count == 0) {
                _audioSourcesPool.Enqueue(SpawnNewAudioSource());
            }
            AudioSource curSource = _audioSourcesPool.Dequeue();
            curSource.clip = clip;
            float duration = curSource.clip == null ? 0f : curSource.clip.length;
            if (randomPitch) curSource.pitch = Random.Range(0.8f, 1.2f);
            else curSource.pitch = 1f;
            
            if (delay > 0) await UniTask.Delay(TimeSpan.FromSeconds(delay));
            curSource.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: _cts.Token);
            curSource.clip = null;
            
            
            _audioSourcesPool.Enqueue(curSource);
        }

        public async UniTaskVoid PlayMusic()
        {

           
            if (!_sharedData.SaveData.music) return;
            AudioClip clip;
            
            //Environment Music
            clip = await _resourcesService.LoadSound(_soundsDataSO.SoundsDataMap[_sharedData.CurStage.soundType]);
            _evnMusicSource.clip = clip;
            _evnMusicSource.loop = true;
            _evnMusicSource.Play();
         
            //Background Music
         
            _backMusicSource.volume = 0;
            _backMusicSource.DOFade(1, 15).SetDelay(15);

            _backMusicSource.Play();

            while (true)
            {
                int done = Random.Range(39, 43);
                clip = await _resourcesService.LoadSound(_soundsDataSO.SoundsDataMap[(SoundType)done]);
                _backMusicSource.clip = clip;
                
                await UniTask.Delay(TimeSpan.FromSeconds(_backMusicSource.clip.length), cancellationToken:_cts.Token);
            }
            
        }
 

        public void StopMusic() {

                _evnMusicSource.Stop();
                _backMusicSource.Stop();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts = null;
        }


    }
}
