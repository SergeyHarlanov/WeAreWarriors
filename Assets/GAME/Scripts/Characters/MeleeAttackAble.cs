using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GAME.Scripts.Characters {
    public class MeleeAttackAble : AttackAble {
        public override void StartAttack(DamageAble target) {
            if (!target.IsAlive || IsAttacking) return;
            
            base.StartAttack(target);
            Attack().Forget();
        }

        private async UniTaskVoid Attack() {
            await UniTask.Delay(TimeSpan.FromSeconds(_beforeAttackDelay), cancellationToken: _cts.Token);
            
            _target.TakeDamage(_attackForce, _buildingAttack, false);
            _onHit?.Invoke(_target.DamageAbleType);
      

            if (_target is null or { IsAlive: false }) {
                _target = null;
                IsAttacking = false;
                return;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_afterAttackDelay), cancellationToken: _cts.Token);
            _target = null;
            IsAttacking = false;
            
        }
    }
}
