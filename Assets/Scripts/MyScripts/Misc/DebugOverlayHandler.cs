using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugOverlayHandler : MonoBehaviour
{
    public static DebugOverlayHandler DOH;
    [SerializeField] private Text BTM1;
    [SerializeField] private Text BTM2;
    [SerializeField] private Text BTM3;
    [SerializeField] private Text RTM1;
    [SerializeField] private Text RTM2;
    [SerializeField] private Text RTM3;
    [Space(10f)]
    [SerializeField] private bool DefaultDisplay = false;

    private bool IsDisplay;

    private void Awake()
    {
        if (DOH == null)
        {
            DOH = this;
        }
        IsDisplay = DefaultDisplay;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Slash))
        {
            IsDisplay = !IsDisplay;
        }
        RTM1.enabled = IsDisplay;
        RTM2.enabled = IsDisplay;
        RTM3.enabled = IsDisplay;
        BTM1.enabled = IsDisplay;
        BTM2.enabled = IsDisplay;
        BTM3.enabled = IsDisplay;
    }

    public void SetSlotText(string Slot, string DisplayText, Color DisplayCol)
    {
        if(!IsDisplay)
        {
            return; 
        }
        switch (Slot)
        {
            case "Blue Team Member 1":
                BTM1.text = DisplayText;
                BTM1.color = DisplayCol;
                return;
            case "Blue Team Member 2":
                BTM2.text = DisplayText;
                BTM2.color = DisplayCol;
                return;
            case "Blue Team Member 3":
                BTM3.text = DisplayText;
                BTM3.color = DisplayCol;
                return;
            case "Red Team Member 1":
                RTM1.text = DisplayText;
                RTM1.color = DisplayCol;
                return;
            case "Red Team Member 2":
                RTM2.text = DisplayText;
                RTM2.color = DisplayCol;
                return;
            case "Red Team Member 3":
                RTM3.text = DisplayText;
                RTM3.color = DisplayCol;
                return;
            default:
                Debug.LogError("Invalid Name in SetSlotText");
                return;
        }
    }
    public void SetSlotText(string Slot, string DisplayText)
    {
        if(!IsDisplay)
        {
            return; 
        }
        switch (Slot)
        {
            case "Blue Team Member 1":
                BTM1.text = DisplayText;
                return;
            case "Blue Team Member 2":
                BTM2.text = DisplayText;
                return;
            case "Blue Team Member 3":
                BTM3.text = DisplayText;
                return;
            case "Red Team Member 1":
                RTM1.text = DisplayText;
                return;
            case "Red Team Member 2":
                RTM2.text = DisplayText;
                return;
            case "Red Team Member 3":
                RTM3.text = DisplayText;
                return;
            default:
                Debug.LogError("Invalid Name in SetSlotText");
                return;
        }
    }
}
