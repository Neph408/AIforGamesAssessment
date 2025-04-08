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
public static class Movement
{
    public const float Leniency = 0.15f; // how close "close enough" is

    // Collectable Pickup Chances
    public const float DefenderInitialCollectChance = 0.75f;
    public const float DefenderRepeatCollectChance = 0.5f;
    public const bool DefenderDoesOwnershipReduceCollectChance = true;// if false, the collection chance will always be DefenderRepeatCollectChance. If true, the collection chance will be DefenderRepeatCollectChance^HeldCount
    public const float DefenderPickupRangeRestriction = 10f;

    //update these
    public const float AttackerInitialCollectChance = 0.75f;
    public const float AttackerRepeatCollectChance = 0.5f;
    public const bool AttackerDoesOwnershipReduceCollectChance = true;
    public const float AttackerPickupRangeRestriction = 25f;

    public const float RetrieverInitialCollectChance = 0.75f;
    public const float RetrieverRepeatCollectChance = 0.5f;
    public const bool RetrieverDoesOwnershipReduceCollectChance = true;
    public const float RetrieverPickupRangeRestriction = 5f;

    public const float ProtectorInitialCollectChance = 0.75f;
    public const float ProtectorRepeatCollectChance = 0.5f;
    public const bool ProtectorDoesOwnershipReduceCollectChance = true;
    public const float ProtectorPickupRangeRestriction = 10f;

    // Enemy Engagment Rates

    public const float DefenderEngagmentChance = 1f;
    public const float AttackerEngagmentChance = 1f;
    public const float ProtectorEngagmentChance = 1f;
    public const float RetrieverEngagmentChance = 0.1f;

    public const bool ProtectorDoesRetrieverThreatenedOverrideTarget = true;
    public const bool ProtectorDoesRetrieverThreatenedOverrideActions = true;

    public const float RetrieverIgnoreAllDistance = 20f;
    public const bool RetrieverDoesProximityToBaseDisableEngagments = true;

    // Heal Health Triggers

    public const float DefenderHealthToHeal = 30f;
    public const float AttackerHealthToHeal = 20f;
    public const float ProtectorHealthToHeal = 40f;
    public const float RetrieverHealthToHeal = 50f;

    // Damage Boost Health Triggers - probably ignore these it may get mroe complicated, or become chance based

    public const float DefenderHealthToBoost = 60f;
    public const float AttackerHealthToBoost = 20f;
    public const float ProtectorHealthToBoost = 40f;
    public const float RetrieverHealthToBoost = 50f;

    // Ignore duration
    public const float IgnoreDuration = 8f;
    public const float RangeCapIgnoreDuration = 1.5f;
}