using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class BH_DefenderPatrol : BehaviourStateTemplate
{
    public Vector3[] patrolPoints;

    public int currentPatrolPoint;

    public float patrolWaitTime = 3f;


    private int forcedPosOnSpawn = -1;

    public BH_DefenderPatrol(AIFSM owner, int forcedPos = -1)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();
        jobName = "Patrolling";
        forcedPosOnSpawn = forcedPos;
        SetupPositions();
        Debug.Log(_AI.gameObject.name + " Patrolling");
        jobName += " " + _AI._agentData.FriendlyTeam.ToString() + " Base";


    }
    private void SetupPositions()
    {
        Vector3 friendlyBasePos;

        patrolPoints = new Vector3[3];

        friendlyBasePos = _AI._agentData.FriendlyBase.transform.position;
        patrolPoints[0] = friendlyBasePos + new Vector3(-12.5f * -(friendlyBasePos.z / Mathf.Abs(friendlyBasePos.z)), 0f, 0f);
        patrolPoints[1] = friendlyBasePos + new Vector3(0f, 0f, 5f * -(friendlyBasePos.z / Mathf.Abs(friendlyBasePos.z)));
        patrolPoints[2] = friendlyBasePos + new Vector3(12.5f * -(friendlyBasePos.z / Mathf.Abs(friendlyBasePos.z)), 0f, 0f);
    }

    public override void OnEntry()
    {
        //Debug.Log("Enter defenderpatrol");
        if (forcedPosOnSpawn != -1)
        {
            MoveToNextTarget(forcedPosOnSpawn + UnityEngine.Random.Range(0,2));
        }
        else
        {
            MoveToNextTarget(UnityEngine.Random.Range(0, 3));
        }
    }

    public override AI.ExecuteResult Execute()
    {
        UpdateVision();
        // State Change Logic
        if (nearbyData.nearbyFlagCount > 0)
        {
            _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_DefenderPatrol(_aifsm), GetFlagByPriority()));
        }
       if (nearbyData.nearbyEnemyCount > 0)
        {
            _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, new BH_DefenderPatrol(_aifsm, currentPatrolPoint), GetFlagHolderIfPresent()));
        }
        if (nearbyData.Collectable.exists) // nearby collectable collection check, moves to BH_CollectableCollect on success
        {
            if(_AI._agentInventory.GetInventoryUsage() < _AI._agentInventory.Capacity)
            {
                if (DecideChoice(CalculatorFunction.Collectable, nearbyData.Collectable.gameObject))
                {
                    _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_DefenderPatrol(_aifsm,currentPatrolPoint), nearbyData.Collectable.gameObject));
                }
                else
                {
                    _aifsm._ignoredObjectList.Add(nearbyData.Collectable.gameObject, AIConstants.Global.IgnoreCollectableDuration);
                }
            }
        }




        // Movement Logic
        if(MoveToPosition(patrolPoints[currentPatrolPoint], patrolWaitTime))
        {
            MoveToNextTarget(); 
        }


        return GenerateResult(true); // return data to AI
    }
    public override void OnExit()
    {
        
    }

    private void MoveToNextTarget(int specificPoint = -1) // ez rotation between locations, optional specific point selection
    {
        currentPatrolPoint = ((specificPoint == -1) ? currentPatrolPoint + 1 : specificPoint) % 3;
        jobName = jobName + " to point " + currentPatrolPoint.ToString();
    }

    public override void HasTakenDamage()
    {
        base.HasTakenDamage();
        if (nearbyData.nearbyEnemyCount > 0)
        {
            _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, new BH_DefenderPatrol(_aifsm, currentPatrolPoint), GetFlagHolderIfPresent()));
        }
    }

}
