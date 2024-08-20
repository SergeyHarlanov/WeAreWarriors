using System;
using System.Threading;

namespace GAME.Scripts.Characters {
    public class AttackAble {
        protected float _attackForce;
        protected float _attackRate;
        protected float _buildingAttack;
        protected float _beforeAttackDelay;
        protected float _afterAttackDelay;
        protected DamageAble _target;
        protected CancellationTokenSource _cts;
        protected Action<DamageAbleType> _onHit;
        
        public bool IsAttacking { get; protected set; }
        
        public void Init(float attackForce, float buildingAttack, float attackRate,
            float beforeAttackDelay, float afterAttackDelay, Action<DamageAbleType> onHit)
        {
            _onHit = onHit;
            IsAttacking = false;
            _attackForce = attackForce;
            _buildingAttack = buildingAttack;
            _attackRate = attackRate;
            _beforeAttackDelay = beforeAttackDelay;
            _afterAttackDelay = afterAttackDelay;
            _cts = new();
        }

        public virtual void Dispose() {
            StopAttack();
            _cts = null;
        }

        public virtual void StartAttack(DamageAble target) {
            if (IsAttacking) return;
            
            _target = target;
            IsAttacking = true;
        }

        public virtual void StopAttack() {
            if (!IsAttacking) return;
            
            _cts.Cancel();
            _target = null;
            IsAttacking = false;
        }
    }
}
