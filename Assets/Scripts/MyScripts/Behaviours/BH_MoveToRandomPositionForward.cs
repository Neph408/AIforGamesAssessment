using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BH_MoveToRandomPositionForward : BehaviourStateTemplate
{
    public BH_MoveToRandomPositionForward(AIFSM owner)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();
    }

    public override void OnEntry()
    {
        if(_aifsm.GetCurrentState() != this)
        {
            _aifsm.SetCurrentState(this);
        }
    }
    public override void Execute()
    {
        throw new System.NotImplementedException();
    }
    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
