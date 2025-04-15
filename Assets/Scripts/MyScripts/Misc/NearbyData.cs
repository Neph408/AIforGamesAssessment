public class NearbyData
{
    public NearbyObjectData Collectable;
    public NearbyObjectData[] Enemy;
    public NearbyObjectData[] Ally;
    public NearbyObjectData[] Flag;
    public NearbyObjectData Base;

    public int nearbyEnemyCount;
    public int nearbyAllyCount;
    public int nearbyFlagCount;

    public int nearbyAllyHoldingFlag;
    public int nearbyEnemyHoldingFlag;

    public NearbyData()
    {
        Collectable = new NearbyObjectData();//big enough for the possibility to never happen of too many objs on floor
        Enemy = new NearbyObjectData[3];
        Ally = new NearbyObjectData[2];
        Flag = new NearbyObjectData[2];
        Base = new NearbyObjectData();
        nearbyAllyCount = 0;
        nearbyFlagCount = 0;
        nearbyEnemyCount = 0;
        nearbyAllyHoldingFlag = 0;
        nearbyEnemyHoldingFlag = 0;
    }



    public string GetDataAsString(bool oneLine = false)
    {
        string newString = "";

        newString += "Collectable : " + Collectable.GetSlotAsString() + (oneLine ? " | " : "\n");
        newString += "Enemy : ";
        for (int i = 0; i < Enemy.Length; i++)
        {
            if (i == Enemy.Length - 1)
            {
                newString += Enemy[i].GetSlotAsString(true) + (oneLine ? " | " : "\n");
            }
            else
            {
                newString += Enemy[i].GetSlotAsString(true) + ", ";
            }
        }
        newString += "Ally : ";
        for (int i = 0; i < Ally.Length; i++)
        {
            if (i == Ally.Length - 1)
            {
                newString += Ally[i].GetSlotAsString(true) + (oneLine ? " | " : "\n");
            }
            else
            {
                newString += Ally[i].GetSlotAsString(true) + ", ";
            }
        }
        newString += "Flag : ";
        for (int i = 0; i < Flag.Length; i++)
        {
            if (i == Flag.Length - 1)
            {
                newString += Flag[i].GetSlotAsString() + (oneLine ? " | " : "\n");
            }
            else
            {
                newString += Flag[i].GetSlotAsString() + ", ";
            }
        }
        newString += "Base : " + Base.GetSlotAsString();
        return newString;
    }

    public void ClearData()
    {
        Collectable = new NearbyObjectData();
        for (int i = 0; i < Ally.Length; i++)
        {
            Ally[i] = new NearbyObjectData();
        }
        for (int i = 0; i < Enemy.Length; i++)
        {
            Enemy[i] = new NearbyObjectData();
        }
        for (int i = 0; i < Flag.Length; i++)
        {
            Flag[i] = new NearbyObjectData();
        }
        Base = new NearbyObjectData();
        nearbyAllyCount = 0;
        nearbyFlagCount = 0;
        nearbyEnemyCount = 0;
        nearbyAllyHoldingFlag = 0;
        nearbyEnemyHoldingFlag = 0;
    }
}