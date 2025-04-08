using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.MPE;
using UnityEngine;

public class BH_StartState : BehaviourStateTemplate
{
    public BH_StartState(AIFSM owner)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();
        jobName = "Starting Up";
        //jobName += " " + _AI._agentData.FriendlyTeam.DisplayName()
    }

    public override void OnEntry()
    {
        if(_aifsm.GetOwnerAI()._agentData.AgentName.Contains("2"))
        {
            _aifsm._baseRole = AIFSM.BaseRole.Defender;
        }
        else
        {
            _aifsm._baseRole = AIFSM.BaseRole.Attacker;
        }
    }
    public override AI.ExecuteResult Execute()
    {
        if(_aifsm._baseRole == AIFSM.BaseRole.Attacker)
        {
        }
        else if(_aifsm._baseRole == AIFSM.BaseRole.Defender)
        {
            _aifsm.SetCurrentState(new BH_DefenderPatrol(_aifsm));
        }
        else
        {
            Debug.LogError(_aifsm.GetOwnerAI()._agentData.AgentName + " has no role assigned");
        }

        returnResult.success = true;
        return returnResult;
    }
    public override void OnExit()
    {
        // nothing to see here
    }
}
