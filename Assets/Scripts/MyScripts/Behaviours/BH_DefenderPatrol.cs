using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class BH_DefenderPatrol : BehaviourStateTemplate
{
    
    public Vector3 patrolPoint1;
    public Vector3 patrolPoint2;
    public Vector3 patrolPoint3;

    public Vector3 currentPatrolTarget;

    public int currentPatrolPoint;

    public float patrolWaitTime = 3f;
    private float timer;
    private bool reachedTarget = false;
    private float leniency = 0.25f; // how close "close enough" is for reachedTarget 

    public BH_DefenderPatrol(AIFSM owner)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();
    }

    public override void OnEntry()
    {
        Debug.Log("Enter defenderpatrol");

        Vector3 friendlyBasePos;

        friendlyBasePos = _AI._agentData.FriendlyBase.transform.position;
        patrolPoint1 = friendlyBasePos + new Vector3(12.5f,0f,0f);
        patrolPoint2 = friendlyBasePos + new Vector3(-12.5f,0f,0f);
        patrolPoint3 = friendlyBasePos + new Vector3(0f,0f,5f * -(friendlyBasePos.z / Mathf.Abs(friendlyBasePos.z)));

        currentPatrolPoint = Random.Range(0, 3);
        SetTargetByInt(currentPatrolPoint);
    }
    public override void Execute()
    {
        // State Change Logic
        if (true == true)
            {
                //_aifsm.SetCurrentState(new b_Core)
                // figure out how to call correct behaviour
                // the answer is dynamically set them
            }

        // Movement Logic
        if (!reachedTarget) // move to target if not at target
        {
            _AI._agentActions.MoveTo(currentPatrolTarget);
        }

        if(!reachedTarget && _aifsm.GetYNegatedMagnitude(currentPatrolTarget, _AI.transform.position) < leniency) // check if just reached target, should only fire once
        {
            reachedTarget = true;
            timer = patrolWaitTime;
            Debug.Log(_AI._agentData.FriendlyTeam.HumanName() + " Patroller Reached target, waiting for " + patrolWaitTime.ToString() + "s");
        }

        if(reachedTarget && timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        
        if(reachedTarget && timer <= 0f)
        {
            SetTargetByInt(currentPatrolPoint + 1);
            reachedTarget = false;
        }



        
    }
    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }


    private void SetTargetByInt(int newPoint) // ez rotation between locations
    {
        currentPatrolPoint = newPoint % 3;
        if (currentPatrolPoint == 0)
        {
            currentPatrolTarget = patrolPoint1;
        }
        else if (currentPatrolPoint == 1)
        {
            currentPatrolTarget = patrolPoint2;
        }
        else
        {
            currentPatrolTarget = patrolPoint3;
        }
        Debug.Log(_AI._agentData.FriendlyTeam.HumanName() + " Patroller heading to position " + currentPatrolPoint.ToString() + ", vector " + currentPatrolTarget.ToString());
    }
}
