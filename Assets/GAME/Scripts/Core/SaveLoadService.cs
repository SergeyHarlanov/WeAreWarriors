using UnityEngine;
using YG;
using Zenject;

namespace GAME.Scripts.Core {
    public class SaveLoadService {
        [Inject] private SharedData _sharedData;
        [Inject] private InitialDataSO _initialSaveData;
        
        public void SaveData() {
            string curDataString = JsonUtility.ToJson(_sharedData.SaveData);
            YandexGame.savesData.saves = curDataString;
            YandexGame.SaveProgress();
        }

        public void LoadData() {
            if (string.IsNullOrEmpty(YandexGame.savesData.saves)) {
                SaveData curData = new();
                curData.DeepClone(_initialSaveData.initData);
                _sharedData.SaveData = curData;
                string saveDataString = JsonUtility.ToJson(curData);
                YandexGame.savesData.saves = saveDataString;
                YandexGame.SaveProgress();
            }
            else {
                SaveData curData = JsonUtility.FromJson<SaveData>(YandexGame.savesData.saves);
                _sharedData.SaveData = curData;
            }
        }
    }
}
