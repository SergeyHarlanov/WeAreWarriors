using System;
using Cysharp.Threading.Tasks;
using Zenject;

namespace GAME.Scripts.Skills.TypeSkills {
    public class SmokeSkillView : SkillViewBase {
        protected override void Fire()
        {
            UseBuff().Forget();
        }

        private async UniTaskVoid UseBuff()
        {
            
         //   int levelSkill = 5;
            int levelSkill = _levelViewController.SharedData.SaveData.skillsUpgradesData[2].level;
            float percentDamageSkill = levelSkill*0.01f;
            float percentSlowDown = 0.01f + percentDamageSkill;
            _levelViewController.ActiveEnemyCharacters.ForEach(x=>x.ActiveBuff(3, 0, 0, percentSlowDown));
         //   await UniTask.Delay(TimeSpan.FromSeconds(_skill.timeAwait), cancellationToken: _cts.Token);
        //    _levelViewController.ActiveEnemyCharacters.ForEach(x=>x.DisactiveBuff(2));
        }
    }
}
