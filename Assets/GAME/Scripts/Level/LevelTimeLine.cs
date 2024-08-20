using System;
using System.Collections.Generic;
using GAME.Scripts.Sounds;
using UnityEngine.AddressableAssets;

namespace GAME.Scripts.Level {
    [Serializable]
    public class LevelTimeLine {
        public string name;
        public string localizationKey;
        public int cost;
        public List<LevelStage> stages;
    }

    [Serializable]
    public class LevelStage {
        public string name;
        public string localizationKey;
        public int cost;
        public StageBase stageBase;
        public GainResourcesData gainResourcesData;
        public AssetReference levelBackgroundPrefabLink;
        public AssetReference levelBaseBuildingPrefabLink;
        public AssetReference stageUiSpriteLink;
        public float difficultyIncrease;
        public List<int> characters;
        public SpawnData spawnData;
        public SoundType soundType;
    }

    [Serializable]
    public class StageBase {
        public float basePlayerHealth;
        public float baseEnemyHealth;
        public float playerHealthPerLevel;
        public int baseUpgradeCost;
    }

    [Serializable]
    public class GainResourcesData {
        public float gainResourceBaseSpeed;
        public float gainResourcePerUpgradeIncrease;
        public float gainResourceBaseUpgradeCost;
        public float gainResourceBaseTime;
        public float gainResourceBaseTimePerLevel;
        public float minGainResourceTime;
        public int gainStartResources;
        public float cheatGainResourceTime;
    }

    [Serializable]
    public class SpawnData {
        public List<SpawnWave> spawnWaves;
    }

    [Serializable]
    public class SpawnWave {
        public int count;
        public int id;
        public float time;
    }
}
