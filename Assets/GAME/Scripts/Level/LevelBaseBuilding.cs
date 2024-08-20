using System;
using System.Globalization;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Characters;
using GAME.Scripts.Signals;
using GAME.Scripts.VFX;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.Level {
    public class LevelBaseBuilding : MonoBehaviour
    {
        [SerializeField] private GameObject obstaclesPlayer, obstaclesEnemy;
        [SerializeField] private Transform _spawnPointTransform;
        [SerializeField] private Transform _topPointTransform;
        [SerializeField] private Transform _bottomPointTransform;
        [SerializeField] private DamageAble _damageAble;
        [SerializeField] private SpriteRenderer _backRenderer;
        [SerializeField] private SpriteRenderer _frontRenderer;
        [SerializeField] private Sprite _playerBackSprite;
        [SerializeField] private Sprite _enemyBackSprite;
        [SerializeField] private Sprite _playerFrontSprite;
        [SerializeField] private Sprite _enemyFrontSprite;
        [SerializeField] private MMProgressBar _healthBar;
        [SerializeField] private Transform _uiElements;
        [SerializeField] private TextMeshProUGUI _healthLabel;


        private bool _isPlayer;
        private float _startHealth;
        private float _health;
        private bool _isAlive;
        private SignalBus _signalBus;
        private VFXService _vfxService;
        
        public Transform SpawnPointTransform => _spawnPointTransform;
        public Transform TopPointTransform => _topPointTransform;
        public Transform BottomPointTransform => _bottomPointTransform;
        public DamageAble BuildingDamageAble => _damageAble;
        public MMProgressBar HealthBar => _healthBar;
        public bool IsAlive => _isAlive;
        

        public void Init(bool isPlayer, float startHealth, SignalBus signalBus, VFXService vfxService)
        {
            _vfxService = vfxService;
            _isPlayer = isPlayer;
            _signalBus = signalBus;
            _frontRenderer.sprite = isPlayer ? _playerFrontSprite : _enemyFrontSprite;
            _backRenderer.sprite = isPlayer ? _playerBackSprite : _enemyBackSprite;
            _frontRenderer.flipX = !isPlayer;
            _backRenderer.flipX = !isPlayer;

            if (isPlayer)
            {
               obstaclesPlayer.SetActive(true);
            }
            else
            {
                obstaclesEnemy.SetActive(true);
            }
            
            _spawnPointTransform.localPosition =
                new Vector3(isPlayer ? _spawnPointTransform.localPosition.x : -_spawnPointTransform.localPosition.x,
                    _spawnPointTransform.localPosition.y, 0f);
            _damageAble.transform.localPosition =
                new Vector3(isPlayer ? _damageAble.transform.localPosition.x : -_damageAble.transform.localPosition.x,
                    _damageAble.transform.localPosition.y, 0f);

            float posY = _uiElements.transform.localPosition.y;
            _uiElements.transform.localPosition =
                new Vector3(isPlayer ? -_uiElements.transform.localPosition.x : _uiElements.transform.localPosition.x,
                    posY, 0f);
            _startHealth = _health = startHealth;
            _healthLabel.text = startHealth.ToString(CultureInfo.InvariantCulture);
            _healthBar.SetBar01(1);
            
            _damageAble.Init(OnTakeDamage);
            _isAlive = true;
        }

        public void UpdateBaseHealthOnUpgrade(float startHealth) {
            _startHealth = _health = startHealth;
            _healthLabel.text = startHealth.ToString(CultureInfo.InvariantCulture);
            _healthBar.SetBar01(1);
        }

        private void OnTakeDamage(float damage, float buildingDamage, bool isCritical) {
            if (!_isAlive) return;
            
            _health -= buildingDamage;

            if (_health < 0) {
                _health = 0;
            }

            if (_health == 0) BaseDeathActions();

            _healthLabel.text = _health.ToString(CultureInfo.InvariantCulture);
            _healthBar.UpdateBar(_health, 0f, _startHealth);
        }

        private async UniTaskVoid BaseDeathActions() {
            _isAlive = false;
            _damageAble.SetAliveStatus(_isAlive);
            gameObject.SetActive(false);
            
            if (_isPlayer)
            {
               
        //        _signalBus.Fire(new QuitBattle()); 
            }
            else
            {
                //_vfxService.ShowVFXCoin(VFXType.EnemyDeath, Camera.main.WorldToScreenPoint(transform.position));
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            _signalBus.Fire(new BaseDestroySignal { IsPlayerBase = _isPlayer});

            await Task.Delay(2000);
            
            Debug.Log("COINADD");
        }
    }
}
