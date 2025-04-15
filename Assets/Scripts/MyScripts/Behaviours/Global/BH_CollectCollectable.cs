using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class BH_CollectCollectable : BehaviourStateTemplate
{
    private BehaviourStateTemplate ReturnState;
    private GameObject IntendedCollectable;

    private float timeoutTimestamp;//fix for bug

    public BH_CollectCollectable(AIFSM owner, BehaviourStateTemplate _ReturnState, GameObject _IntendedCollectable)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();
        IntendedCollectable = _IntendedCollectable;
        ReturnState = _ReturnState;

        timeoutTimestamp = Time.time + Random.Range(7f, 12f);

        jobName = "Collecting Collectable";
        jobName += " " + _IntendedCollectable.name + " [" + IntendedCollectable.transform.position.ToString()+"]";
    }

    public override void OnEntry()
    {
    }
    public override AI.ExecuteResult Execute()
    {
        UpdateVision();

        if ( Time.time > timeoutTimestamp)// in rare occasions, friendly ai can both try to grab the same collectable at the same time and push against eacherother in such a way that prevenst them picking the collectable up, this just means they stop trying after a certain duration, randomly however to allow for at least one to get .
        {
            _aifsm._ignoredObjectList.Add(IntendedCollectable, 2f);
            _AI._agentActions.MoveTo(_AI.transform.position + new Vector3(Random.Range(-2f,2f),0f, Random.Range(-2f, 2f)));
            _aifsm.SetCurrentState(ReturnState);
            return GenerateResult(true);
        }

        if (IntendedCollectable == null)
        {
            _aifsm.SetCurrentState(ReturnState);
            return GenerateResult(true);
        }
        if(IntendedCollectable.transform.parent != null)
        {
            _aifsm.SetCurrentState(ReturnState);
            return GenerateResult(true);
        }

        if(_aifsm._overrideRole == AIFSM.OverrideRole.None)
        {
            if (nearbyData.nearbyAllyHoldingFlag > 0 && _aifsm._overrideRole != AIFSM.OverrideRole.Retriever)
            {
                _aifsm._overrideRole = AIFSM.OverrideRole.Protector;
                _aifsm.SetCurrentState(new BH_ProtectorCover(_aifsm, ReturnState, GetAllyHoldingFlagByPriority()));
                return GenerateResult(true);
            }
        }

        if (MoveToPosition(IntendedCollectable, 0f))
        {
            if(_AI._agentSenses.IsItemInReach(IntendedCollectable)) // distance check for collect
            {
                if (_AI._agentInventory.AddItem(IntendedCollectable)) // check if can add, if can, add
                {
                    if (IntendedCollectable.name == _AI._agentData.FriendlyFlagName || IntendedCollectable.name == _AI._agentData.EnemyFlagName)// check if item was flag
                    {

                        _aifsm._overrideRole = AIFSM.OverrideRole.Retriever; // assign retriever

                    }
                    _AI._agentActions.CollectItem(IntendedCollectable); // pick item off ground
                } 
                else
                {
                    if(IntendedCollectable.name == _AI._agentData.FriendlyFlagName || IntendedCollectable.name == _AI._agentData.EnemyFlagName) // if couldnt add, check if was flag
                    {
                        if(_AI._agentInventory.HasItem("Power Up").quantityOwned > _AI._agentInventory.Capacity / 2) // if was flag, discard a single powerup if occupies more than half the inv. will prioritise most held
                        {
                            _AI._agentActions.DropItem(_AI._agentInventory.GetItem("Power Up"));
                        }
                        if (_AI._agentInventory.HasItem("Health Kit").quantityOwned > _AI._agentInventory.Capacity / 2)// if was flag, discard a single powerup if occupies more than half the inv. will prioritise most held
                        {
                            _AI._agentActions.DropItem(_AI._agentInventory.GetItem("Health Kit"));
                        }
                        _aifsm._overrideRole = AIFSM.OverrideRole.Retriever;  // assign ret
                        _AI._agentActions.CollectItem(IntendedCollectable); // pick up
                    }
                }
            }
            if (_aifsm._overrideRole == AIFSM.OverrideRole.Retriever)// force override BH to RetRet
            {
                _aifsm.SetCurrentState(new BH_RetrieverReturn(_aifsm, ReturnState));
                return GenerateResult(true);
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
