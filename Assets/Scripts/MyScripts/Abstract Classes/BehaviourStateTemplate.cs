using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class BehaviourStateTemplate
{
    public class NearbyObjectData
    {
        public bool exists = false;
        public GameObject gameObject = null;

        public NearbyObjectData()
        {
            exists = false;
            gameObject = null;
        }
        public NearbyObjectData(bool isExists, GameObject targetGameObject)
        {
            exists = isExists;
            gameObject = targetGameObject;
        }

        public bool IsSlotEmpty()
        {
            if (gameObject == null) return true; // if it aint got a reference, there aint anything to talk about
            return false;
        }
    }
    public class NearbyData
    {
        public NearbyObjectData Collectable;
        public NearbyObjectData[] Enemy;
        public NearbyObjectData[] Ally;
        public NearbyObjectData[] Flag;
        public NearbyObjectData Base;

        public NearbyData()
        {
            Collectable = new NearbyObjectData();//big enough for the possibility to never happen of too many objs on floor
            Enemy = new NearbyObjectData[2];
            Ally = new NearbyObjectData[2];
            Flag = new NearbyObjectData[2];
            Base = new NearbyObjectData();
        }

        public void ClearData()
        {
            Collectable = new NearbyObjectData();
            Ally[0] = new NearbyObjectData();
            Ally[1] = new NearbyObjectData();
            Enemy[0] = new NearbyObjectData();
            Enemy[1] = new NearbyObjectData();
            Flag[0] = new NearbyObjectData();
            Flag[1] = new NearbyObjectData();
            Base = new NearbyObjectData();
        }
    }
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
    public AI.ExecuteResult returnResult;
    public NearbyData nearbyData;
    //public GameKnowledgeData gameKnowledgeData;
    private string LogText;


    protected List<GameObject> ObjectsInView;

    public BehaviourStateTemplate()
    {
        nearbyData = new NearbyData();
        nearbyData.ClearData();
    }

    public abstract void OnEntry();
    public abstract AI.ExecuteResult Execute();
    public abstract void OnExit();

    public virtual IEnumerator UpdateVision(float UpdateFrequency, bool OutputToLog = false)
    {
        AgentData.Teams selfTeam = _AI._agentData.FriendlyTeam;

        while (true)
        {
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
                        if((i.transform.position - _AI.transform.position).magnitude <= GetCollectableRangeCap())
                        {
                            if (!_aifsm._ignoredObjectList.Contains(i)) // if ignored, ignore
                            {
                                nearbyData.Collectable = new NearbyObjectData(true, i);
                            }
                            break;
                        }
                        else
                        {
                            if(!_aifsm._ignoredObjectList.Contains(i))
                            {
                                _aifsm._ignoredObjectList.Add(i, true);
                            } 
                            break;
                        }
                    case "Flag":
                        nearbyData.Flag[nearbyData.Flag[0].IsSlotEmpty() ? 0 : 1] = new NearbyObjectData(true, i);
                        break;
                    case "Base":
                        nearbyData.Base = new NearbyObjectData(true, i);
                        break;
                }
                
                if(OutputToLog)
                {
                    LogText += "Name - " + i.name + " : Distance - " + (Mathf.Round((i.transform.position - _AI.transform.position).magnitude * 10f) / 10f).ToString() + "m\n";
                }
            }

            if(OutputToLog)
            {
                LogText = _AI._agentData.FriendlyTeam.ToString() + " Team Defender - Current Nearby Object Count : " + ObjectsInView.Count.ToString() + "\n" + LogText;
                Debug.Log(LogText);
                LogText = "";
            }

            yield return new WaitForSecondsRealtime(UpdateFrequency);
        }
    }

    public float GetCollectableRangeCap()
    {
        if(_aifsm._overrideRole == AIFSM.OverrideRole.None)
        {
            return (_aifsm._baseRole == AIFSM.BaseRole.Defender) ? Movement.DefenderPickupRangeRestriction : Movement.AttackerPickupRangeRestriction;
        }
        return (_aifsm._overrideRole == AIFSM.OverrideRole.Protector) ? Movement.ProtectorPickupRangeRestriction : Movement.RetrieverPickupRangeRestriction;
    }
    private void RegisterNearbyAI(AgentData.Teams selfTeam, GameObject TargetAI)
    {
        if (_aifsm._ignoredObjectList.Count == 0 || _aifsm._ignoredObjectList.Contains(TargetAI))// if ignored, ignore
        {
            return;
        }

        if (selfTeam == TargetAI.GetComponent<AI>()._agentData.FriendlyTeam)
        {

            nearbyData.Ally[nearbyData.Ally[0].IsSlotEmpty() ? 0 : 1] = new NearbyObjectData(true, TargetAI);
            return;
        }
        else
        {
            nearbyData.Enemy[nearbyData.Enemy[0].IsSlotEmpty() ? 0 : 1] = new NearbyObjectData(true, TargetAI);
            return;
        }
    }
    public bool DecideChoice(AIFSM.BaseRole baseRole, AIFSM.OverrideRole overrideRole, CalculatorFunction function, GameObject Target)
    {
        float bar = CalculateChance(baseRole, overrideRole, function, Target);
        float roll = UnityEngine.Random.Range(0f, 1f);
        Debug.Log(roll.ToString() + " " + bar.ToString());
        return roll < bar;
    }
    public float CalculateChance(AIFSM.BaseRole baseRole, AIFSM.OverrideRole overrideRole, CalculatorFunction function, GameObject Target)
    {
        // I like to call this a work of arse
        if(overrideRole == AIFSM.OverrideRole.None)
        {
            if(baseRole == AIFSM.BaseRole.Defender)
            {
                if(function == CalculatorFunction.Collectable)
                {
                    if (!_AI._agentInventory.HasItem(Target.name).owned) { return Movement.DefenderInitialCollectChance; }
                    return Mathf.Pow(Movement.DefenderRepeatCollectChance, Movement.DefenderDoesOwnershipReduceCollectChance ? _AI._agentInventory.HasItem(Target.name).quantityOwned : 1);
                }
                else
                {
                    return Movement.DefenderEngagmentChance;
                }
            }
            else // attacker
            {
                if (function == CalculatorFunction.Collectable)
                {
                    if (_AI._agentInventory.HasItem(Target.name).owned) { return Movement.AttackerInitialCollectChance; }
                    return Mathf.Pow(Movement.AttackerRepeatCollectChance, Movement.AttackerDoesOwnershipReduceCollectChance ? _AI._agentInventory.HasItem(Target.name).quantityOwned : 1);
                }
                else
                {
                    return Movement.AttackerEngagmentChance;
                }
            }
        }
        else
        {
            if (overrideRole == AIFSM.OverrideRole.Protector)
            {
                if (function == CalculatorFunction.Collectable)
                {
                    if (_AI._agentInventory.HasItem(Target.name).owned) { return Movement.ProtectorInitialCollectChance; }
                    return Mathf.Pow(Movement.ProtectorRepeatCollectChance, Movement.ProtectorDoesOwnershipReduceCollectChance ? _AI._agentInventory.HasItem(Target.name).quantityOwned : 1);
                }
                else
                {
                    return Movement.ProtectorEngagmentChance;
                }
            }
            else // retriever
            {
                if (function == CalculatorFunction.Collectable)
                {
                    if (_AI._agentInventory.HasItem(Target.name).owned) { return Movement.RetrieverInitialCollectChance; }
                    return Mathf.Pow(Movement.RetrieverRepeatCollectChance, Movement.RetrieverDoesOwnershipReduceCollectChance ? _AI._agentInventory.HasItem(Target.name).quantityOwned : 1);
                }
                else
                {
                    return Movement.RetrieverEngagmentChance;
                }
            }
        }
    }
    public string GetName()
    {
        return jobName;
    }
}
