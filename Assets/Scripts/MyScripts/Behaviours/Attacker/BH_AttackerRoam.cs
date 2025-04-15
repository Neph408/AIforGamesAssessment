using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_AttackerRoam : BehaviourStateTemplate
{

    private bool forcedTargetPosition = false;
    private Vector3 targetPosition;

    public BH_AttackerRoam(AIFSM owner)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();

        
        jobName = "Roaming";
        forcedTargetPosition = false;
    }
    public BH_AttackerRoam(AIFSM owner, Vector3 newPosition)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();

        forcedTargetPosition=true;
        targetPosition = newPosition;
        jobName = "Roaming";
    }

    public override void OnEntry()
    {
        Debug.Log(_AI.gameObject.name + " Enter AttackerRoam");
        targetPosition = GetRandomPositionTowards(_AI._agentData.EnemyBase);
    }
    public override AI.ExecuteResult Execute()
    {
        UpdateVision();

        if(nearbyData.nearbyAllyHoldingFlag > 0 && _aifsm._overrideRole != AIFSM.OverrideRole.Retriever)
        {
            _aifsm._overrideRole = AIFSM.OverrideRole.Protector;
            _aifsm.SetCurrentState(new BH_ProtectorCover(_aifsm, new BH_AttackerRoam(_aifsm), GetAllyHoldingFlagByPriority()));
            return GenerateResult(true);
        }

        if(nearbyData.nearbyEnemyHoldingFlag > 0) // if enemy flag holder nearby, attack
        {
            _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, new BH_AttackerRoam(_aifsm), GetFlagHolderIfPresent()));
            return GenerateResult(true);
        }

        if(nearbyData.nearbyFlagCount > 0) //  otherwise, pickup flag
        {
            /*if(nearbyData.nearbyEnemyCount == 1 && GetYNegatedMagnitude(nearbyData.Enemy[0].targetGameObject, _AI.transform.position) < 20f)
            {
                _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, new BH_AttackerRoam(_aifsm), nearbyData.Enemy[0].targetGameObject));
                return GenerateResult(true);
            }*/
            _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_AttackerRoam(_aifsm), GetFlagByPriority()));
            return GenerateResult(true);
        }

        if (nearbyData.nearbyEnemyCount > 0) // otherwise, attack enemy
        {
            _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, new BH_AttackerRoam(_aifsm), GetFlagHolderIfPresent()));
            return GenerateResult(true);
        }

        if(nearbyData.Collectable.exists) // otherwise, collect collectables
        {
            if (DecideChoice(CalculatorFunction.Collectable, nearbyData.Collectable.targetGameObject))
            {
                _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_AttackerRoam(_aifsm), nearbyData.Collectable.targetGameObject));
                return GenerateResult(true);
            }
            else
            {
                _aifsm._ignoredObjectList.Add(nearbyData.Collectable.targetGameObject, AIConstants.Global.IgnoreCollectableDuration);
            }
        }

        if(forcedTargetPosition)
        {
            if(MoveToPosition(targetPosition))
            {
                forcedTargetPosition = false;
            }
        }
        else
        {
            _AI._agentActions.MoveToRandomLocation();
            /*if(MoveToPosition(targetPosition))
            {
                targetPosition = GetRandomPositionTowards(_AI._agentData.EnemyBase,-1);
            }*/
        }


        return GenerateResult(true);
    }
    public override void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    /*public override void HasTakenDamage()
    {
        base.HasTakenDamage();
        if (nearbyData.nearbyEnemyCount > 0)
        {
            _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, new BH_AttackerRoam(_aifsm), GetFlagHolderIfPresent()));
       }
    }*/
}
