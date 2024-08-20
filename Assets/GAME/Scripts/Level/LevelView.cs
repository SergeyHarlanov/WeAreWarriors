using GAME.Scripts.VFX;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.Level {
    public class LevelView : MonoBehaviour {
        [SerializeField] private Transform _charactersParentTransform;
        [SerializeField] private GameObject _backgroundLevelWhite;
        [SerializeField] private float _roadYPosition;
        
        private LevelBackground _levelBackground;
        private LevelBaseBuilding _playerBase;
        private LevelBaseBuilding _enemyBase;
        private Camera _cam;
        private VFXService _vfxService;

        public Transform CharacterParentTransform => _charactersParentTransform;
        public LevelBaseBuilding PlayerBase => _playerBase;
        public LevelBaseBuilding EnemyBase => _enemyBase;

        public void Init(LevelBackground backgroundPrefab, LevelBaseBuilding playerBuildingPrefab, 
            LevelBaseBuilding enemyBuildingPrefab, float playerBaseHealth, float enemyBaseHealth, SignalBus signalBus, VFXService vfxService) {
            
            _levelBackground = Instantiate(backgroundPrefab, transform);
            _playerBase = Instantiate(playerBuildingPrefab, transform);
            _enemyBase = Instantiate(enemyBuildingPrefab, transform);
            _playerBase.Init(true, playerBaseHealth, signalBus, vfxService);
            _enemyBase.Init(false, enemyBaseHealth, signalBus, vfxService); 
            _levelBackground.Init(signalBus); 
            //_cam = Camera.main;
            //Vector3 leftPoint = _cam.ScreenToWorldPoint(Vector3.zero);
            //Vector3 rightPoint = _cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
            Vector3 leftPoint = new Vector3(-6f, 0f, 0f);
            Vector3 rightPoint = new Vector3(6f, 0f, 0f);
            leftPoint.y = _roadYPosition;
            leftPoint.z = 0;
            rightPoint.y = _roadYPosition;
            rightPoint.z = 0;

            _playerBase.transform.position = leftPoint;
            _enemyBase.transform.position = rightPoint;
        }
        
        public void Dispose() {
            _levelBackground.Dispose();
            _backgroundLevelWhite.SetActive(false);
        }
    }
}
