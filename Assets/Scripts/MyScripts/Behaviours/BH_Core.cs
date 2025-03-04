using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BH_Core : BehaviourStateTemplate
{

    public BH_Core(AIFSM owner)
    {
        _aifsm = owner;
    }

    public override void OnEntry()
    {
        throw new System.NotImplementedException();
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
