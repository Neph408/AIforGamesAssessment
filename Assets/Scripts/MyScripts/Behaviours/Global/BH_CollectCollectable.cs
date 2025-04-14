using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_CollectCollectable : BehaviourStateTemplate
{
    private BehaviourStateTemplate ReturnState;
    private GameObject IntendedCollectable;

    private bool reachedTarget = false;

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
        UpdateVision();

        if (IntendedCollectable == null)
        {
            _aifsm.SetCurrentState(ReturnState);
        }



        if (MoveToPosition(IntendedCollectable, 0f))
        {
            if (IntendedCollectable.name == "Blue Flag" || IntendedCollectable.name == "Red Flag")
            {
                _aifsm._overrideRole = AIFSM.OverrideRole.Retriever;
            }
            if(_AI._agentSenses.IsItemInReach(IntendedCollectable))
            {
                if (_AI._agentInventory.AddItem(IntendedCollectable))
                {
                    _AI._agentActions.CollectItem(IntendedCollectable);
                }   
            }
            _aifsm.SetCurrentState(ReturnState);
            return GenerateResult(true);
        }

        return GenerateResult(true);
    }
    public override void OnExit()
    {
        //Debug.Log(_AI._agentData.FriendlyTeam.ToString() + " is no longer collecting ");
        //throw new System.NotImplementedException();
    }

}
