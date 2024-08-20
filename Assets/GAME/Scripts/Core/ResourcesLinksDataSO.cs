using System.Collections.Generic;
using System.Collections.ObjectModel;
using GAME.Scripts.VFX;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GAME.Scripts.Core {
    [CreateAssetMenu(fileName = "ResourcesLinksDataSO", menuName = "GAME/Resources/ResourcesLinksDataSO")]
    public class ResourcesLinksDataSO : ScriptableObject {
        [SerializeField] private List<AssetReference> _levelsLinks;
        [SerializeField] private List<VFXResourceLink> _effectsData;
        
        
        private Dictionary<VFXType, AssetReference> _effectsLinks;
        

        public ReadOnlyCollection<AssetReference> LevelsLinks => _levelsLinks.AsReadOnly();
        public Dictionary<VFXType, AssetReference> EffectsLinks {
            get {
               
        
         //       Debug.Log(_effectsData.Count+":Count");
                   
                if (_effectsLinks == null) {
                    _effectsLinks = new();
                    foreach (VFXResourceLink resourceData in _effectsData) {
                        _effectsLinks.Add(resourceData.type, resourceData.assetLink);
                    }
                }
            //    _effectsLinks = new();
              //  _effectsLinks.Add(_effectsData[0].type, _effectsData[0].assetLink);
        
                return _effectsLinks;
            }
        }

    }
}
