using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_AttackerCore : BehaviourStateTemplate
{
    // USE THIS AS JUST AN EASY C/V FOR NEW BEHAVIOURS
    // DO NOT USE AS AN ACTUAL BEHAVIOUR

    public BH_AttackerCore(AIFSM owner)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();

        jobName = "UNNAMED JOB";
        //jobName += " " + _AI._agentData.FriendlyTeam.DisplayName()
    }

    public override void OnEntry()
    {
        Debug.Log(_AI.gameObject.name + " Entered AttackerCore");
        UpdateVision();
    }
    public override AI.ExecuteResult Execute()
    {
        if (nearbyData.Enemy[0] != null)
        {
            //_aifsm.SetCurrentState(new b_Core)
            // figure out how to call correct behaviour
            // the answer is dynamically set them
        }
        else
        {
            _aifsm.SetCurrentState(new BH_AttackerRoam(_aifsm));
        }
        throw new System.NotImplementedException();
        returnResult.success = true;
        return returnResult;
    }
    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
