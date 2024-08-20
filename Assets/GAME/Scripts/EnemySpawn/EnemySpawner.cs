using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Characters;
using GAME.Scripts.Level;
using Zenject;

namespace GAME.Scripts.EnemySpawn {
    public class EnemySpawner {
        [Inject] private CharactersDataSO _charactersDataSO;
        
        
        private LevelStage _stage;
        private float _nextSpawnDelay;
        private int _curWave;
        private CancellationTokenSource _cts;
        private bool _disposed;
        

        public event Action<int, int> SpawnEnemy;
        
        
        public void Init(LevelStage stage) {
            _stage = stage;
            _curWave = 0;
            _nextSpawnDelay = stage.spawnData.spawnWaves[_curWave].time;
            _cts = new ();
            _disposed = false;
        }

        public void StartSpawn() {
            SpawnAfterTime().Forget();
        }

        public void Dispose() {
            _disposed = true;
            _cts?.Cancel();
            _curWave = 0;
            _stage = null;
            _nextSpawnDelay = 0f;
        }

        private async UniTaskVoid SpawnAfterTime() {
            await UniTask.Delay(TimeSpan.FromSeconds(_nextSpawnDelay), cancellationToken: _cts.Token);
            if (_disposed) return;
            SpawnEnemy?.Invoke(_stage.spawnData.spawnWaves[_curWave].id, _stage.spawnData.spawnWaves[_curWave].count);
            _curWave++;
            if (_curWave == _stage.spawnData.spawnWaves.Count) return;
            _nextSpawnDelay = _stage.spawnData.spawnWaves[_curWave - 1].time - _nextSpawnDelay;
            SpawnAfterTime().Forget();
        }
    }
}
