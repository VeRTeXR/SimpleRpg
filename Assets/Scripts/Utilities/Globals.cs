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
        public static string TeamFullWarning = "Your team is full.";
        #endregion
        
        
        
        
        #region Configurations
        public static int MaxUnitInTeam = 3;
        public static int LevelExperienceLimit = 3;
        public static int HeroInventoryLimit = 10;
        public static double TriggerSelectionTime = 1f;
        public static int StarterUnitCount = 3;
        #endregion
    }
}
