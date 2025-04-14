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
    }
    public override AI.ExecuteResult Execute()
    {
        UpdateVision();


        if(nearbyData.nearbyFlagCount > 0)
        {
            _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_AttackerRoam(_aifsm), GetFlagByPriority()));
        }

        if (nearbyData.nearbyEnemyCount > 0)
        {
            _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, new BH_AttackerRoam(_aifsm), GetFlagHolderIfPresent()));
        }

        if(nearbyData.Collectable.exists)
        {
            if (DecideChoice(CalculatorFunction.Collectable, nearbyData.Collectable.targetGameObject))
            {
                _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_AttackerRoam(_aifsm), nearbyData.Collectable.targetGameObject));
            }
            else
            {
                _aifsm._ignoredObjectList.Add(nearbyData.Collectable.targetGameObject, AIConstants.Global.IgnoreCollectableDuration);
            }
        }

        if(forcedTargetPosition)
        {
            if(_AI._agentActions.MoveTo(targetPosition))
            {
                forcedTargetPosition = false;
            }
        }
        else
        {
            _AI._agentActions.MoveToRandomLocation();
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
