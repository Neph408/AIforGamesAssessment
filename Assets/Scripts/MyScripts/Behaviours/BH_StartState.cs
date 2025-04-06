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
    }

    public override void OnEntry()
    {
        Debug.Log("Entered StartState");
        if(_aifsm.GetOwnerAI()._agentData.AgentName.Contains("2"))
        {
            _aifsm.role = AIFSM.e_Role.Defender;
        }
        else
        {
            _aifsm.role = AIFSM.e_Role.Attacker;
        }
    }
    public override void Execute()
    {
        if(_aifsm.role == AIFSM.e_Role.Attacker)
        {
        }
        else if(_aifsm.role == AIFSM.e_Role.Defender)
        {
            _aifsm.SetCurrentState(new BH_DefenderPatrol(_aifsm));
        }
        else
        {
            Debug.LogError(_aifsm.GetOwnerAI()._agentData.AgentName + " has no role assigned");
        }
        
    }
    public override void OnExit()
    {
        Debug.Log("Exiting StartState");
    }
}
