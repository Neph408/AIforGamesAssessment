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
    private int nearbyEnemyArrayPos;
    private GameObject ProtectorTether;

    public BH_AttackTarget(AIFSM owner, BehaviourStateTemplate _ReturnState, GameObject _Target)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();

        ReturnState = _ReturnState;
        TargetObject = _Target;

        jobName = "Attacking";
        
    }
    public BH_AttackTarget(AIFSM owner, BehaviourStateTemplate _ReturnState, GameObject _Target, GameObject _ProtectorTether)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();

        ReturnState = _ReturnState;
        TargetObject = _Target;
        ProtectorTether = _ProtectorTether;
        jobName = "Attacking";

    }

    public override void OnEntry()
    {
        UpdateVision();
        jobName += " " + GetTargetAsFormattedString();
    }
    public override AI.ExecuteResult Execute()
    {
        UpdateVision();

        if (TargetObject == null)
        {
            _aifsm.SetCurrentState(ReturnState);
            return GenerateResult(true);
        }
        if(_aifsm._overrideRole == AIFSM.OverrideRole.Protector)
        {
            if (GetYNegatedMagnitude(ProtectorTether, _AI.gameObject) > AIConstants.Protector.TetherDistance)
            {
                _aifsm.SetCurrentState(ReturnState);
                return GenerateResult(true);
            }
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

        HealthConsumableCheck();
        PowerupConsumableCheck(TargetObject);

        if(nearbyData.nearbyEnemyHoldingFlag > 0)
        {
            _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, ReturnState, GetFlagHolderIfPresent()));
            return GenerateResult(true);
        }

        if (nearbyData.nearbyFlagCount > 0) // the flag means everything, break away and try to get it
        {
            _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_AttackerRoam(_aifsm), GetFlagByPriority()));
            return GenerateResult(true);
        }

        if(CheckForNearerEnemy(TargetObject, out GameObject newTarget))
        {
            TargetObject = newTarget;
            jobName = jobName.Split(" ")[0] + " " + GetTargetAsFormattedString();
        }

        if (GetYNegatedMagnitude(TargetObject) > _AI._agentData.AttackRange)
        {
            _AI._agentActions.MoveTo(TargetObject);
        }
        else if(TargetObject != null)
        {
            _AI._agentActions.Stop();
            _AI._agentActions.AttackEnemy(TargetObject);
            TargetObject.GetComponent<AI>()._playerFSM.GetCurrentState().HasTakenDamage(_AI.gameObject);
        }

        

        return GenerateResult(true);
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

    public string GetTargetAsFormattedString()
    {
        foreach (NearbyObjectData nod in nearbyData.Enemy)
        {
            if(nod.targetGameObject == TargetObject)
            {
                return nod.GetSlotAsString(true);
            }
        }
        return TargetObject.name;
    }

    public override void RetrieverTakenDamage(GameObject attacker)
    {
        if(_aifsm._overrideRole == AIFSM.OverrideRole.Protector)
        {
            TargetObject = attacker;
            jobName = jobName.Split(" ")[0] + " " + GetTargetAsFormattedString();
        }
    }

}
