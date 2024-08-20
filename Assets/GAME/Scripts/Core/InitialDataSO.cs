using UnityEngine;

namespace GAME.Scripts.Core {
    [CreateAssetMenu(fileName = "InitialDataSO", menuName = "GAME/InitialDataSO")]
    public class InitialDataSO : ScriptableObject {
        public SaveData initData;
    }
}
