using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static AIConstants;

public class BH_ProtectorCover : BehaviourStateTemplate
{
    private BehaviourStateTemplate ReturnState;
    private GameObject TargetToCover;
    private AI AIToCover;

    public BH_ProtectorCover(AIFSM owner, BehaviourStateTemplate _returnState, GameObject _TargetToCover)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();

        ReturnState = _returnState;
        TargetToCover = _TargetToCover;
        AIToCover = TargetToCover.GetComponent<AI>();

        jobName = "Covering ";

    }

    public override void OnEntry()
    {
        UpdateVision();
        jobName += " " + GetTargetAsFormattedString();
        if(!AIToCover._playerFSM.GetCurrentState().HasProtector(_AI))
        {
            AIToCover._playerFSM.GetCurrentState().AddProtector(_AI);
        }
    }
    public override AI.ExecuteResult Execute()
    {
        UpdateVision();

        if(TargetToCover == null || (!AIToCover._agentData.HasFriendlyFlag && !AIToCover._agentData.HasEnemyFlag))// || nearbyData.nearbyAllyHoldingFlag == 0) // ai dies or no longer holding a flag or //no more allies w/ flags nearby (for whatever reason)
        {
            _aifsm.SetCurrentState(ReturnState);
            _aifsm._overrideRole = AIFSM.OverrideRole.None;
            GenerateResult(true);
        }


        if(nearbyData.nearbyEnemyCount > 0)
        {
            _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, new BH_ProtectorCover(_aifsm, ReturnState, TargetToCover), GetNearestEnemy(), TargetToCover));
        }

        _AI._agentActions.MoveTo(TargetToCover);

        
        return GenerateResult(true);
    }
    public override void OnExit()
    {
        if (AIToCover != null)
        {
            if (!AIToCover._playerFSM.GetCurrentState().HasProtector(_AI))
            {
                AIToCover._playerFSM.GetCurrentState().AddProtector(_AI);
            }
        }
    }

    public override void RetrieverTakenDamage(GameObject attacker)
    {
        _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, new BH_ProtectorCover(_aifsm, ReturnState, TargetToCover), attacker, TargetToCover));
    }

    public string GetTargetAsFormattedString()
    {
        foreach (NearbyObjectData nod in nearbyData.Enemy)
        {
            if (nod.targetGameObject == TargetToCover)
            {
                return nod.GetSlotAsString(true);
            }
        }
        return TargetToCover.name;
    }


}
