using System.Threading;
using GAME.Scripts.Core;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.VFX {
    public class VEffect : MonoBehaviour {
        public enum  TypeCoinEffect
        {
            Battle,
            Menu
        }
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private float _duration = 0.6f;

        public float Duration => _duration;

        protected SignalBus _signalBus;
        protected SharedData _sharedData;
        public virtual void Show(Vector3 startPosition, SignalBus signalBus, SharedData sharedData, TypeCoinEffect typeCoinEffect, Vector3 endPos)
        {
            _signalBus = signalBus;
            _sharedData = sharedData;
            transform.position = startPosition;
            gameObject.SetActive(true);
            if (_particleSystem)
            {
                _particleSystem.Play();
            }
        }

        public void Stop() {
            if (_particleSystem)
            {
                _particleSystem.Stop();
            }
    
            gameObject.SetActive(false);
        }
    }
}
