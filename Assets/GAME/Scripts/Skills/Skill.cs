using System;
using UnityEngine.AddressableAssets;

namespace GAME.Scripts.Skills {
    public enum SkillType {
        Arrows = 0,
        Fab = 1,
        Smoke = 2,
        Potion = 3,
    }

    [System.Serializable]
    public class UpgradeSkillValue
    {
        public float timeStunn = 0.5f;
        public float percentToDamage = 1;
        public float percentToSlowDown = 1;
    }
    [Serializable]
    public class Skill {
        public string name;
        public int id;
        public string localizationKey;
        public string upgradeDescription;
        public SkillType type;
        public AssetReference uiSpriteLink;
        public float baseEffectValue;
        public float perUpgradeEffectValue;
        public int upgradeCost;
        public int countOfUsagePerBattle;
        public int openTimeLineId;
        public int openStageId;
        public AssetReference prefabLink;
        public float timeAwait;
        public UpgradeSkillValue upgradeSkillValue;
        public int numberOfUses = 1;
    }
}
