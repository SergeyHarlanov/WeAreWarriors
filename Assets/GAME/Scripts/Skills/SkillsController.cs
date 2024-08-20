using System.Threading;
using Cysharp.Threading.Tasks;
using GAME.Scripts.Core;
using GAME.Scripts.Level;
using GAME.Scripts.Signals;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.Skills {
    public class SkillsController {
        [Inject] private SignalBus _signalBus;
        [Inject] private ResourcesService _resourcesService; 
        
        private LevelViewController _levelViewController;
        private LevelView _levelView;
        private SkillViewBase _currentSkill;
        private bool _isSkillActive;
        private CancellationTokenSource _cts;
        private Skill _skill;

        private bool _isSkillSpawn;
        private int _numbersOfUseSkill;

        public int NumbersOfUseSkill => _numbersOfUseSkill;
        public LevelView LevelView => _levelView;
        public void Init(Skill skill, LevelView levelView, LevelViewController levelViewController) {
            _skill = skill;
            _isSkillActive = false;
            _currentSkill = null;
            _levelView = levelView;
            _levelViewController = levelViewController;
            
            _signalBus.Subscribe<StartGameSignal>(OnSpawnSkill);
            _signalBus.Subscribe<UseSkillSignal>(OnSkillUsed);
            _signalBus.Subscribe<EndSkillSignal>(OnSkillEnd);
        }

        public void UpdateSkill(Skill skill)
        {

            
            _skill = skill;

        }
        
        public void Dispose() {
            if (_skill == null) return;

           
            _currentSkill?.Dispose();

            
            _currentSkill = null;
            _skill = null;
                
            _signalBus.Unsubscribe<StartGameSignal>(OnSpawnSkill);
            _signalBus.Unsubscribe<UseSkillSignal>(OnSkillUsed);
            _signalBus.Unsubscribe<EndSkillSignal>(OnSkillEnd);
        }

        private async UniTaskVoid SpawnSkill(Skill skill) {
            SkillViewBase skillViewBase = await _resourcesService.LoadAsset<SkillViewBase>(skill.prefabLink);
            _currentSkill = Object.Instantiate(skillViewBase);
            _currentSkill.Init(this, _levelViewController);
        }

        private void OnSkillUsed(UseSkillSignal useSkillSignal) {
            if (_isSkillActive  ) return;

            _numbersOfUseSkill++;
            _isSkillActive = true;
            _currentSkill.UseSkill(this, _signalBus, _skill);
        }

        private void OnSpawnSkill() {
            Debug.Log("SPAWN SKILL"+_skill.name);
            SpawnSkill(_skill).Forget();
        }
        
        private void OnSkillEnd() {
            _isSkillActive = false;
            _currentSkill.DeActive();
        }
    }
}
