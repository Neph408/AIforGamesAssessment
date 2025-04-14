using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class BH_AttackTarget : BehaviourStateTemplate
{
    private GameObject TargetObject;
    private BehaviourStateTemplate ReturnState;

    public BH_AttackTarget(AIFSM owner, BehaviourStateTemplate _ReturnState, GameObject _Target)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();

        ReturnState = _ReturnState;
        TargetObject = _Target;

        jobName = "Attacking";
        jobName += " " + TargetObject.name;
    }

    public override void OnEntry()
    {
        //throw new System.NotImplementedException();
    }
    public override AI.ExecuteResult Execute()
    {
        UpdateVision();

        if (GetYNegatedMagnitude(TargetObject) > _AI._agentData.AttackRange)
        {
            _AI._agentActions.MoveTo(TargetObject);
        }
        else if(TargetObject != null)
        {
            _AI._agentActions.Stop();
            _AI._agentActions.AttackEnemy(TargetObject);
        }

        if (TargetObject == null)
        {
            _aifsm.SetCurrentState(ReturnState);
        }

        returnResult.success = true;
        return returnResult;
    }
    public override void OnExit()
    {
    }

    public Vector3 GetApproxDistance()
    {
        Vector3 targetpos = TargetObject.transform.position;
        float distToAvoid = 0.5f;
        Vector3 vectorToTarget = (targetpos - _AI.gameObject.transform.position).normalized;
        return targetpos - (vectorToTarget * distToAvoid);
    }
}
