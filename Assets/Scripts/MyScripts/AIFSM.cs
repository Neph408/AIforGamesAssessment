using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFSM
{
    private AI OwnerAI = null;

    public BH_MoveToRandomPositionForward b_MoveToRandomPositionForward;
    public BH_StartState b_StartState;
    private BehaviourStateTemplate CurrentState;
    public AIFSM(AI playerAI)
    {
        OwnerAI = playerAI;
        SetupFSM();
    }
    private void SetupFSM()
    {
        b_StartState = new BH_StartState(this);
        SetCurrentState(b_StartState);
    }

    public BehaviourStateTemplate GetCurrentState()
    {
        return CurrentState;
    }

    public void SetCurrentState(BehaviourStateTemplate newBST)
    {
        if(CurrentState != null) {CurrentState.OnExit();}

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
}
