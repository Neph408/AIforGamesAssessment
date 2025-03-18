using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_DefenderPatrol : BehaviourStateTemplate
{
    // USE THIS AS JUST AN EASY C/V FOR NEW BEHAVIOURS
    // DO NOT USE AS AN ACTUAL BEHAVIOUR
    public BH_DefenderPatrol(AIFSM owner)
    {
        _aifsm = owner;
    }

    public override void OnEntry()
    {
        Debug.Log("Enter defenderpatrol");
    }
    public override void Execute()
    {

        if(true == true)
        {
            //_aifsm.SetCurrentState(new b_Core)
            // figure out how to call correct behaviour
            // the answer is dynamically set them
        }
        Debug.Log("defender patrols");
    }
    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }
}
