using System.Collections.Generic;
using UnityEngine;

namespace GAME.Scripts.Skills {
    [CreateAssetMenu(fileName = "SkillsDataSO", menuName = "GAME/SkillsDataSO")]
    public class SkillsDataSO : ScriptableObject {
        public List<Skill> skills;
    }
}
