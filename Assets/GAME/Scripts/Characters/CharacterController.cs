using System;
using System.Collections.Generic;
using DG.Tweening;
using FTRuntime;
using GAME.Scripts.Core;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using GAME.Scripts.Sounds;
using GAME.Scripts.VFX;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GAME.Scripts.Characters {
    public class CharacterController: ITickable {
        private CharacterView _view;
        private List<CharacterController> _enemies;
        private LevelBaseBuilding _enemyBase;
        private ResourcesService _resourcesService;
        private VFXService _vfxService;
        private SignalBus _signalBus;

        private float _damageMultiplier = 1;
        private float _moveMultiplier = 1;
        private bool _isStunning;
        
        private Vector3 _direction;
        private bool _isMoving;
        private bool _active;
        private RaycastHit2D[] _hits;
        private LayerMask _attackMask;
        private Transform _viewTransform;
        private AttackAble _attackAble;
        private float _startHealth;
        private float _health;
        private Action<CharacterController> _onDeath;
        private int _walkAnimationId;
        private int _attackAnimationId;
        private int _curAnimationId;


        public float GetDamage => CharData.attack;
        public Vector2 GetCurrentPos => _view.NavAgent.transform.position;
        public Character CharData { get; private set; }
        public bool IsEnemy { get; private set; }

        private Vector3 _startPos;
        private bool _isSortedChanged;
        private bool _wasFirstAttackPlayer;
        
        public CharacterController(Character charData, CharacterView view, ResourcesService resourcesService, VFXService vfxService) {
            CharData = charData;
            _view = view;
            _vfxService = vfxService;
            _viewTransform = _view.transform;
            _resourcesService = resourcesService;
            _isMoving = false;
            _active = false;
        }

        public void Init(Action<CharacterController> onDeath, SignalBus signalBus)
        {
            _onDeath = onDeath;
            _hits = new RaycastHit2D[30];
            _view.NavAgent.updateRotation = false;
            _view.NavAgent.updateUpAxis = false;
            _view.NavAgent.speed = CharData.speed;
            _view.NavAgent.isStopped = true;
            _view.gameObject.SetActive(false);
            _attackAble = CharData.attackType switch {
                CharacterAttackType.Melee => new MeleeAttackAble(),
                CharacterAttackType.Ranged => new RangedAttackAble(_resourcesService, CharData.bulletPrefabReference, _view.BulletStartTransform, CharData.bulletJumpPower),
                _ => new MeleeAttackAble(),
            };
            float beforeAttackDelay = (float)CharData.attackFrame / (float)_view.Anim.clip.frameRate;
            float afterAttackDelay =
                (float)(_view.Anim.clip.clip.Sequences[CharData.playerAttackAnimationId].Frames.Count - CharData.attackFrame) /
                (float)_view.Anim.clip.frameRate;
            _attackAble.Init(CharData.attack, CharData.buildingAttack, CharData.attackRate, beforeAttackDelay, afterAttackDelay, OnHit);
            _view.DamageAble.Init(OnTakeDamage);
            _curAnimationId = -1;
            _view.SwfClip.sortingOrder = -7;
            _signalBus = signalBus;
        }

        public void Dispose() {
            _active = false;
            StopMove();
            _attackAble.Dispose();
            _attackAble = null;
            _view.Dispose();
            Object.Destroy(_view.gameObject);
            _onDeath = null;
            _viewTransform = null;
            _view = null;
        }

        public void Tick() {
            if (!_active || !_enemyBase.IsAlive) return;

            (float distance, DamageAble damageAble, bool isBase) nearestTargetData = GetNearestTargetData();
            float distanceDelta = nearestTargetData.isBase ? -1.5f : 0f;
            _view.UpdateTargetPos(nearestTargetData.damageAble.transform.position);
         
            // Attack
            if (nearestTargetData.distance + distanceDelta < CharData.attackDistance && !_isStunning ) {
                if (_wasFirstAttackPlayer)
                {
                    _wasFirstAttackPlayer = false;
                    return;
                }

            
                StopMove(); 
                _attackAble.StartAttack(nearestTargetData.damageAble);
                
                if (_curAnimationId != _attackAnimationId) {
                    _curAnimationId = _attackAnimationId;
                    _view.Anim.loopMode = SwfClipController.LoopModes.Loop;
                    _view.Anim.GotoAndPlay(_view.Anim.clip.clip.Sequences[_attackAnimationId].Name, 0);
                }
                // Change First Attack
               
            }
          
        
            // Move
            else {
                MoveTo(nearestTargetData.damageAble.transform.position);
            }
            //Change Sorted Layer
            if (Vector2.Distance(_viewTransform.position, _startPos)>=1.5f && !_isSortedChanged) {
               
                _isSortedChanged = true;
                _view.SwfClip.sortingOrder = 10;
            }
        }

        public void Activate(Vector3 startPos, Vector3 direction, LayerMask ourLayer, LayerMask layerToAttack, 
            List<CharacterController> enemies, LevelBaseBuilding enemyBase, bool isEnemy, bool wasFirstAttackPlayer = false) {
            _enemies = enemies;
            _enemyBase = enemyBase;
            _direction = direction;
            IsEnemy = isEnemy;
            _walkAnimationId = isEnemy ? CharData.enemyWalkAnimationId : CharData.playerWalkAnimationId;
            _attackAnimationId = isEnemy ? CharData.enemyAttackAnimationId : CharData.playerAttackAnimationId;
            _health = _startHealth = CharData.health;
            _view.HealthBar.SetBar01(1);
            _view.transform.position = startPos;
            SetLayer(ourLayer);
            _attackMask = layerToAttack;
            _view.gameObject.SetActive(true);
            _view.DamageAble.SetAliveStatus(true);
            _view.Anim.transform.localScale = new Vector3(isEnemy ? -1f : 1f, 1f, 1f);
            if (_attackAble is RangedAttackAble) {
                _view.BulletStartTransform.localPosition = new Vector3(
                    isEnemy ? -_view.BulletStartTransform.localPosition.x : _view.BulletStartTransform.localPosition.x,
                    _view.BulletStartTransform.localPosition.y, _view.BulletStartTransform.localPosition.z);
            }

            _active = true;
            _startPos = _viewTransform.position;
            _wasFirstAttackPlayer = wasFirstAttackPlayer;
            _viewTransform.name = "Enemy"+Random.Range(0, 9999);
            Debug.Log("SpawnEnemy"+ _viewTransform.name);
        }

        public void DeActivate() {
            _isMoving = false;
            if (_view.NavAgent.isActiveAndEnabled) {
                _view.NavAgent.isStopped = true;
                _view.NavAgent.SetDestination(_viewTransform.position);
            }

            _active = false;
            _attackAble.StopAttack();
            _view.gameObject.SetActive(false);
            _curAnimationId = -1;
            _attackAble.Dispose();
            _view.Dispose();
            Object.Destroy(_view.gameObject);
            _onDeath?.Invoke(this);
            _onDeath = null;
        }

        public void SetDamage(int damage)
        {
            OnTakeDamage(damage, 0, false);
        }

        public void ActiveBuff(int indexBuff, float scale = 1.5f, float damage = 1.5f,float moveMultipliyer = 1, Action fabAction = null)
        {
            _viewTransform.DOPunchScale(Vector3.one * 1.1f, 0.3f);
            switch (indexBuff)
            {
                //potion
                case 2:
                {
                    _viewTransform.DOScale(Vector2.one * scale, 0.5f);
                    _damageMultiplier = damage;
                    break;
                }
                //smoke
                case 3:
                {
                    _moveMultiplier = Mathf.Clamp( 1 - moveMultipliyer, 0.01f, 1f);
                    _view.NavAgent.speed = CharData.speed * _moveMultiplier;
                    break;
                }
                //arrow
                case 0:
                {
                    _isStunning = true;
                    _view.NavAgent.speed = 0;
                    break;
                }
                //fab
                case 1:
                {
                    fabAction?.Invoke();
                    break;
                }
            }
        }
        public void DisactiveBuff(int indexBuff)
        {
            switch (indexBuff)
            {
                //potion
                case 2:
                {
                    _viewTransform.DOScale(Vector2.one * 1, 0.5f);
                    _damageMultiplier = 1;
                    break;
                }
                //smoke
                case 3:
                {
                    _moveMultiplier = 1;
                    _view.NavAgent.speed = CharData.speed * _moveMultiplier;
                    break;
                }
                //arrow
                case 0:
                {
                    _isStunning = false;
                    _view.NavAgent.speed = CharData.speed ;
                    break;
                }
                //fab
                case 1:
                {

                    break;
                }
            }
        }
        private void SetLayer(LayerMask layer) {
            _view.gameObject.layer = layer;
        }

        private void MoveTo(Vector3 position) {
            if (_curAnimationId != _walkAnimationId) {
                _curAnimationId = _walkAnimationId;
                _view.Anim.loopMode = SwfClipController.LoopModes.Loop;
                _view.Anim.GotoAndPlay(_view.Anim.clip.clip.Sequences[_walkAnimationId].Name, 0);
            } 
            _view.NavAgent.isStopped = false;
            _view.NavAgent.SetDestination(position);
            _isMoving = true;
        }

        private void StopMove() {
            if (!_isMoving) return;
            
            if (_view.NavAgent.isActiveAndEnabled) {
                _view.NavAgent.SetDestination(_viewTransform.position);
                _view.NavAgent.isStopped = true;
            }

            _isMoving = false;
          
        }

        private (float distance, DamageAble damageAble, bool isBase) GetNearestTargetData() {
            (float distance, DamageAble damageAble, bool isBase) data = new () {
                distance = Vector3.Distance(_enemyBase.transform.position, _viewTransform.position),
                damageAble = _enemyBase.BuildingDamageAble,
                isBase = true,
            };

            float minDistance = float.MaxValue;
            foreach (CharacterController curChar in _enemies) {
                if (!curChar._active || !curChar._view.DamageAble.IsAlive) continue;
                
                float curDistance = Vector2.Distance(curChar._viewTransform.position, _viewTransform.position);
                if (curDistance < minDistance) {
                    minDistance = curDistance;
                    data.distance = minDistance;
                    data.damageAble = curChar._view.DamageAble;
                    data.isBase = false;
                }
            }
            
            return data;
        }

        private Vector3 GetTargetPosition() {
            if (_enemies.Count == 0) {
                float deltaY = _enemyBase.TopPointTransform.position.y - _enemyBase.BottomPointTransform.position.y;
                Vector3 targetPoint = _enemyBase.BottomPointTransform.position + Vector3.up * Random.Range(0f, deltaY);
                return targetPoint;
            }

            return _viewTransform.position;
        }

        private void CheckDamageAbles() {
            int nearDamageAbles = Physics2D.CircleCastNonAlloc(_viewTransform.position, 
                1f, Vector2.zero, _hits, 10f, _attackMask);

            _isMoving = nearDamageAbles <= 0;
            
            for (int i = 0; i < nearDamageAbles; i++) {
                RaycastHit2D curHit = _hits[i];
                if (curHit.collider != null) {
                    if (curHit.collider.TryGetComponent(out DamageAble curDamageAble)) {
                        curDamageAble.TakeDamage(CharData.attack * _damageMultiplier, CharData.buildingAttack, false);
                        return;
                    }
                }
            }
        }

        private void OnTakeDamage(float damage, float buildingDamage, bool isCritical) {
            if (!_active) return;
            
        
            
            _health -= damage;
            if (_health < 0) _health = 0;
            _view.HealthBar.UpdateBar(_health, 0f, _startHealth);

            if (_health == 0) {
                DeathActions();
            }
        }

        private void DeathActions()
        {
            _signalBus.Fire(new PlaySoundSignal(){SoundType = CharData.deathSoundTypePlayer});
            
            if (IsEnemy)
            {
                _vfxService.ShowVFXCoin(Camera.main.WorldToScreenPoint(_viewTransform.position),
                    VFXType.EnemyDeath, VEffect.TypeCoinEffect.Battle).Forget();
            }

            _active = false;
            _view.DamageAble.SetAliveStatus(false);
            DeActivate();
        }

        private void OnHit(DamageAbleType damageAbleType)
        {
            if (damageAbleType == DamageAbleType.Base && !IsEnemy)
            {
                _vfxService.ShowVFXCoin(Camera.main.WorldToScreenPoint(_viewTransform.position),
                    VFXType.EnemyDeath, VEffect.TypeCoinEffect.Battle).Forget();
                
              

            }
            _signalBus.Fire(new PlaySoundSignal(){SoundType = CharData.hitSoundTypePlayer});
        }
    }
}
