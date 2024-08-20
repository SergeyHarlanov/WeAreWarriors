using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GAME.Scripts.Sounds {
    [CreateAssetMenu(fileName = "SoundsDataSO", menuName = "GAME/Sounds/SoundsDataSO")]
    public class SoundsDataSO : ScriptableObject {
        [SerializeField] private List<SoundData> soundsData;

        private Dictionary<SoundType, AssetReference> _soundsDataMap;
        
        public Dictionary<SoundType, AssetReference> SoundsDataMap {
            get {
                if (_soundsDataMap == null) {
                    _soundsDataMap = new();
                    foreach (SoundData curData in soundsData) {
                        _soundsDataMap.Add(curData.type, curData.soundLink);
                    }
                }

                return _soundsDataMap;
            }
        }
    }
    
    [Serializable]
    public class SoundData {
        public SoundType type;
        public AssetReference soundLink;
    }
}
