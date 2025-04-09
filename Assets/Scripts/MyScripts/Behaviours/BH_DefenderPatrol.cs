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
        Debug.Log(_AI._agentData.FriendlyTeam.ToString() + " Roaming");
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
        if (nearbyData.Collectable.exists) // nearby collectable collection check, moves to BH_CollectableCollect on success
        {
            if(_AI._agentInventory.GetInventoryUsage() < _AI._agentInventory.Capacity)
            {
                if (DecideChoice(CalculatorFunction.Collectable, nearbyData.Collectable.gameObject))
                {
                    _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_DefenderPatrol(_aifsm,currentPatrolPoint), nearbyData.Collectable.gameObject));

                    returnResult.success = true;
                    return returnResult;
                }
                else
                {
                    _aifsm._ignoredObjectList.Add(nearbyData.Collectable.gameObject);
                }
            }
        }




        // Movement Logic
        if(MoveToPosition(patrolPoints[currentPatrolPoint], patrolWaitTime))
        {
            MoveToNextTarget(); 
        }


        returnResult.success = true; //placeholder for now
        return returnResult; // return data to AI
    }
    public override void OnExit()
    {
        
    }

    private void MoveToNextTarget(int specificPoint = -1) // ez rotation between locations, optional specific point selection
    {
        currentPatrolPoint = ((specificPoint == -1) ? currentPatrolPoint + 1 : specificPoint) % 3;
        returnResult.jobTitle = jobName + " to point " + currentPatrolPoint.ToString();
    }

}
