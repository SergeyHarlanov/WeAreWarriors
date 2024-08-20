using System;
using System.Collections.Generic;
using GAME.Scripts.Characters;
using GAME.Scripts.Level;
using GAME.Scripts.Skills;
using UnityEngine;
using Zenject;

namespace GAME.Scripts.Core {
    public class SharedData {
        [Inject] private CharactersDataSO _charactersDataSO;
        [Inject] private LevelTimeLinesDataSO _levelTimeLinesDataSO;
        [Inject] private SkillsDataSO _skillsDataSO;
        
        
  
        public SaveData SaveData { get; set; }
        public Transform SoundsParent { get; set; }
        public LevelViewController LevelController { get; set; }
        public bool CheatMeatProduction { get; set; }

        public bool CanBuyCharacter {
            get {
                List<int> curCharactersIds = _levelTimeLinesDataSO.timeLines[SaveData.currentTimeLine.timeLineId]
                    .stages[SaveData.currentTimeLine.playerStageId].characters;
                for (int i = 0; i < curCharactersIds.Count; i++) {
                    if (!SaveData.currentTimeLine.openedCharacters.Contains(i)) {
                        Character curChar = _charactersDataSO.CharactersData[curCharactersIds[i]];
                        bool isEnoughMoney = curChar.unlockCost <= SaveData.coins;
                        if (isEnoughMoney) return true;
                    }
                }
                
                return false;
            }
        }

        public bool CanEvaluate {
            get {
                LevelTimeLineSaveData curTimeLineSaveData = SaveData.currentTimeLine;
                LevelTimeLine curTimeLine = _levelTimeLinesDataSO.timeLines[curTimeLineSaveData.timeLineId];
                bool hasNextStage = curTimeLineSaveData.playerStageId < curTimeLine.stages.Count - 1;
                if (!hasNextStage) return false;
                LevelStage nextStage = hasNextStage ? curTimeLine.stages[curTimeLineSaveData.playerStageId + 1] : null;
                int evaluateCost = nextStage.cost;
                bool isEnoughMoney = evaluateCost <= SaveData.coins;
                return isEnoughMoney;
            }
        }

        public int FoodGenerationUpgradeCost {
            get {
                LevelStage curStage = _levelTimeLinesDataSO.timeLines[SaveData.currentTimeLine.timeLineId]
                    .stages[SaveData.currentTimeLine.playerStageId];
                int curLevel = SaveData.currentTimeLine.gainResourceUpgradeLevel;
                int nextLevel = curLevel + 1;
                if (curLevel == 0) return (int)curStage.gainResourcesData.gainResourceBaseUpgradeCost;
                return (int)MathF.Exp(1.94f * nextLevel + 1.64f);
            }
        }
        
        public int BaseHealthUpgradeCost {
            get {
                LevelStage curStage = _levelTimeLinesDataSO.timeLines[SaveData.currentTimeLine.timeLineId]
                    .stages[SaveData.currentTimeLine.playerStageId];
                int curLevel = SaveData.currentTimeLine.stageBaseLevel;
                int nextLevel = curLevel + 1;
                if (curLevel == 0) return curStage.stageBase.baseUpgradeCost;
                return (int)MathF.Exp(1.94f * nextLevel + 1.64f);
            }
        }

        public LevelStage CurStage
        {
            get
            {
                LevelStage curStage = _levelTimeLinesDataSO.timeLines[SaveData.currentTimeLine.timeLineId]
                    .stages[SaveData.currentTimeLine.playerStageId];
                return curStage;
            }
        }
        public bool CanUpgradeFoodGeneration => FoodGenerationUpgradeCost <= SaveData.coins;
        public bool CanUpgradeBaseHealth => BaseHealthUpgradeCost <= SaveData.coins;

        public bool CanUpgradeSkill {
            get {
                bool hasOpenedSkill = SaveData.skillsUpgradesData.Count > 0;
                if (!hasOpenedSkill) return false;

                foreach (SkillUpgradeSaveData upgradeData in SaveData.skillsUpgradesData) {
                    Skill skillInfo = _skillsDataSO.skills[upgradeData.id];
                    bool canBuy = skillInfo.upgradeCost <= SaveData.crystals;
                    if (canBuy) return true;
                }

                return false;
            }
        }
    }
}
