using GAME.Scripts.Core;
using GAME.Scripts.Signals;
using YG;
using Zenject;

namespace GAME.Scripts.Ads {
    public struct AdReward {
        public RewardedAdPlacementType placementType;
        public int count;
    }
    
    public enum RewardedAdPlacementType {
        Revive = 0,
        Coins = 1,
        Energy = 2,
        Meat = 3,
    }

    public class AdsService {
        [Inject] private SignalBus _signalBus;
        [Inject] private SharedData _sharedData;
        [Inject] private SaveLoadService _saveLoadService;
        
        public void Init() {
            YandexGame.RewardVideoEvent += OnRewardedWatched;
      
          
            _signalBus.Subscribe<ShowAdsRewardSignal>(OnShow);
        }


        public void Dispose() {
            YandexGame.RewardVideoEvent -= OnRewardedWatched;
            _signalBus.Unsubscribe<ShowAdsRewardSignal>(OnShow);
        }
        
        
        public void ShowRewarded(AdReward reward) {
            string strData = $"{(int)reward.placementType}{reward.count.ToString()}"; 
            int id = int.Parse(strData);
            YandexGame.RewVideoShow(id);
        }

        public void ShowInterstitial() {
            YandexGame.FullscreenShow();
        }

        private void OnRewardedWatched(int data) {
            string strData = data.ToString();
            int id = int.Parse(strData[0].ToString());
            int count = 0;
            if (strData.Length > 1) count = int.Parse(strData.Substring(1));
            RewardedAdPlacementType placement = (RewardedAdPlacementType)id;
            switch (placement) {
                case RewardedAdPlacementType.Revive:
                    break;
                case RewardedAdPlacementType.Coins:
                    break;
                case RewardedAdPlacementType.Energy:
                    break;
                case RewardedAdPlacementType.Meat:
                    _signalBus.Fire(new MeatRewardAddSignal(){MeatAmount = 12});
                    break;
            }
        }

        private void OnShow(ShowAdsRewardSignal args)
        {
            ShowRewarded(args.adReward);
        }
    }
}
