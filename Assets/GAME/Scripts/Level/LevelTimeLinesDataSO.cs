using System.Collections.Generic;
using UnityEngine;

namespace GAME.Scripts.Level {
    [CreateAssetMenu(fileName = "LevelTimeLinesDataSO", menuName = "GAME/LevelTimeLinesDataSO")]
    public class LevelTimeLinesDataSO : ScriptableObject {
        public List<LevelTimeLine> timeLines;
    }
}
