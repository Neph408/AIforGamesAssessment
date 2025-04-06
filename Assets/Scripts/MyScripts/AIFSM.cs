using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFSM
{
    public enum e_Role
    {
        Defender,
        Attacker
    }

    private AI OwnerAI = null;

    public BH_MoveToRandomPositionForward b_MoveToRandomPositionForward;
    public BH_StartState b_StartState;
    private BehaviourStateTemplate CurrentState;

    public e_Role role;
    public AIFSM(AI playerAI)
    {
        OwnerAI = playerAI;
        SetupFSM();
    }
    private void SetupFSM()
    {
        SetCurrentState(new BH_StartState(this));
    }

    public BehaviourStateTemplate GetCurrentState()
    {
        return CurrentState;
    }

    public void SetCurrentState(BehaviourStateTemplate newBST)
    {
        if(CurrentState != null) {CurrentState.OnExit();}
        OwnerAI.currentJob = newBST.GetName();
        CurrentState = newBST;
        CurrentState.OnEntry();
    }

    public void WipeCurrentState()
    {
        CurrentState.OnExit();
        CurrentState = null;
    }

    public bool FSMUpdate()
    {
        if(CurrentState != null)
        {
            CurrentState.Execute();
            return true;
        }
        return false;
    }

    public AI GetOwnerAI()
    {
        return OwnerAI;
    }

    public float GetYNegatedMagnitude(Vector3 Target, Vector3 CurrentPosition) // exists because i want to check how close the ai is to the intended target, y coord doesnt matter in this case
    {
        Target.y = 0;
        CurrentPosition.y = 0;
        return (Target - CurrentPosition).magnitude;
    }
}
