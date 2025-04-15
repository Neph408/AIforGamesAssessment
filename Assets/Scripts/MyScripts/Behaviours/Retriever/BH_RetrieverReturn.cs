using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using static AIConstants;

public class BH_RetrieverReturn : BehaviourStateTemplate
{
    public GameObject HeldFlag;

    private BehaviourStateTemplate ReturnState;

    private GameObject TargetLocation;

    private AI[] Protectors;

    private float TimestampOfLastAlert = 0f;

    public BH_RetrieverReturn(AIFSM owner, BehaviourStateTemplate _ReturnState)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();
        ReturnState = _ReturnState;
        Protectors = new AI[2];
        jobName = "Retrieving Flag";
    }

    public override void OnEntry()
    {
        if(_AI._agentInventory.HasItem("Blue Flag").owned)
        {
            HeldFlag = _AI._agentInventory.GetItem("Blue Flag");
        }
        if (_AI._agentInventory.HasItem("Red Flag").owned)
        {
            HeldFlag = _AI._agentInventory.GetItem("Red Flag");
        }

        if (HeldFlag == null)
        {
            Debug.LogError("Was sent to RetrieverReturn while not holding flag, please fix");
            _aifsm.SetCurrentState(ReturnState);
            return;
        }

        TargetLocation = _AI._agentData.FriendlyBase;
    }
    public override AI.ExecuteResult Execute()
    {
        UpdateVision();
        
        HealthConsumableCheck();

        if(nearbyData.nearbyFlagCount > 0)
        {
            if(_AI._agentData.HasFriendlyFlag && nearbyData.Flag[0].targetGameObject.name == _AI._agentData.FriendlyFlagName && _AI._agentInventory.GetInventoryUsage() == _AI._agentInventory.Capacity) // if inv full, prioritise holding own flag over enemy
            {
                _AI._agentActions.DropItem(HeldFlag);
            }
                _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, ReturnState, nearbyData.Flag[0].targetGameObject));
                return GenerateResult(true);
        }

        if(nearbyData.Collectable.exists) // while retrieveing, if ai sees a health kit, it may choose to collect it, but with notably reduced chances, increasinly so if it owns one already
        {
            if(nearbyData.Collectable.targetGameObject.name == "Health Kit")
            {
                if(DecideChoice(CalculatorFunction.Collectable, nearbyData.Collectable.targetGameObject))
                {
                    _aifsm.SetCurrentState(new BH_CollectCollectable(_aifsm, new BH_RetrieverReturn(_aifsm, ReturnState), nearbyData.Collectable.targetGameObject));
                    GenerateResult(true);
                }
            }
        }

        if(_AI._agentActions.MoveTo(TargetLocation))
        {
             if(GetYNegatedMagnitude(TargetLocation,_AI.gameObject) < AIConstants.Global.Leniency && nearbyData.nearbyEnemyCount <= 1)
             {
                _AI._agentActions.DropItem(HeldFlag);
                _aifsm._overrideRole = AIFSM.OverrideRole.None;
                _aifsm.SetCurrentState(ReturnState);
                return GenerateResult(true);
            }
        }

        
        return GenerateResult(true);
    }
    public override void OnExit()
    {

    }

    public override void AddProtector(AI prot)
    {
        Protectors[(Protectors[0] == null) ? 0 : 1] = prot;
    }
    public override bool HasProtector(AI prot)
    {
        foreach (AI protector in Protectors)
        {
            if (protector == prot)
            {
                return true;
            }
        }
        return false;
    }
    public override void RemoveProtector(AI prot)
    {
        if (Protectors[1] == prot)
        {
            Protectors[1] = null;
        }
        else if (Protectors[0] == prot)
        {
            Protectors[0] = null;
            if(Protectors[1] != null)
            {
                Protectors[0] = Protectors[1];
                Protectors[1] = null;
            }
        }
        else
        {
            Debug.LogError("Protector cannot be removed as it is not on the protector list");
        }
    }

    public override void HasTakenDamage(GameObject attacker)
    {
        if (Time.time - TimestampOfLastAlert > AIConstants.Retriever.AlertCooldown)
        {
            foreach (AI prot in Protectors)
            {
                if (prot != null)
                {
                    prot._playerFSM.GetCurrentState().RetrieverTakenDamage(attacker);
                }
            }
            TimestampOfLastAlert = Time.time;
        }
    }

}
