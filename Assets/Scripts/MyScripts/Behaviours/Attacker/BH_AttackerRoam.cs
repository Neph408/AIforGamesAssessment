using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_AttackerRoam : BehaviourStateTemplate
{

    public BH_AttackerRoam(AIFSM owner)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();

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
            _aifsm.SetCurrentState(new BH_AttackTarget(_aifsm, GetFlagHolderIfPresent(), new BH_AttackerRoam(_aifsm)));
        }

        return GenerateResult(true);
    }
    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

}
