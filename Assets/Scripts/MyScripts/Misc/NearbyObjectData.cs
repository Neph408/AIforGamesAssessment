using UnityEngine;

public class NearbyObjectData
{
    public bool exists = false;
    public GameObject targetGameObject = null;

    public NearbyObjectData()
    {
        exists = false;
        targetGameObject = null;
    }
    public NearbyObjectData(bool isExists, GameObject targetGameObject)
    {
        exists = isExists;
        this.targetGameObject = targetGameObject;
    }
    private string FormatToColourTag(Color col)
    {
        string hex = "#";
        hex += ((int)(col.r * 255)).ToString("X");
        hex += ((int)(col.g * 255)).ToString("X");
        hex += ((int)(col.b * 255)).ToString("X");
        return hex;

    }
    public string GetSlotAsString(bool GetColourInfo = false)
    {
        string colOpener = "";
        string colColour = "";
        string colCloser = "";

        if (GetColourInfo)
        {
            colOpener = "<color=*>";
            colColour = "#ffffff";
            colCloser = "</color>";
        }

        if (!exists)
        {
            return colOpener.Replace("*", colColour) + "Null" + colCloser;
        }

        if (GetColourInfo)
        {
            if (targetGameObject.CompareTag("Red Team") || targetGameObject.CompareTag("Blue Team"))
            {
                colColour = FormatToColourTag(targetGameObject.GetComponent<AI>().AICol);
            }
            else
            {
                colOpener = "";
                colColour = "";
                colCloser = "";
            }
        }
        return colOpener.Replace("*", colColour) + targetGameObject.name + colCloser;
    }



    public bool IsSlotEmpty()
    {
        if (targetGameObject == null) return true; // if it aint got a reference, there aint anything to talk about
        return false;
    }
}