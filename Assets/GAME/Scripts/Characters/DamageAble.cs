using System;
using UnityEngine;

namespace GAME.Scripts.Characters {
    public enum DamageAbleType
    {
        Character = 0,
        Base = 1
    }
    public class DamageAble : MonoBehaviour {
        [SerializeField] private DamageAbleType _damageAbleType;

        private Action<float, float, bool> _onTakeDamage;

        public event Action OnDeath; 

        public bool IsAlive { get; private set; }
        public DamageAbleType DamageAbleType => _damageAbleType;

        public void Init(Action<float, float, bool> onTakeDamage) {
            _onTakeDamage = onTakeDamage;
            IsAlive = true;
        }
        
        public void TakeDamage(float damage, float buildingDamage, bool isCritical) {
            _onTakeDamage?.Invoke(damage, buildingDamage, isCritical);
        }

        public void SetAliveStatus(bool isAlive) {
            IsAlive = isAlive;
            if (!IsAlive) OnDeath?.Invoke();
        }
    }
}
