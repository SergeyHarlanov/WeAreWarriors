using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FTRuntime;
using GAME.Scripts.Signals;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace GAME.Scripts.Skills.TypeSkills {
    public class ArrowSkillView : SkillViewBase {


        [SerializeField] private GameObject _prefabArrow;
        [SerializeField] private float _yValueSpawnArrow;
        [SerializeField] private int _countArrows;

        private List<GameObject> _arrows = new List<GameObject>();
        protected override void Fire()
        {
            UseBuff().Forget();
        }

        public override void Dispose()
        {
            foreach (var item in _arrows)
            {
                Destroy(item);
            }
            base.Dispose();
        }

        private async UniTaskVoid UseBuff()
        {
            for (int i = 0; i < _countArrows; i++)
            {
                
                float randX = Random.Range( _skillsController.LevelView.PlayerBase.transform.position.x
                    ,  _skillsController.LevelView.EnemyBase.transform.position.x);


                Transform arrowClone = Instantiate(_prefabArrow).transform;

                arrowClone.parent = transform;
                
                _arrows.Add(arrowClone.gameObject);

                arrowClone.position = new Vector3(randX, _yValueSpawnArrow);
                
                Tweener tweener = arrowClone.DOMoveY(-0.7f, 1f);

                float seconsDelay = Random.Range(0f, 2f);
                tweener.SetDelay(seconsDelay);
                tweener.OnComplete(() =>
                {
           
                    arrowClone.gameObject.SetActive(false);
          
                });

            }

            int skillId = _levelViewController.SharedData.SaveData.equippedSkillId;
            int levelSkill = _levelViewController.SharedData.SaveData.skillsUpgradesData[0].level;
          //  int levelSkill = 5;
            float awaitSkillAction = levelSkill*0.5f;
            
            _levelViewController.ActiveEnemyCharacters.ForEach(x=>x.ActiveBuff(0));
            await UniTask.Delay(TimeSpan.FromSeconds(_skill.timeAwait + awaitSkillAction), cancellationToken: _cts.Token);
            _levelViewController.ActiveEnemyCharacters.ForEach(x=>x.DisactiveBuff(0));
        }
    }
}
