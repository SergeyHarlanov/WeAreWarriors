using System;
using FTRuntime;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.AI;

namespace GAME.Scripts.Characters {
    public class CharacterView : MonoBehaviour {
        [SerializeField] private NavMeshAgent _navAgent;
        [SerializeField] private DamageAble _damageAble;
        [SerializeField] private MMProgressBar _healthBar;
        [SerializeField] private SwfClipController _anim;
        [SerializeField] private Transform _bulletStartTransform;
        [SerializeField] private SwfClip _swfClip;
        
        public NavMeshAgent NavAgent => _navAgent;
        public DamageAble DamageAble => _damageAble;
        public MMProgressBar HealthBar => _healthBar;
        public SwfClipController Anim => _anim;
        public Transform BulletStartTransform => _bulletStartTransform;
        public SwfClip SwfClip => _swfClip;
        
        public Vector3 TargetPos { get; set; }

       

        public void UpdateTargetPos(Vector3 pos) {
            TargetPos = pos;
        }

        public void Dispose() {
            
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, TargetPos);
        }
    }
}
