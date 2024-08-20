using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FTRuntime;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.Skills {
    public class SkillViewBase : MonoBehaviour {
        
        [SerializeField] protected SwfClipController _swfClipController;
        [SerializeField] protected int _FireFrame, _endFrame;
        
        protected CancellationTokenSource _cts;
        protected SkillsController _skillsController;
        protected LevelViewController _levelViewController;
        protected Skill _skill;
      
        
        protected async  UniTaskVoid UseSkillInternal(SkillsController skillsController, SignalBus signalBus)
        {
            transform.position = Vector3.zero;
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _cts.Token);
            gameObject.SetActive(true);
            SkillAnimation(skillsController, signalBus).Forget();
        }

        protected async UniTaskVoid SkillAnimation(SkillsController skillsController, SignalBus signalBus) {
            Debug.Log("useSkill");
            _swfClipController.GotoAndPlay(_swfClipController.clip.clip.Sequences[0].Name, 0);
            float FireDelay = _FireFrame / _swfClipController.clip.frameRate - 1;
            float FireTotalDelay = _endFrame / _swfClipController.clip.frameRate - (FireDelay +1);
            await UniTask.Delay(TimeSpan.FromSeconds(FireDelay), cancellationToken: _cts.Token);
            Fire();
            await UniTask.Delay(TimeSpan.FromSeconds(FireTotalDelay), cancellationToken: _cts.Token);
           gameObject.SetActive(false);
            signalBus.Fire<EndSkillSignal>();
        }

        protected virtual void Fire()
        {
            
        }
      
        public void Init(SkillsController skillsController, LevelViewController levelViewController ) {
           gameObject.SetActive(false);
           _skillsController = skillsController;
           _levelViewController = levelViewController;
           
           _cts = new();
        }
        
        public virtual void UseSkill(SkillsController skillsController, SignalBus signalBus, Skill skill) {
 

            UseSkillInternal(skillsController, signalBus).Forget();
            _skill = skill;
        }

        public virtual void DeActive() {
            gameObject.SetActive(false);
        }

        public virtual void Dispose() {
            _cts.Cancel();
            _cts = null;
             Destroy(gameObject);
        }
    }
}
