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
    private float timer;
    private bool reachedTarget = false;

    private int forcedPosOnSpawn = -1;

    public BH_DefenderPatrol(AIFSM owner, int forcedPos = -1)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();
        jobName = "Patrolling";
        forcedPosOnSpawn = forcedPos;
        SetupPositions();

        jobName += " " + _AI._agentData.FriendlyTeam.ToString() + " Base";


    }
    private void SetupPositions()
    {
        Vector3 friendlyBasePos;

        patrolPoints = new Vector3[3];

        friendlyBasePos = _AI._agentData.FriendlyBase.transform.position;
        patrolPoints[0] = friendlyBasePos + new Vector3(-12.5f, 0f, 0f);
        patrolPoints[1] = friendlyBasePos + new Vector3(0f, 0f, 5f * -(friendlyBasePos.z / Mathf.Abs(friendlyBasePos.z)));
        patrolPoints[2] = friendlyBasePos + new Vector3(12.5f, 0f, 0f);
    }

    public override void OnEntry()
    {
        //Debug.Log("Enter defenderpatrol");


        if (forcedPosOnSpawn != -1)
        {
            MoveToNextTarget(forcedPosOnSpawn + Random.Range(2,4));
        }
        else
        {
            MoveToNextTarget(Random.Range(0, 3));
        }

        _AI.StartCoroutine(UpdateVision(DetectionUpdateFrequency.DefenderPatrol));
    }

    /*
     * 
     * ITS NOT WAITING WHN IT SHOULD
     * IF ROLL > BAR, IT SHOULD STOP AND WAIT FOR 3s, IT DOESNT
     * FIND OUT WHY
     * 
     * 
     */
    public override AI.ExecuteResult Execute()
    {
        // State Change Logic
        if (nearbyData.Collectable.exists)
        {
            if(_AI._agentInventory.GetInventoryUsage() < _AI._agentInventory.Capacity)
            {
                //please make a calc for this
                //if(Random.Range(0f,1f) < (!_AI._agentInventory.HasItem(nearbyData.Collectable.TargetGameObject.name).IsOwned ? Movement.DefenderInitialCollectChance : (Movement.DefenderDoesOwnershipReduceCollectChance ? Mathf.Pow(Movement.DefenderRepeatCollectChance, _AI._agentInventory.HasItem(nearbyData.Collectable.TargetGameObject.name).QuantityOwned) : Movement.DefenderRepeatCollectChance)))
                // i did
                if (DecideChoice(_aifsm._baseRole, _aifsm._overrideRole, CalculatorFunction.Collectable, nearbyData.Collectable.gameObject))
                {
                    _aifsm._ignoredObjectList.Add(nearbyData.Collectable.gameObject); // wh ut. This should not be running more than once, the state change would stop that, yet it was firing like 4 times
                    _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_DefenderPatrol(_aifsm,1), nearbyData.Collectable.gameObject));
                }
                else
                {
                    _aifsm._ignoredObjectList.Add(nearbyData.Collectable.gameObject);
                }
            }
        } 
                
         
        // Movement Logic
        if (!reachedTarget) // move to target if not at target
        {
            _AI._agentActions.MoveTo(patrolPoints[currentPatrolPoint]);
        }

        if(!reachedTarget && _aifsm.GetYNegatedMagnitude(patrolPoints[currentPatrolPoint], _AI.transform.position) < Movement.Leniency) // check if just reached target, should only fire once
        {
            reachedTarget = true;
            timer = patrolWaitTime;
            returnResult.jobTitle = jobName + " at point " + currentPatrolPoint.ToString();
        }

        if(reachedTarget && timer > 0f) // lazy timer
        {
            timer -= Time.deltaTime;
        }
        
        if(reachedTarget && timer <= 0f) // update to next patrol point
        {
            MoveToNextTarget();
            reachedTarget = false;
        }



        returnResult.success = true; //placeholder for now
        return returnResult; // return data to AI
    }
    public override void OnExit()
    {
        _AI.StopCoroutine(UpdateVision(DetectionUpdateFrequency.DefenderPatrol));
    }
    private void MoveToNextTarget(int specificPoint = -1) // ez rotation between locations, optional specific point selection
    {
        currentPatrolPoint = ((specificPoint == -1) ? currentPatrolPoint + 1 : specificPoint) % 3;
        returnResult.jobTitle = jobName + " to point " + currentPatrolPoint.ToString();
    }

}
