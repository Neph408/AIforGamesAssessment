using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AIFSM;
using static BehaviourStateTemplate;

public class AIFSM
{
    public enum BaseRole
    {
        Defender,
        Attacker
    }
    public enum OverrideRole
    {
        None,
        Protector,
        Retriever
    }

    public class IgnoredObjectData
    {
        public GameObject gameObject;
        public float timestampOfAcknowledge;

        public IgnoredObjectData(GameObject _gameObject, float _timestampOfAcknowledge)
        {
            gameObject =_gameObject;
            timestampOfAcknowledge = _timestampOfAcknowledge;
        }
    }
    public class IgnoredObjectList
    {
        public List<IgnoredObjectData> IgnoredCollectables;

        public IgnoredObjectList()
        {
            IgnoredCollectables = new List<IgnoredObjectData>();
        }
        public int Count
        {
            get { return IgnoredCollectables.Count; }
        }

        public void Add(GameObject target, float duration)
        {
            IgnoredCollectables.Add(new IgnoredObjectData(target, Time.time + duration));
        }

        public void Remember(int target)
        {
            IgnoredCollectables.RemoveAt(target);
        }
        public bool Contains(GameObject go)
        {
            if (IgnoredCollectables.Count == 0)
            {
                return false;
            }
            foreach (var ignoredObject in IgnoredCollectables)
            {
                if (ignoredObject.gameObject == go)
                {
                    return true;
                }
            }
            return false;
        }

        public void Update()
        {
            if(IgnoredCollectables.Count > 0)
            {
                for (int i = 0; i < IgnoredCollectables.Count; i++)
                {
                    if (Time.time > IgnoredCollectables[i].timestampOfAcknowledge)
                    {
                        Remember(i);
                    }
                }
            }
        }
    }

    private AI OwnerAI = null;

    public IgnoredObjectList _ignoredObjectList;


    private BehaviourStateTemplate CurrentState;

    public BaseRole _baseRole;
    public OverrideRole _overrideRole;

    private AI.ExecuteResult failureState;

    public AIFSM(AI playerAI)
    {
        OwnerAI = playerAI;

        _ignoredObjectList = new IgnoredObjectList();

        _overrideRole = OverrideRole.None; // for safety, should ever be not none by default

        failureState.success = false;
        failureState.jobTitle = "No Assigned Behaviour";

        SetupFSM();
    }
    private void SetupFSM()
    {
        SetCurrentState(new BH_StartState(this));
    }

    public bool HasCurrentState()
    {
        return CurrentState != null;
    }

    public BehaviourStateTemplate GetCurrentState()
    {
        return CurrentState;
    }

    public void SetCurrentState(BehaviourStateTemplate newBST)
    {
        if(CurrentState != null) {CurrentState.OnExit();}
        OwnerAI.currentJob = newBST.GetName();
        CurrentState = newBST;
        CurrentState.OnEntry();
    }

    public void WipeCurrentState()
    {
        CurrentState.OnExit();
        CurrentState = null;
    }

    public AI.ExecuteResult FSMUpdate()
    {
        _ignoredObjectList.Update();
        if(CurrentState != null)
        {
            return CurrentState.Execute(); ;
        }
        return failureState;
    }

    public AI GetOwnerAI()
    {
        return OwnerAI;
    }
}
