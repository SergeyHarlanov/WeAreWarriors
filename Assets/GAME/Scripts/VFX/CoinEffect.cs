using DG.Tweening;
using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.VFX
{
    public class CoinEffect : VEffect
    {
        public override void Show(Vector3 startPosition, SignalBus signalBus, SharedData sharedData, TypeCoinEffect typeCoinEffect, Vector3 endPos)
        {
                
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.1f);
            transform.position = (startPosition);
            gameObject.SetActive(true);
             
     
            Vector2 randPosFromCharacter = Vector2.zero;
            
            switch (typeCoinEffect)
            {
                case TypeCoinEffect.Battle:
                {
                    float radius = 100;
                    randPosFromCharacter=
                        (Vector2)transform.position + new Vector2(Random.Range(-radius, radius),Random.Range(-radius, radius));
                    break;
                }
                case TypeCoinEffect.Menu:
                {
                    float radius = 250;
                    randPosFromCharacter =
                        new Vector2(Screen.width/2, Screen.height/2)  + new Vector2(Random.Range(-radius, radius),Random.Range(-radius, radius));
                    break;
                }
            }
            
          
                
            Sequence tweener = transform.DOJump(randPosFromCharacter , 0.2f, 10, 0.3f, false);
            tweener.OnComplete(() =>
            {
                Tweener tweener = transform.DOMove((endPos) , 0.2f);
                tweener.OnComplete(() =>
                {
                    Tweener tweener =  transform.DOScale(Vector3.zero , 0.1f);
                    tweener.OnComplete(() =>
                    {
                       
                      
                        switch (typeCoinEffect)
                        {
                            case TypeCoinEffect.Battle:
                            {
                                sharedData.LevelController.SetCoinsCollect(1);

                                break;
                            }
                            case TypeCoinEffect.Menu:
                            {

                                //_signalBus.Fire<MainScreenAddCoinSignal>(new MainScreenAddCoinSignal()
                              //     { coin = _sharedData.LevelController.AmountCoinsCollectedBattle });
                             //   _sharedData.LevelController.ResetCoinCollected();
                         //       _sharedData.SaveData.currentTimeLine.
                                //изменил теперь начисляет в VFXService
                           //    _signalBus.Fire<MainScreenAddCoinSignal>(new MainScreenAddCoinSignal(){coin = 1});
                                break;
                            }
                      
                        }
              
                    });
                    
                });
             
                    
            });
            base.Show(startPosition, signalBus, sharedData, typeCoinEffect, endPos);
        }
    }
}