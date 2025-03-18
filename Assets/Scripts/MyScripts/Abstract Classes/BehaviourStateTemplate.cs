using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourStateTemplate
{
    public AIFSM _aifsm;
    private string className = "";

    public abstract void OnEntry();
    public abstract void Execute();
    public abstract void OnExit();

    public string GetName()
    {
        return className;
    }
}
