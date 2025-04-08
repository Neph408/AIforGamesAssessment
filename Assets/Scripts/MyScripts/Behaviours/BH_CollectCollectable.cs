using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_CollectCollectable : BehaviourStateTemplate
{
    private BehaviourStateTemplate ReturnState;
    private GameObject IntendedCollectable;

    public BH_CollectCollectable(AIFSM owner, BehaviourStateTemplate _ReturnState, GameObject _IntendedCollectable)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();
        IntendedCollectable = _IntendedCollectable;
        ReturnState = _ReturnState;

        jobName = "Collecting Collectable";
        jobName += " " + _IntendedCollectable.name + " [" + IntendedCollectable.transform.position.ToString()+"]";
    }

    public override void OnEntry()
    {
        Debug.Log(_AI._agentData.FriendlyTeam.ToString()+" Collecting Shit");
    }
    public override AI.ExecuteResult Execute()
    {
        //AI.Destroy(IntendedCollectable);
        if(true == true)
        {
            //_aifsm.SetCurrentState(new b_Core)
            // figure out how to call correct behaviour
            // the answer is dynamically set them
        }
        _aifsm.SetCurrentState(ReturnState);
        returnResult.success = true;
        return returnResult;
    }
    public override void OnExit()
    {
        Debug.Log(_AI._agentData.FriendlyTeam.ToString() + " is no longer collecting ");
        //throw new System.NotImplementedException();
    }

}
