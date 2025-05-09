using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AIFSM;
using static BehaviourStateTemplate;

public class AIFSM
{
    public enum BaseRole
    {
        Defender,
        Attacker
    }
    public enum OverrideRole
    {
        None,
        Protector,
        Retriever
    }

    private AI OwnerAI = null;

    public IgnoredObjectList _ignoredObjectList;

    private BehaviourStateTemplate CurrentState;

    public BaseRole _baseRole;
    public OverrideRole _overrideRole;

    private AI.ExecuteResult failureState;

    public AIFSM(AI playerAI)
    {
        OwnerAI = playerAI;

        _ignoredObjectList = new IgnoredObjectList();

        _overrideRole = OverrideRole.None; // for safety, should ever be not none by default

        failureState.success = false;
        failureState.jobTitle = "No Assigned Behaviour";

        SetupFSM();
    }
    private void SetupFSM()
    {
        SetCurrentState(new BH_StartState(this));
    }

    public bool HasCurrentState()
    {
        return CurrentState != null;
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
        if (CurrentState != null)
        {
            CurrentState.OnExit();
            CurrentState = null;
        }
    }
    public AI.ExecuteResult FSMUpdate()
    {
        _ignoredObjectList.Update();
        if(CurrentState != null)
        {
            return CurrentState.Execute(); ;
        }
        return failureState;
    }

    public AI GetOwnerAI()
    {
        return OwnerAI;
    }
}
