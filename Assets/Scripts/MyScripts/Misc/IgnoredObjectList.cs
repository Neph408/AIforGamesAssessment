using System.Collections.Generic;
using UnityEngine;

public class IgnoredObjectList
{
    public List<IgnoredObjectData> IgnoredCollectables;

    public IgnoredObjectList()
    {
        IgnoredCollectables = new List<IgnoredObjectData>();
    }
    public int Count
    {
        get { return IgnoredCollectables.Count; }
    }



    public string GetDataAsString()
    {
        string NewString = "";

        if (Count == 0)
        {
            return "None";
        }

        for (int i = 0; i < Count; i++)
        {
            NewString += IgnoredCollectables[i].GetObjName() + " - " + IgnoredCollectables[i].GetTimeUntilRemember().ToString() + (i == Count - 1 ? "" : " | ");
        }
        return NewString;
    }

    public void Add(GameObject target, float duration)
    {
        IgnoredCollectables.Add(new IgnoredObjectData(target, Time.time + duration));
    }
    public void Remember(int target)
    {
        IgnoredCollectables.RemoveAt(target);
    }
    public bool Contains(GameObject go)
    {
        if (IgnoredCollectables.Count == 0)
        {
            return false;
        }
        foreach (var ignoredObject in IgnoredCollectables)
        {
            if (ignoredObject.gameObject == go)
            {
                return true;
            }
        }
        return false;
    }
    public void Update()
    {
        if (IgnoredCollectables.Count > 0)
        {
            for (int i = 0; i < IgnoredCollectables.Count; i++)
            {
                if (IgnoredCollectables[i].gameObject == null)
                {
                    IgnoredCollectables.RemoveAt(i);
                }
                if (Time.time > IgnoredCollectables[i].timestampOfAcknowledge)
                {
                    Remember(i);
                }
            }
        }
    }
}