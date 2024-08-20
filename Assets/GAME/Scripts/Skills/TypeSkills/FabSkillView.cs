using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.Skills.TypeSkills {
    public class FabSkillView : SkillViewBase {
        protected override void Fire()
        {
            UseBuff();
        }
        private void UseBuff()
        {
            Action fabAction = () =>
            {
                _levelViewController.ActiveEnemyCharacters.ForEach(x =>
                    x.SetDamage(GetDamageFromAllEnemies()));
            };
            _levelViewController.ActiveEnemyCharacters.ForEach(x=>x.ActiveBuff(1,0, 0,  0, fabAction));
       
        }
        private int GetDamageFromAllEnemies()
        {
            float damageToAllHeroes = _levelViewController.ActivePlayerCharacters
                .Sum(x => x.GetDamage);

            int levelSkill = _levelViewController.SharedData.SaveData.skillsUpgradesData[1].level;
          //  int levelSkill = 5;
            float percentDamageSkill = levelSkill*1f;
            float percent = 5 + percentDamageSkill;
            double allDamagePercent = System.Math.Round((double)(damageToAllHeroes/ percent));
            
            return (int)allDamagePercent;
        }

       
    }
}
