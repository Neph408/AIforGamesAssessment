using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BH_MoveToRandomPositionForward : BehaviourStateTemplate
{

    public BH_MoveToRandomPositionForward(AIFSM owner)
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

        if (true == true)
        {
            //_aifsm.SetCurrentState(new b_Core)
            // figure out how to call correct behaviour
            // the answer is dynamically set them
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
