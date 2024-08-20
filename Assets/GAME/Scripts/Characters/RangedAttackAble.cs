using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GAME.Scripts.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace GAME.Scripts.Characters {
    public class RangedAttackAble: AttackAble {
        private ResourcesService _resourcesService;
        private Bullet _bulletPrefab;
        private Transform _startPointTransform;
        private float _bulletJumpPower;
        private Queue<Bullet> _bulletsPool;
        private List<Bullet> _activeBullets;
        private bool _disposed;
        private CancellationTokenSource _bulletCTS;
        
        
        
        public RangedAttackAble(ResourcesService resourcesService, AssetReference bulletReference, Transform startPointTransform, float bulletJumpPower) {
            _resourcesService = resourcesService;
            _startPointTransform = startPointTransform;
            _bulletJumpPower = bulletJumpPower;
            InitBullets(bulletReference).Forget();
            _bulletCTS = new();
            _disposed = false;
        }

        public override void Dispose() {
            base.Dispose();
            
            _bulletCTS.Cancel();
            foreach (Bullet curBullet in _activeBullets) {
                curBullet.transform.DOKill();
                Object.Destroy(curBullet);
            }
            _activeBullets.Clear();
            _activeBullets = null;

            foreach (Bullet curBullet in _bulletsPool) {
                curBullet.transform.DOKill();
                Object.Destroy(curBullet);
            }
            _bulletsPool.Clear();
            _bulletsPool = null;
            _bulletPrefab = null;
            _disposed = true;
        }

        private async UniTaskVoid InitBullets(AssetReference bulletReference) {
            _bulletsPool = new();
            _activeBullets = new();
            _bulletPrefab = await _resourcesService.LoadAsset<Bullet>(bulletReference);
            for (int i = 0; i < 10; i++) {
                CreateNewBullet();
            }
        }

        private void CreateNewBullet() {
            Bullet curBullet = Object.Instantiate(_bulletPrefab, _startPointTransform);
            curBullet.gameObject.SetActive(false);
            _bulletsPool.Enqueue(curBullet);
        }

        public override void StartAttack(DamageAble target) {
            if (!target.IsAlive || IsAttacking) return;
            
            base.StartAttack(target);
            Attack().Forget();
        }

        private async UniTaskVoid Attack() {
            await UniTask.Delay(TimeSpan.FromSeconds(_beforeAttackDelay), cancellationToken: _cts.Token);
            if (!_target.IsAlive) {
                _target = null;
                IsAttacking = false;
                return;
            }
            if (_bulletsPool.Count == 0) CreateNewBullet();
            Bullet curBullet = _bulletsPool.Dequeue();
            _activeBullets.Add(curBullet);
            curBullet.transform.position = _startPointTransform.position;
            curBullet.gameObject.SetActive(true);
            curBullet.transform.DOJump(_target.transform.position, _bulletJumpPower, 1, _attackRate * 0.5f).SetEase(Ease.Linear);
            GiveDamage(curBullet, _attackRate * 0.5f).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_afterAttackDelay), cancellationToken: _cts.Token);
            _target = null;
            IsAttacking = false;
        }

        private async UniTaskVoid GiveDamage(Bullet curBullet, float delay) {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _bulletCTS.Token);
            if (_disposed) return;
            if (_target is { IsAlive: true })
            {
                _target.TakeDamage(_attackForce, _buildingAttack, false);
                _onHit?.Invoke(_target.DamageAbleType);
            }
            curBullet.gameObject.SetActive(false);
            _activeBullets.Remove(curBullet);
            _bulletsPool.Enqueue(curBullet);
        }
    }
}
