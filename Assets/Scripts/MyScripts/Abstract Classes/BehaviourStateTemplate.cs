using JetBrains.Annotations;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro.EditorUtilities;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class BehaviourStateTemplate
{   
    public struct GameKnowledgeData
    {
        public bool IsAllyFlagNotAtBase;
        public bool IsEnemyFlagNotAtBase;

    }
    public enum CalculatorFunction
    {
        Collectable,
        Enemy
    }

    public AIFSM _aifsm;
    public AI _AI;
    
    public string jobName;

    protected DebugOverlayHandler doh;

    public AI.ExecuteResult returnResult;
    public NearbyData nearbyData;

    private string LogText;

    private float timer;
    private bool reachedTarget = false;

    protected List<GameObject> ObjectsInView;

    public BehaviourStateTemplate()
    {
        nearbyData = new NearbyData();
        nearbyData.ClearData();
        doh = DebugOverlayHandler.DOH;
    }

    public abstract void OnEntry();

    public abstract AI.ExecuteResult Execute();

    public abstract void OnExit();

    public virtual void UpdateVision(bool OutputToLog = true)
    {
        AgentData.Teams selfTeam = _AI._agentData.FriendlyTeam;

        ObjectsInView = _AI._agentSenses.GetObjectsInView();
        nearbyData.ClearData(); // clears to not retain outdated info


        foreach (GameObject i in ObjectsInView)
        {
            switch (i.tag)
            {
                case "Blue Team":
                    RegisterNearbyAI(selfTeam, i);
                    break;
                case "Red Team":
                    RegisterNearbyAI(selfTeam, i);
                    break;
                case "Collectable":
                    if (i.transform.parent == null)
                    {
                        if ((i.transform.position - _AI.transform.position).magnitude <= GetCollectableRangeCap() && Time.realtimeSinceStartup > AIConstants.Defender.StartupIgnoreCollectableDuration)
                        {
                            if (!_aifsm._ignoredObjectList.Contains(i)) // if ignored, ignore
                            {
                                nearbyData.Collectable = new NearbyObjectData(true, i);
                            }
                            break;
                        }
                        else
                        {
                            if (!_aifsm._ignoredObjectList.Contains(i))
                            {
                                _aifsm._ignoredObjectList.Add(i, AIConstants.Global.IgnoreCollectableOutsideRangeCapDuration);
                            }
                            break;
                        }
                    }
                    break;
                case "Flag":
                    if (!IsSelfFlagAtOwnBase(i))
                    {
                        if (i.transform.parent == null)
                        {
                            nearbyData.Flag[nearbyData.Flag[0].IsSlotEmpty() ? 0 : 1] = new NearbyObjectData(true, i);
                            nearbyData.nearbyFlagCount += 1;
                        }
                    }
                    break;
                case "Base":
                    nearbyData.Base = new NearbyObjectData(true, i);
                    break;
            }


            LogText = _AI.gameObject.name.ToString() + " - Current Nearby Object Count : " + ObjectsInView.Count.ToString() + "\n"
            + nearbyData.nearbyEnemyCount.ToString() + " Enemies | " + nearbyData.nearbyFlagCount.ToString() + " Flags | " + nearbyData.nearbyAllyCount.ToString() + " Nearby Allies" + "\n"
            + "Nearby Data : \n" 
            + nearbyData.GetDataAsString() + "\n"
            + "Ignore List ("+_aifsm._ignoredObjectList.Count+") : \n"
            + _aifsm._ignoredObjectList.GetDataAsString();
            doh.SetSlotText(_AI.gameObject.name, LogText, _AI.AICol);
            LogText = "";
            
        }
    }

    private bool IsSelfFlagAtOwnBase(GameObject val)
    {
        if(FlagNameToTeam(val.name) == _AI._agentData.FriendlyTeam)
        {
            float dis = Mathf.Abs((_AI._agentData.FriendlyBase.transform.position - val.transform.position).magnitude);
            if (dis < 3f)
            {
                return true;
            }
        }
        return false;
    }
    public float GetCollectableRangeCap()
    {
        if(_aifsm._overrideRole == AIFSM.OverrideRole.None)
        {
            return (_aifsm._baseRole == AIFSM.BaseRole.Defender) ? AIConstants.Defender.PickupRangeRestriction : AIConstants.Attacker.PickupRangeRestriction;
        }
        return (_aifsm._overrideRole == AIFSM.OverrideRole.Protector) ? AIConstants.Protector.PickupRangeRestriction : AIConstants.Retriever.PickupRangeRestriction;
    }
    private void RegisterNearbyAI(AgentData.Teams selfTeam, GameObject TargetAI)
    {
        if (_aifsm._ignoredObjectList.Contains(TargetAI))// if ignored, ignore  FOR THE LONGEST FUCKING TIME THERE WAS A IF (IGNORELIST == 0 || IGNORELIST.CONTAINS TARGET) HERE AND I DONT KNOW WHY AND IT CAUSED ME HOURS OF PAIN WHAT THE FUCK IS WRONG WITH ME LIKE HOLY FUCK IT MADE ERVYTHING EB IGNORED OIF THE IGNORE LIST AS EMPTY
        {
            return;
        }

        if (selfTeam == TargetAI.GetComponent<AI>()._agentData.FriendlyTeam)
        {

            nearbyData.Ally[nearbyData.Ally[0].IsSlotEmpty() ? 0 : 1] = new NearbyObjectData(true, TargetAI);
            nearbyData.nearbyAllyCount += 1;
            return;
        }
        else
        {
            if (_aifsm._baseRole == AIFSM.BaseRole.Defender && _aifsm._overrideRole == AIFSM.OverrideRole.None)
            {
                if (GetYNegatedMagnitude(TargetAI, _AI._agentData.FriendlyBase) > AIConstants.Defender.EngagementRangeRestriction) 
                {
                    _aifsm._ignoredObjectList.Add(TargetAI, AIConstants.Global.IgnoreEnemyDuration);
                    return;
                } 
            }
            nearbyData.Enemy[nearbyData.Enemy[0].IsSlotEmpty() ? 0 : (nearbyData.Enemy[1].IsSlotEmpty()) ? 1 : 2] = new NearbyObjectData(true, TargetAI);
            nearbyData.nearbyEnemyCount += 1;
            return;
        }
    }
    public bool DecideChoice(CalculatorFunction function, GameObject Target, bool OutputToLog = false)
   {
        float bar = CalculateChance(_aifsm._baseRole, _aifsm._overrideRole, function, Target);
        float roll = UnityEngine.Random.Range(0f, 1f);
        if (OutputToLog)
        {
            Debug.Log(_AI._agentData.FriendlyTeam.ToString() + " Deciding, chose " + (roll < bar).ToString());
        }
        return roll < bar;
    }
    public float CalculateChance(AIFSM.BaseRole baseRole, AIFSM.OverrideRole overrideRole, CalculatorFunction function, GameObject Target)
    {
        // I like to call this a work of arse

        if (function == CalculatorFunction.Collectable)
        { 
            if(_AI._agentInventory.GetInventoryUsage() == _AI._agentInventory.Capacity)
            {
                return 0f;
            }
        }

        if(overrideRole == AIFSM.OverrideRole.None)
        {
            if(baseRole == AIFSM.BaseRole.Defender)
            {
                if(function == CalculatorFunction.Collectable)
                {
                    if (!_AI._agentInventory.HasItem(Target.name).owned) { return AIConstants.Defender.InitialCollectChance; }
                    return Mathf.Pow(AIConstants.Defender.RepeatCollectChance, AIConstants.Defender.OwnershipReducesCollectChance ? _AI._agentInventory.HasItem(Target.name).quantityOwned : 1);
                }
                else
                {
                    return AIConstants.Defender.EngagmentChance;
                }
            }
            else // attacker
            {
                if (function == CalculatorFunction.Collectable)
                {
                    if (_AI._agentInventory.HasItem(Target.name).owned) { return AIConstants.Attacker.InitialCollectChance; }
                    return Mathf.Pow(AIConstants.Attacker.RepeatCollectChance, AIConstants.Attacker.OwnershipReducesCollectChance ? _AI._agentInventory.HasItem(Target.name).quantityOwned : 1);
                }
                else
                {
                    return AIConstants.Attacker.EngagmentChance;
                }
            }
        }
        else
        {
            if (overrideRole == AIFSM.OverrideRole.Protector)
            {
                if (function == CalculatorFunction.Collectable)
                {
                    if (_AI._agentInventory.HasItem(Target.name).owned) { return AIConstants.Protector.InitialCollectChance; }
                    return Mathf.Pow(AIConstants.Protector.RepeatCollectChance, AIConstants.Protector.DoesOwnershipReduceCollectChance ? _AI._agentInventory.HasItem(Target.name).quantityOwned : 1);
                }
                else
                {
                    return AIConstants.Protector.EngagmentChance;
                }
            }
            else // retriever
            {
                if (function == CalculatorFunction.Collectable)
                {
                    if (_AI._agentInventory.HasItem(Target.name).owned) { return AIConstants.Retriever.InitialCollectChance; }
                    return Mathf.Pow(AIConstants.Retriever.RepeatCollectChance, AIConstants.Retriever.OwnershipReducesCollectChance ? _AI._agentInventory.HasItem(Target.name).quantityOwned : 1);
                }
                else
                {
                    return AIConstants.Retriever.EngagmentChance;
                }
            }
        }
    }
    public bool MoveToPosition(GameObject targetObject, float waitAtPositionDuration)
    {
        return (MoveToPosition(targetObject.transform.position, waitAtPositionDuration));
    }
    public bool MoveToPosition(Vector3 targetLocation, float waitAtPositionDuration)
    {
        if(nearbyData.nearbyEnemyCount > 0)
        {
            reachedTarget = true;
            timer = 0f;
        }

        if (!reachedTarget) // move to target if not at target
        {
            _AI._agentActions.MoveTo(targetLocation);
            timer = waitAtPositionDuration;
        }

        if (!reachedTarget && GetYNegatedMagnitude(targetLocation, _AI.transform.position) < AIConstants.Global.Leniency) // check if just reached target, should only fire once
        {
            reachedTarget = true;
        }

        if (reachedTarget && timer > 0f) // lazy timer
        {
            timer -= Time.deltaTime;
        }

        if (reachedTarget && timer <= 0f) // return true when reached location and waited for duration
        {
            reachedTarget = false;
            return true;
        }
        return false;
    }
    public AI.ExecuteResult GenerateResult(bool success)
    {
        returnResult.success = success;
        returnResult.jobTitle = jobName;
        return  returnResult;
    }
    public string GetName()
    {
        return jobName;
    }
    protected GameObject GetFlagByPriority()
    {
        string selfFlag = (_AI._agentData.FriendlyTeam == AgentData.Teams.BlueTeam) ? "Blue Flag" : "Red Flag";
        string enemyFlag = (_AI._agentData.FriendlyTeam == AgentData.Teams.BlueTeam) ? "Red Flag" : "Blue Flag";
        // prioritises Own Flag > Enemy Flag
        int newTarget = 0;

        for (int i = 0; i < nearbyData.nearbyFlagCount; i++)
        {
            if (nearbyData.Flag[i].targetGameObject.name == selfFlag)
            {
                return nearbyData.Flag[i].targetGameObject;
            }
            else
            {
                newTarget = i;
            }
        }

        return nearbyData.Flag[newTarget].targetGameObject;
    }
    protected GameObject GetFlagHolderIfPresent() // exists to prioritise nearby enemies by whether they have the flag or not
    {
        string selfFlag = (_AI._agentData.FriendlyTeam == AgentData.Teams.BlueTeam) ? "Blue Flag" : "Red Flag";
        string enemyFlag = (_AI._agentData.FriendlyTeam == AgentData.Teams.BlueTeam) ? "Red Flag" : "Blue Flag";
        // prioritises Own Flag > Enemy Flag > No Flag

        int newTarget = 0; // default selection is first in list, if this method has been called, 0 is always occupied
        for (int i = 0; i < nearbyData.nearbyEnemyCount; i++)
        {
            if (nearbyData.Enemy[i].targetGameObject.GetComponent<AI>()._agentInventory.HasItem(selfFlag).owned) // if holding own teams flag
            {
                return nearbyData.Enemy[i].targetGameObject; // immediate return, no point in checking anythig else
            }
            else if (nearbyData.Enemy[i].targetGameObject.GetComponent<AI>()._agentInventory.HasItem(enemyFlag).owned) // if holding enemy teams flag
            {
                newTarget = i; // override default selection to holder of enemy flag, but still checks rest of list
            }
        }
        return nearbyData.Enemy[newTarget].targetGameObject; // returns enemy flag holder if found, slot 0 if not
    }
    protected string TeamToFlagName(AgentData.Teams val)
    {
        if (val == AgentData.Teams.BlueTeam)
        {
            return "Blue Flag";
        }
        else
        {
            return "Red Flag";
        }
    }
    protected AgentData.Teams FlagNameToTeam(string val)
    {
        if(val == "Blue Flag")
        {
            return AgentData.Teams.BlueTeam;
        }
        if(val == "Red Flag")
        {
            return AgentData.Teams.RedTeam;
        }
        throw new Exception(val + " is an invalid flag name");
    }
    public float GetYNegatedMagnitude(Vector3 Target, Vector3 CurrentPosition) // exists because i want to check how close the ai is to the intended target, y coord doesnt matter in this case
    {
        Target.y = 0;
        CurrentPosition.y = 0;
        return (Target - CurrentPosition).magnitude;
    }
    public float GetYNegatedMagnitude(GameObject TargetObject, Vector3 CurrentPosition) // exists because i want to check how close the ai is to the intended target, y coord doesnt matter in this case
    {
        if(TargetObject == null)
        {
            return -1;
        }
        Vector3 TargetPos = TargetObject.transform.position;
        TargetPos.y = 0;
        CurrentPosition.y = 0;
        return (TargetPos - CurrentPosition).magnitude;
    }
    public float GetYNegatedMagnitude(GameObject TargetObject, GameObject OtherObject) // exists because i want to check how close the ai is to the intended target, y coord doesnt matter in this case
    {
        if (TargetObject == null)
        {
            return -1;
        }
        if (OtherObject == null)
        {
            return -1;
        }
        Vector3 TargetPos = TargetObject.transform.position;
        Vector3 OtherPos = OtherObject.transform.position;
        TargetPos.y = 0;
        OtherPos.y = 0;
        return (TargetPos - OtherPos).magnitude;
    }
    public float GetYNegatedMagnitude(GameObject TargetObject) // exists because i want to check how close the ai is to the intended target, y coord doesnt matter in this case
    {
        if (TargetObject == null)
        {
            return -1;
        }
        Vector3 TargetPos = TargetObject.transform.position;
        Vector3 CurrPos = _AI.transform.position;
        TargetPos.y = 0;
        CurrPos.y = 0;
        return (TargetPos - CurrPos).magnitude;
    }
    
    public virtual void HasTakenDamage()
    {
        //UpdateVision();
    }
}
