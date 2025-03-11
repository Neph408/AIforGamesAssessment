using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_StartState : BehaviourStateTemplate
{

    public BH_StartState(AIFSM owner)
    {
        _aifsm = owner;
    }

    public override void OnEntry()
    {
        Debug.Log("Entered StartState");
    }
    public override void Execute()
    {
        Debug.Log("Exec go brr");
        if(_aifsm.GetOwnerAI()._agentData.FriendlyTeam == AgentData.Teams.RedTeam) // this can likely be compressed to a find objs with tag (ai.GetOwnerAI._ad.ft ? red : blue)
        {
            // whether to defend or attack
        }
        else
        {
            // same but if blue (literally only difference is communication)
        }
    }
    public override void OnExit()
    {
        Debug.Log("Exiting StartState");
    }
}
