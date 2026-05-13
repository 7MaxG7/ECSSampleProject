namespace Infrastructure
{
    public class Constants
    {
        // Ecs
        public const string INFRASTRUCTURE_SYSTEMS_KEY = "Infrastructure";
        public const string FIXED_SYSTEMS_KEY_POSTFIX = "_fixed";
        
        // Scenes
        public const string BOOTSTRAP_SCENE_NAME = "BootstrapScene";
        public const string BATTLE_SCENE_NAME = "BattleScene";

        // Static data
        public const string UNIT_DATA_PATH = "Units";
        
        // Roots
        public const string UNITS_PARENT_NAME = "Units";
        
        // Math
        public const float ALMOST_ONE = 1 - float.Epsilon;
        
        // UI
        public const string WIN_END_BATTLE_LABLE = "Win!";
        public const string DEFEAT_END_BATTLE_LABLE = "Defeat";
    }
}