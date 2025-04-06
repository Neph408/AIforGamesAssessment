using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_Core : BehaviourStateTemplate
{
    // USE THIS AS JUST AN EASY C/V FOR NEW BEHAVIOURS
    // DO NOT USE AS AN ACTUAL BEHAVIOUR
    public BH_Core(AIFSM owner)
    {
        _aifsm = owner;
        _AI = owner.GetOwnerAI();
    }

    public override void OnEntry()
    {
        throw new System.NotImplementedException();
    }
    public override void Execute()
    {

        if(true == true)
        {
            //_aifsm.SetCurrentState(new b_Core)
            // figure out how to call correct behaviour
            // the answer is dynamically set them
        }
        throw new System.NotImplementedException();
    }
    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
