using UnityEngine;

namespace Utilities
{
    public static class Globals
    {
        public static string PlayerProgressKey = "PlayerProgress";
        public static string SaveDataPath = Application.streamingAssetsPath+"SaveData.es3";
        
        
        #region Texts
        public static string PlayerTurn = "Player Turn";
        public static string EnemyTurn = "Enemy Turn";
        public static string PlayerWonHeader = "You Win!";
        public static string PlayerLose = "You Lose!";
        public static string TeamFullWarning = "Your team is full"; 
        public static string BattleTeamUnitRequirementHint = "Battle only available for 3 unit team";
        #endregion
        
        #region Configurations
        public static int MaxUnitInTeam = 3;
        public static int LevelExperienceLimit = 5;
        public static int HeroInventoryLimit = 10;
        public static float ShowUnitTooltipTime = 3f;
        public static int StarterUnitCount = 3;
        public static int RoundToGrantHeroToPlayer = 5;
        #endregion
    }
}
