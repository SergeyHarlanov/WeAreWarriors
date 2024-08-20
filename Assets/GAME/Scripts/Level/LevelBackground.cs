using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GAME.Scripts.Signals;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace GAME.Scripts.Level {
   public class LevelBackground : MonoBehaviour {
        [SerializeField] private SpriteRenderer cloudPrefab;
        [SerializeField] private Sprite[] cloudsSprite;
        
        [SerializeField] private Vector3 _scaleEchelon;
        [SerializeField] private float _spaceBetweenEchelon;
        [SerializeField] private float _paddingTop;
        
        [SerializeField] private int _startRandomClouds = 3, _endRandomClouds = 15;

        [SerializeField] private float _speedClouds;
        [SerializeField] private bool _isGismozShowZone = false;

       [SerializeField] private List<Transform> _cloudClones = new ();
        private Tweener[] TweenersClouds;
        private Vector2[] _posCkloudsBeforeSkill;
        
        private bool _left;
        private CancellationTokenSource _cts;
        private bool _isHideClouds;
        
        
        
        private void OnDrawGizmos() {
            if (_isGismozShowZone)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 padding = new Vector3(0, _paddingTop, 0);
                    Gizmos.DrawCube(padding + transform.position + new Vector3(0, _spaceBetweenEchelon * i, 0),
                        _scaleEchelon);
                    Gizmos.DrawCube(padding + transform.position + new Vector3(0, _spaceBetweenEchelon * i, 0),
                        _scaleEchelon);
                    Gizmos.DrawCube(padding + transform.position + new Vector3(0, _spaceBetweenEchelon * i, 0),
                        _scaleEchelon);
                }
            }
        }

        public void Init(SignalBus signalBus) {
 
            
            _cts = new();
            
            int dir = Random.Range(0, 2);
            _left = dir == 0;
            
            int randClouds = Random.Range(_startRandomClouds, _endRandomClouds);
            
            TweenersClouds = new Tweener[randClouds];
            _posCkloudsBeforeSkill = new Vector2[randClouds];
            
            for (int i = 0; i < randClouds; i++) {
                Transform tr = Instantiate(cloudPrefab).transform;
                tr.parent = transform;
                transform.localScale = Vector3.one;
                
                _cloudClones.Add(tr);

                float percent = ((float)100f/randClouds) / 100f;
                percent *= i + 1;
                MoveCloud(i, Mathf.Lerp(-_scaleEchelon.x / 2f, _scaleEchelon.x / 2, percent)).Forget();
            }
            
            Debug.Log("CLOUDS START");
            signalBus.Subscribe<UseSkillSignal>(HideClouds);
            signalBus.Subscribe<EndSkillSignal>(ShowClouds);
        }
        
        private async UniTask MoveCloud(int index, float randX) {
            while (true) {
                if (!this) return;

                TweenersClouds[index].Pause();
                int randLine = Random.Range(0, 3);
            
                RandSprite(index, randLine);
                Vector3 padding = new Vector3(0, _paddingTop, 0);
                Vector3 startPos = padding +  transform.position + new Vector3(0, _spaceBetweenEchelon * randLine, 0);
               
                _cloudClones[index].position = startPos + (_left ? _scaleEchelon / 2 : -_scaleEchelon / 2) ;
                float randY =  Random.Range(-_scaleEchelon.y / 2f, _scaleEchelon.y / 2f);
                _cloudClones[index].position = new Vector3(randX, startPos.y + randY, _cloudClones[index].position.z);
             
            
                float time = _speedClouds * (3 - randLine);
                
                TweenersClouds[index].Play();
                TweenersClouds[index] = _cloudClones[index].DOMoveX(_left ? -_scaleEchelon.x / 2 : _scaleEchelon.x / 2, time);
              

                await UniTask.WaitUntil(() => !TweenersClouds[index].active, cancellationToken: _cts.Token);
                
                randX = !_left ? -_scaleEchelon.x / 2 : _scaleEchelon.x / 2;
            }
        }
        
        private async UniTask MoveCloudFromSkill(int index) {
            _cloudClones[index].DOMoveX( _posCkloudsBeforeSkill[index].x, 0.7f).SetEase(Ease.InBack);
         
            await UniTask.Delay(TimeSpan.FromSeconds(0.7f), cancellationToken: _cts.Token);

            TweenersClouds[index].Play();
        }

        private void RandSprite(int indexEchelone,int indexLine) {
            if (!_cloudClones[indexEchelone]) return;

            _cloudClones[indexEchelone].name = "Clouds" + indexLine;
            
            int randClouds = indexLine switch {
                0 => Random.Range(3, 9),
                1 => Random.Range(0, 9),
                2 => Random.Range(0, 6),
                _ => 0, 
            };

            _cloudClones[indexEchelone].GetComponent<SpriteRenderer>().sprite = cloudsSprite[randClouds];
        }

        private void HideClouds() {
          if (_isHideClouds) return;
          _isHideClouds = true;

          _posCkloudsBeforeSkill = new Vector2[_cloudClones.Count];
            for (int i = 0; i < _cloudClones.Count; i++) {
                int dir = Random.Range(0, 2);

                TweenersClouds[i].Pause();

                if (_cloudClones[i])
                {
                    _posCkloudsBeforeSkill[i] = _cloudClones[i].position;
                }
              
                _cloudClones[i].DOMoveX((dir == 0 ? -_scaleEchelon.x / 2 : _scaleEchelon.x / 2), 0.7f).SetEase(Ease.InBack);
            }
        }

        private  void ShowClouds() {
            if (!_isHideClouds) return;
            _isHideClouds = false;
            
            for (int i = 0; i < TweenersClouds.Length; i++) {
               MoveCloudFromSkill(i).Forget();
            }
        }

        public void Dispose() {
          //  _cts.Cancel();
            //_cts = null;
            _cloudClones.ForEach(x=>Destroy(x.gameObject));            
        }
    }
}
