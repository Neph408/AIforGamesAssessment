using JetBrains.Annotations;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;
using TMPro.EditorUtilities;
using UnityEngine;
using static AIConstants;
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

    public virtual void RetrieverTakenDamage(GameObject attacker)
    {
        // does nothing for non protectors
        Debug.LogError("Retriver Alert sent to non-protector");
    }

    public virtual void UpdateVision(bool OutputToLog = true)
    {
        AgentData.Teams selfTeam = _AI._agentData.FriendlyTeam;

        ObjectsInView = _AI._agentSenses.GetObjectsInView();
        nearbyData.ClearData(); // clears to not retain outdated info
        /*
         
        why did i not just have a bunch of virtuals that get overridden in behaviour methods
        this wouldve been so much cleaner
        like
        see flag on ground
        see enemy holding flag
        see enemy
        see ally retriever
        
        this wouldve been so much cleaner

         */

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
                    if (!FlagAtOwnBase(i))
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

            UpdateDebugOverlay();
        }
    }

    private void UpdateDebugOverlay()
    {
        string LogText;
        string roleText = "-"; 
        if(_aifsm._overrideRole == AIFSM.OverrideRole.None)
        {
            roleText = _aifsm._baseRole == AIFSM.BaseRole.Defender ? "Defender" : "Attacker";
        }
        else
        {
            roleText = _aifsm._overrideRole == AIFSM.OverrideRole.Protector ? "Protector" : "Retriever";
        }

        LogText = _AI.gameObject.name.ToString() +" | Role : "+ roleText + " | " + jobName + "\n"
            + "Current Nearby Object Count : " + ObjectsInView.Count.ToString() + "\n"
            + nearbyData.nearbyEnemyCount.ToString() + " Enemies | " + nearbyData.nearbyFlagCount.ToString() + " Flags | " + nearbyData.nearbyAllyCount.ToString() + " Nearby Allies" + "\n"
            + nearbyData.nearbyEnemyHoldingFlag.ToString() + " Enemies W/ Flag | " + nearbyData.nearbyAllyHoldingFlag.ToString() + " Nearby Allies W/ Flag" + "\n"
            + "Nearby Data : \n" 
            + nearbyData.GetDataAsString() + "\n"
            + "Ignore List ("+_aifsm._ignoredObjectList.Count+") : \n"
            + _aifsm._ignoredObjectList.GetDataAsString();
            doh.SetSlotText(_AI.gameObject.name, LogText, _AI.AICol);
        LogText = "";
    }

    protected bool HealthConsumableCheck()
    {
        if(!_AI._agentInventory.HasItem("Health Kit").owned)
        {
            return false;
        }

        if(_aifsm._overrideRole == AIFSM.OverrideRole.None)
        {
            if(_aifsm._baseRole == AIFSM.BaseRole.Defender)
            {
                if(_AI._agentData.CurrentHitPoints < AIConstants.Defender.HealthToHeal)
                {
                    _AI._agentActions.UseItem(_AI._agentInventory.GetItem("Health Kit"));
                    return true;
                }
            }
            else
            {
                if (_AI._agentData.CurrentHitPoints < AIConstants.Attacker.HealthToHeal)
                {
                    _AI._agentActions.UseItem(_AI._agentInventory.GetItem("Health Kit"));
                    return true;
                }
            }
        }
        else
        {
            if(_aifsm._overrideRole == AIFSM.OverrideRole.Retriever)
            {
                if (_AI._agentData.CurrentHitPoints < AIConstants.Retriever.HealthToHeal)
                {
                    _AI._agentActions.UseItem(_AI._agentInventory.GetItem("Health Kit"));
                    return true;
                }
            }
            else
            {
                if (_AI._agentData.CurrentHitPoints < AIConstants.Protector.HealthToHeal)
                {
                    _AI._agentActions.UseItem(_AI._agentInventory.GetItem("Health Kit"));
                    return true;
                }
            }
        }
        return false;
    }

    protected bool PowerupConsumableCheck(GameObject CurrentAttackingTarget)
    {
        AI CurrentAttackingAI = CurrentAttackingTarget.GetComponent<AI>();
        if (!_AI._agentInventory.HasItem("Power Up").owned || _AI._agentData.IsPoweredUp)
        {
            return false;
        }

        if (_aifsm._overrideRole == AIFSM.OverrideRole.None)
        {
            if(CurrentAttackingAI._agentData.HasFriendlyFlag || CurrentAttackingAI._agentData.HasEnemyFlag) // go for murder (try to kill flag holder no matter what)
            {
                _AI._agentActions.UseItem(_AI._agentInventory.GetItem("Power Up"));
                return true;
            }
            if (_aifsm._baseRole == AIFSM.BaseRole.Defender)
            {
                if (_AI._agentData.CurrentHitPoints < AIConstants.Defender.HealthToPowerUp || (CurrentAttackingAI._agentData.CurrentHitPoints - _AI._agentData.CurrentHitPoints) > AIConstants.Defender.HealthDeltaToPowerUp)
                {
                    _AI._agentActions.UseItem(_AI._agentInventory.GetItem("Power Up"));
                    return true;
                }
            }
            else
            {
                if (_AI._agentData.CurrentHitPoints < AIConstants.Attacker.HealthToPowerUp || (CurrentAttackingAI._agentData.CurrentHitPoints - _AI._agentData.CurrentHitPoints) > AIConstants.Attacker.HealthDeltaToPowerUp)
                {
                    _AI._agentActions.UseItem(_AI._agentInventory.GetItem("Power Up"));
                    return true;
                }
            }
        }
        else
        {
            if (_aifsm._overrideRole == AIFSM.OverrideRole.Retriever)
            {
                if (_AI._agentData.CurrentHitPoints < AIConstants.Retriever.HealthToPowerUp || (CurrentAttackingAI._agentData.CurrentHitPoints - _AI._agentData.CurrentHitPoints) > AIConstants.Retriever.HealthDeltaToPowerUp)
                {
                    _AI._agentActions.UseItem(_AI._agentInventory.GetItem("Power Up"));
                    return true;
                }
            }
            else
            {
                if (_AI._agentData.CurrentHitPoints < AIConstants.Protector.HealthToPowerUp || (CurrentAttackingAI._agentData.CurrentHitPoints - _AI._agentData.CurrentHitPoints) > AIConstants.Protector.HealthDeltaToPowerUp)
                {
                    _AI._agentActions.UseItem(_AI._agentInventory.GetItem("Power Up"));
                    return true;
                }
            }
        }
        return false;
    }

    private bool FlagAtOwnBase(GameObject targetFlag)
    {
        if (GetYNegatedMagnitude(targetFlag, _AI._agentData.FriendlyBase) < 3f)
        {
            return true;
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
    private void RegisterNearbyAI(AgentData.Teams selfTeam, GameObject TargetObject)
    {
        if (_aifsm._ignoredObjectList.Contains(TargetObject))// if ignored, ignore  FOR THE LONGEST FUCKING TIME THERE WAS A IF (IGNORELIST == 0 || IGNORELIST.CONTAINS TARGET) HERE AND I DONT KNOW WHY AND IT CAUSED ME HOURS OF PAIN WHAT THE FUCK IS WRONG WITH ME LIKE HOLY FUCK IT MADE ERVYTHING EB IGNORED OIF THE IGNORE LIST AS EMPTY
        {
            return;
        }

        if (selfTeam == TargetObject.GetComponent<AI>()._agentData.FriendlyTeam)
        {

            nearbyData.Ally[nearbyData.Ally[0].IsSlotEmpty() ? 0 : 1] = new NearbyObjectData(true, TargetObject);
            nearbyData.nearbyAllyCount += 1;
            if(TargetObject.GetComponent<AI>()._agentData.HasFriendlyFlag || TargetObject.GetComponent<AI>()._agentData.HasEnemyFlag)
            {
                nearbyData.nearbyAllyHoldingFlag += 1;
            }
            return;
        }
        else
        {
            if (_aifsm._baseRole == AIFSM.BaseRole.Defender && _aifsm._overrideRole == AIFSM.OverrideRole.None)
            {
                if (GetYNegatedMagnitude(TargetObject, _AI._agentData.FriendlyBase) > AIConstants.Defender.EngagementRangeRestriction ) 
                {
                    _aifsm._ignoredObjectList.Add(TargetObject, AIConstants.Global.IgnoreEnemyDuration);
                    return;
                } 
            }
            if (_aifsm._overrideRole == AIFSM.OverrideRole.Protector)
            {
                if (GetYNegatedMagnitude(TargetObject, _AI.transform.position) > AIConstants.Protector.EngagementRangeRestriction)
                {
                    _aifsm._ignoredObjectList.Add(TargetObject, AIConstants.Global.IgnoreEnemyDuration);
                    return;
                }
            }
            nearbyData.Enemy[nearbyData.Enemy[0].IsSlotEmpty() ? 0 : (nearbyData.Enemy[1].IsSlotEmpty()) ? 1 : 2] = new NearbyObjectData(true, TargetObject);
            nearbyData.nearbyEnemyCount += 1;
            if (TargetObject.GetComponent<AI>()._agentData.HasFriendlyFlag || TargetObject.GetComponent<AI>()._agentData.HasEnemyFlag)
            {
                nearbyData.nearbyEnemyHoldingFlag += 1;
            }
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
                    if (!_AI._agentInventory.HasItem(Target.name).owned) { return AIConstants.Attacker.InitialCollectChance; }
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
                    if (!_AI._agentInventory.HasItem(Target.name).owned) { return AIConstants.Protector.InitialCollectChance; }
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
                    if (!_AI._agentInventory.HasItem(Target.name).owned) { return AIConstants.Retriever.InitialCollectChance; }
                    return Mathf.Pow(AIConstants.Retriever.RepeatCollectChance, AIConstants.Retriever.OwnershipReducesCollectChance ? _AI._agentInventory.HasItem(Target.name).quantityOwned : 1);
                }
                else
                {
                    return AIConstants.Retriever.EngagmentChance;
                }
            }
        }
    }
    public bool MoveToPosition(GameObject targetObject, float waitAtPositionDuration = 0f)
    {
        return (MoveToPosition(targetObject.transform.position, waitAtPositionDuration));
    }
    public bool MoveToPosition(Vector3 targetLocation, float waitAtPositionDuration = 0f)
    {
        if(nearbyData.nearbyEnemyCount > 0) 
        {
            //reachedTarget = true;
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
        string displayJob = jobName;
        returnResult.success = success;
        if(displayJob.Contains("</color>"))
        {
            displayJob = displayJob.Replace("</color>", "");
            displayJob = displayJob.Split("<")[0] + displayJob.Split(">")[1]; 
        }
        returnResult.jobTitle = displayJob;
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
    protected GameObject GetAllyHoldingFlagByPriority()
    {
        GameObject enemyFlagHolder = nearbyData.Ally[0].targetGameObject;// default return val
        foreach (NearbyObjectData ally in nearbyData.Ally)
        {
            if(ally.targetGameObject.GetComponent<AI>()._agentData.HasFriendlyFlag) // oh,. hasflag exists. i gotta go change a lot of hasitem checks
            {
                return ally.targetGameObject;
            }
            if(ally.targetGameObject.GetComponent<AI>()._agentData.HasEnemyFlag)
            {
                enemyFlagHolder = ally.targetGameObject;
            }
        }
        return enemyFlagHolder;
    }
    protected GameObject GetFlagHolderIfPresent() // exists to prioritise nearby enemies by whether they have the flag or not
    {
        // prioritises Own Flag > Enemy Flag > No Flag

        int newTarget = 0; // default selection is first in list, if this method has been called, 0 is always occupied
        int newTargetPriority = 0; // holding enemy flag is 1 priority
        float newTargetDistance = 1000f; // only for 0 priority 
        for (int i = 0; i < nearbyData.nearbyEnemyCount; i++)
        {
            if (nearbyData.Enemy[i].targetGameObject.GetComponent<AI>()._agentData.HasFriendlyFlag) // if holding own teams flag
            {
                return nearbyData.Enemy[i].targetGameObject; // immediate return, no point in checking anythig else
            }
            else if (nearbyData.Enemy[i].targetGameObject.GetComponent<AI>()._agentData.HasEnemyFlag) // if holding enemy teams flag
            {
                newTarget = i; // override default selection to holder of enemy flag, but still checks rest of list
                newTargetPriority = 1;
            }
            else
            {
                if(newTargetPriority == 0)
                {
                    if(GetYNegatedMagnitude(nearbyData.Enemy[i].targetGameObject, _AI.transform.position) < newTargetDistance)
                    {
                        newTarget = i;
                        newTargetDistance = GetYNegatedMagnitude(nearbyData.Enemy[i].targetGameObject, _AI.transform.position);
                    }
                }
            }
        }
        return nearbyData.Enemy[newTarget].targetGameObject; // returns enemy flag holder if found, otherwise returns nearest normal enemy
    }
    protected GameObject GetNearestEnemy() // gets nearest enemy without refreshing vision
    {
        // prioritises Own Flag > Enemy Flag > No Flag

        int newTarget = 0; // default selection is first in list, if this method has been called, 0 is always occupied
        float newTargetDistance = 1000f;
        for (int i = 0; i < nearbyData.nearbyEnemyCount; i++)
        {
            if (GetYNegatedMagnitude(nearbyData.Enemy[i].targetGameObject, _AI.transform.position) < newTargetDistance)
            {
                newTarget = i;
                newTargetDistance = GetYNegatedMagnitude(nearbyData.Enemy[i].targetGameObject, _AI.transform.position);
            }
        }
        return nearbyData.Enemy[newTarget].targetGameObject; // returns enemy flag holder if found, otherwise returns nearest normal enemy
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
    #region gynm
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
    #endregion
    protected bool CheckForNearerEnemy(GameObject currentTarget, out GameObject newTarget)
    {
        float currentEngagmentDistanceDelta = 0f;

        if(_aifsm._overrideRole == AIFSM.OverrideRole.None)
        {
            currentEngagmentDistanceDelta = _aifsm._baseRole == AIFSM.BaseRole.Defender ? AIConstants.Defender.ReengagmentDistanceDelta : AIConstants.Attacker.ReengagmentDistanceDelta;
        }
        else
        {
            currentEngagmentDistanceDelta = _aifsm._overrideRole == AIFSM.OverrideRole.Retriever ? 10000f : AIConstants.Protector.ReengagmentDistanceDelta;
        }

        if(GetYNegatedMagnitude(currentTarget, _AI.transform.position) > currentEngagmentDistanceDelta)
        {
            newTarget = GetFlagHolderIfPresent(); // gets nearest enemy if doesnt find any flag holdesr
            return true;
        }
        newTarget = currentTarget;
        return false;
    }
    protected Vector3 GetRandomPositionTowards(GameObject targetPosition, float forcedforward = 2f)
    {
        return GetRandomPositionTowards(targetPosition.transform.position, forcedforward);
    }
    protected Vector3 GetRandomPositionTowards(Vector3 targetPosition,float forcedforward = 2f)
    {
        int safetybreak = 0;
        if (forcedforward > 0f) { forcedforward = 0f; }
        Vector3 retPos;
        do
        {
            retPos = _AI._agentActions.GetRandomDestination(10f);
            safetybreak += 1;
            if (safetybreak > 100) { break; }
        } while(GetYNegatedMagnitude(targetPosition, _AI.transform.position) - GetYNegatedMagnitude(targetPosition, retPos)  < forcedforward);

        return retPos;
    }


    public virtual void HasTakenDamage(GameObject attacker) { }
    public virtual void AddProtector(AI prot) { }
    public virtual bool HasProtector(AI prot) { return false; }
    public virtual void RemoveProtector(AI prot) { }



}
