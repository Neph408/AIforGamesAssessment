/// <summary>
/// The names used to identify important entities in the game
/// Names refer to individual GameObjects
/// </summary>
public static class Names
{
    public const string PowerUp = "Power Up";
    public const string HealthKit = "Health Kit";

    public const string BlueFlag = "Blue Flag";
    public const string RedFlag = "Red Flag";

    public const string BlueBase = "Blue Base";
    public const string RedBase = "Red Base";

    public const string BlueTeamMember1 = "Blue Team Member 1";
    public const string BlueTeamMember2 = "Blue Team Member 2";
    public const string BlueTeamMember3 = "Blue Team Member 3";

    public const string RedTeamMember1 = "Red Team Member 1";
    public const string RedTeamMember2 = "Red Team Member 2";
    public const string RedTeamMember3 = "Red Team Member 3";
}

/// <summary>
/// The tags used to identify important entity groups in the game
/// Tags refer to groups of related objects
/// </summary>
public static class Tags
{
    public const string BlueTeam = "Blue Team";
    public const string RedTeam = "Red Team";
    public const string Collectable = "Collectable";
    public const string Flag = "Flag";
    public const string Base = "Base";
}


// Fixed values for AI update frequency
public static class DetectionUpdateFrequency
{
    public const float DefenderPatrol = 0.25f;
    public const float DefenderAttack = 0.1f;
    public const float DefenderCollect = 0.2f;

    public const float AttackerRoam = 0.125f;
    public const float AttackerAttack = 0.1f;
    public const float AttackerCollect = 0.2f;

    public const float OverrideRoleRoam = 0.1f;
    public const float OverrideRoleAttack = 0.1f;
    public const float OverrideRoleCollect = 0.1f;
}

// Other Misc values for movement related stuff
public static class AIConstants
{
    public class Global
    {
        // Distance leniency
        public const float Leniency = 0.15f; // how close "close enough" is to a target
        
        // Ignore duration
        public const float IgnoreDuration = 8f;
        public const float RangeCapIgnoreDuration = 1.5f;
    }
    
    public class Defender
    {
        // Collectable Pickup Chances
        public const float InitialCollectChance = 0.75f; // Chance to pick up collectable if ai holds none of same type
        public const float RepeatCollectChance = 0.5f; // Chance to pick up collectable if ai holds any of same type
        public const bool OwnershipReducesCollectChance = true;// if false, the collection chance will always be DefenderRepeatCollectChance. If true, the collection chance will be DefenderRepeatCollectChance^HeldCount
        public const float PickupRangeRestriction = 10f; // Ai will ignore any collectables outside of this distance

        // Enemy Engagment Rates
        public const float EngagmentChance = 1f;

        // Heal Health Triggers
        public const float HealthToHeal = 30f;

        // Damage Boost Health Triggers - probably ignore these it may get mroe complicated, or become chance based
        public const float HealthToBoost = 60f;

        // Startup Ignore
        public const float StartupIgnoreCollectableDuration = 2f;

    }
    public class Attacker
    {
        // Collectable Pickup Chances
        public const float InitialCollectChance = 0.75f;
        public const float RepeatCollectChance = 0.5f;
        public const bool OwnershipReducesCollectChance = true;
        public const float PickupRangeRestriction = 25f;

        // Enemy Engagment Rates
        public const float EngagmentChance = 1f;

        // Heal Health Triggers
        public const float HealthToHeal = 20f;

        // Damage Boost Health Triggers - probably ignore these it may get mroe complicated, or become chance based
        public const float HealthToBoost = 20f;

    }
    public class Retriever
    {
        // Collectable Pickup Chances
        public const float InitialCollectChance = 0.75f;
        public const float RepeatCollectChance = 0.5f;
        public const bool OwnershipReducesCollectChance = true;
        public const float PickupRangeRestriction = 5f;

        // Enemy Engagment Rates
        public const float EngagmentChance = 0.1f;
        public const float IgnoreAllDistance = 20f;
        public const bool ProximityToBaseDisablesEngagments = true;

        // Heal Health Triggers
        public const float HealthToHeal = 50f;

        // Damage Boost Health Triggers - probably ignore these it may get mroe complicated, or become chance based
        public const float HealthToBoost = 50f;

    }
    public class Protector
    {
        // Collectable Pickup Chances
        public const float InitialCollectChance = 0.75f;
        public const float RepeatCollectChance = 0.5f;
        public const bool DoesOwnershipReduceCollectChance = true;
        public const float PickupRangeRestriction = 10f;

        // Enemy Engagment Rates
        public const float EngagmentChance = 1f;
        public const bool RetrieverThreatenedOverridesTarget = true;
        public const bool RetrieverThreatenedOverridesActions = true;

        // Heal Health Triggers
        public const float HealthToHeal = 40f;

        // Damage Boost Health Triggers - probably ignore these it may get mroe complicated, or become chance based
        public const float HealthToBoost = 40f;

    }
}