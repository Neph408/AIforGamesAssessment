using UnityEngine;

public class IgnoredObjectData
{
    public GameObject gameObject;
    public float timestampOfAcknowledge;

    public IgnoredObjectData(GameObject _gameObject, float _timestampOfAcknowledge)
    {
        gameObject = _gameObject;
        timestampOfAcknowledge = _timestampOfAcknowledge;
    }

    public string GetObjName()
    {
        return gameObject.name;
    }

    public float GetTimeUntilRemember()
    {
        return ((int)((timestampOfAcknowledge - Time.time) * 100f)) / 100f;
    }

}