using System;
using GAME.Scripts.Sounds;
using UnityEngine.AddressableAssets;

namespace GAME.Scripts.Characters {
    [Serializable]
    public class Character {
        public string name;
        public string keyNameLocalization;
        public int id;
        public int spawnCost;
        public int unlockCost;
        public float speed;
        public float attack;
        public float buildingAttack;
        public float health;
        public int drop;
        public int dropHitBase;
        public float attackRate;
        public float attackDistance;
        public CharacterAttackType attackType;
        public AssetReference uiSpriteReference;
        public AssetReference viewPrefabReference;
        public AssetReference bulletPrefabReference;
        public float bulletJumpPower;
        public int playerWalkAnimationId;
        public int playerAttackAnimationId;
        public int enemyWalkAnimationId;
        public int enemyAttackAnimationId;
        public int attackFrame;
        public SoundType hitSoundTypePlayer = SoundType.HitBow;
        public SoundType deathSoundTypePlayer = SoundType.DeathBird;
    }

    public enum CharacterAttackType {
        None = 0,
        Melee = 1,
        Ranged = 2,
    }
}
