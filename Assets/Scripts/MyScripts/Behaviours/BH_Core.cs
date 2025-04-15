using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_Core : BehaviourStateTemplate
{
    // USE THIS AS JUST AN EASY C/V FOR NEW BEHAVIOURS
    // DO NOT USE AS AN ACTUAL BEHAVIOUR

    public BH_Core(AIFSM owner)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();

        jobName = "UNNAMED JOB";
        //jobName += " " + _AI._agentData.FriendlyTeam.DisplayName()
    }

    public override void OnEntry()
    {
        throw new System.NotImplementedException();
    }
    public override AI.ExecuteResult Execute()
    {

        if(true == true)
        {
            //_aifsm.SetCurrentState(new b_Core)
            // figure out how to call correct behaviour
            // the answer is dynamically set them
        }
        throw new System.NotImplementedException();
        
        //return GenerateResult(true);
    }
    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
