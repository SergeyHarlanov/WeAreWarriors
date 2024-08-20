using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAME.Scripts.Core {
    [Serializable]
    public class SaveData {
        public bool music;
        public bool sounds;
        public int coins;
        public int crystals;
        public LevelTimeLineSaveData currentTimeLine;
        public List<LevelTimeLineSaveData> openedTimeLines;
        public List<CharacterUpgradeSaveData> characterUpgradesData;
        public List<SkillUpgradeSaveData> skillsUpgradesData;
        public int equippedSkillId;
       

        public void DeepClone(SaveData data) {
            music = data.music;
            sounds = data.sounds;
            coins = data.coins;
            crystals = data.crystals;
            
            currentTimeLine = new();
            currentTimeLine.DeepClone(data.currentTimeLine);
            
            openedTimeLines = new();
            foreach (LevelTimeLineSaveData curOpenedTimeLine in data.openedTimeLines) {
                LevelTimeLineSaveData newTimeLine = new();
                newTimeLine.DeepClone(curOpenedTimeLine);
                openedTimeLines.Add(newTimeLine);
            }

            characterUpgradesData = new();
            foreach (CharacterUpgradeSaveData curCharData in data.characterUpgradesData) {
                CharacterUpgradeSaveData charData = new();
                charData.DeepClone(curCharData);
                characterUpgradesData.Add(charData);
            }

            skillsUpgradesData = new();
            Debug.Log(data.skillsUpgradesData.Count+"::::");
            foreach (SkillUpgradeSaveData curSkillData in data.skillsUpgradesData) {
                SkillUpgradeSaveData skillData = new();
                skillData.DeepClone(curSkillData);
                skillsUpgradesData.Add(skillData);
            }

            equippedSkillId = data.equippedSkillId;
        }
    }

    [Serializable]
    public class LevelTimeLineSaveData {
        public int timeLineId;
        public int playerStageId;
        public int enemyStageId;
        public List<int> openedCharacters;
        public int stageBaseLevel;
        public int gainResourceUpgradeLevel;

        public void DeepClone(LevelTimeLineSaveData data) {
            timeLineId = data.timeLineId;
            playerStageId = data.playerStageId;
            enemyStageId = data.enemyStageId;
            openedCharacters = new();
            openedCharacters.AddRange(data.openedCharacters);
            stageBaseLevel = data.stageBaseLevel;
            gainResourceUpgradeLevel = data.gainResourceUpgradeLevel;
        }
    }

    [Serializable]
    public class CharacterUpgradeSaveData {
        public int id;
        public int level;

        public void DeepClone(CharacterUpgradeSaveData data) {
            id = data.id;
            level = data.level;
        }
    }

    [Serializable]
    public class SkillUpgradeSaveData {
        public int id;
        public int level;

        public void DeepClone(SkillUpgradeSaveData data) {
            id = data.id;
            level = data.level;
        }
    }
}
