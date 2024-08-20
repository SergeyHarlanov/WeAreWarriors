using System;
using UnityEngine.AddressableAssets;

namespace GAME.Scripts.VFX {
    [Serializable]
    public class VFXResourceLink {
        public VFXType type;
        public AssetReference assetLink;
    }
}
