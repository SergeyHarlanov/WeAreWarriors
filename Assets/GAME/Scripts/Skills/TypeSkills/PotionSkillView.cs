using System;
using Cysharp.Threading.Tasks;
using Zenject;

namespace GAME.Scripts.Skills.TypeSkills {
    public class PotionSkillView : SkillViewBase {
        protected override void Fire()
        {
            UseBuff().Forget();
        }

        private async UniTaskVoid UseBuff()
        {
           // int levelSkill = 5;
            int levelSkill = _levelViewController.SharedData.SaveData.skillsUpgradesData[3].level;
            float percentDamageSkill = levelSkill*0.01f;
            float percentDamage = 1.5f + percentDamageSkill;
            _levelViewController.ActivePlayerCharacters.ForEach(x=>x.ActiveBuff(2, 1.5f, percentDamage));
            await UniTask.Delay(TimeSpan.FromSeconds(_skill.timeAwait), cancellationToken: _cts.Token);
            _levelViewController.ActivePlayerCharacters.ForEach(x=>x.DisactiveBuff(2));
        }
     
    }
}
