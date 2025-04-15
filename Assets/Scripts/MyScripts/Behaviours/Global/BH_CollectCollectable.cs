using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
            if(_AI._agentSenses.IsItemInReach(IntendedCollectable))
            {
                if (_AI._agentInventory.AddItem(IntendedCollectable))
                {
                    if (IntendedCollectable.name == _AI._agentData.FriendlyFlagName || IntendedCollectable.name == _AI._agentData.EnemyFlagName)
                    {

                        _aifsm._overrideRole = AIFSM.OverrideRole.Retriever;

                    }
                    _AI._agentActions.CollectItem(IntendedCollectable);
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
