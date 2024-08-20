using GAME.Scripts.VFX;
using UnityEngine;

namespace GAME.Scripts.Signals {
    public class ShowVFXSignal {
        public VFXType EffectType { get; set; }
        public Vector3 EffectPosition { get; set; }
    }
}
