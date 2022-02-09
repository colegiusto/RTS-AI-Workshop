using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AI : MonoBehaviour
{
    public List<Unit> units;


    List<Unit> selected;
    List<SquadronAI> squads;
    StateManager stateMachine;
    float squadsize;
    int gold;

    public StateData state
    {
        get
        {
           return new StateData(new Dictionary<StateParameter, float>
           {
               {StateParameter.Gold, gold },
           });
        }
    }
    void Awake()
    {
        gold = 0;
        selected = new List<Unit>();
        squads = new List<SquadronAI>();
        stateMachine = new StateManager(this, state, StateData.Win);
    }

    // Update is called once per frame
    void Update()
    {
        
        MakeDecision();
    }

    void AllocateSelected()
    {
        foreach(SquadronAI squad in squads)
        {
            if (squad.value > squadsize)
            {
                continue;
            }
            foreach (Unit u in units)
            {
                if (u.assigned)
                {
                    selected.Remove(u);
                    continue;
                }
                    
                squad.AddUnit(u);
                if (squad.value > squadsize)
                {
                    continue;
                }
            }

        }
        
    }

    void MakeDecision()
    {

    }
    void CheckClick()
    {
        if (!Input.GetMouseButtonDown(0)) 
        {
            return;
        }
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        print(mousePos);
        if (selected.Count > 0)
        {
            PathSetter p = selected[0].GetPathSetter(mousePos);
            //print(p.cost);
            p.Set();
        }
        selected.Add(units[0]);
        

    }

    
    public class SquadronAI
    {
        List<Unit> Units;
        public List<Unit> units
        {
            get => Units;
            set
            {
                foreach(Unit u in value)
                {
                    AddUnit(u);
                }
            }
        }
        public float value
        {
            get
            {
                float v = 0;
                foreach(Unit u in units)
                {
                    v += u.value;
                }
                return v;
            }
        }

        public SquadActions action
        {
            get;
            set;
        }

        AI ai;
        public SquadronAI(List<Unit> units, AI ai)
        {
            this.units = units;
            this.ai = ai;
        }

        public void Update()
        {
            if(!(units?.Count > 0))
            {
                ai.squads.Remove(this);
                return;
            }
        }

        public void AddUnit(Unit unit)
        {
            units.Add(unit);
            unit.assigned = true;
        }
    }

    public enum SquadActions
    {
        Move,
    }
}

public class StateManager : MonoBehaviour
{
    public StateData currentState;
    List<StrategyAction> strategies;
    StateData goalState;

    List<Tactic> tactics;
    AI ai;
    public StateManager(AI _ai, StateData _currentState, StateData _goalState)
    {
        ai = _ai;
        goalState = _goalState;
        currentState = _currentState;
    }

    private void ActionPlan()
    {
        currentState.ChangeState(goalState);
    }
}
public class AIAction
{
    
    AI ai;
    PreConditions preConditions;

    public StateData change;


    public AIAction(AI _ai, StateData _change, PreConditions conditions = null)
    {
        preConditions = conditions;
        ai = _ai;
        change = _change;
    }

    public bool CanPreform(StateData current)
    {
        return preConditions.Satisfied(current);
    }

}

public class StrategyAction : AIAction
{
    public StrategyAction(AI _ai, StateData _change, PreConditions conditions = null) : base(_ai, _change, conditions)
    {
        
    }
}
public class Tactic: AIAction
{
    public Tactic(AI _ai, StateData _change, PreConditions conditions = null) : base(_ai, _change, conditions)
    {

    }
}

//public struct StateData
//{
//    public Dictionary<StateParameter, float> stateValues;

//    public StateData(Dictionary<StateParameter, float> _stateValues)
//    {
//        stateValues = _stateValues;
//    }
//    public StateData ChangeState(Dictionary<StateParameter, float> change)
//    {
//        StateData lData = this;
//        foreach(StateParameter p in change.Keys)
//        {
//            stateValues[p] += change[p];
//        }
//        return lData;
//    }

//    public StateData ChangeState(StateData change)
//    {
//        return ChangeState(change.stateValues);
//    }
//    public StateData GetChange(Dictionary<StateParameter, float> target)
//    {
//        StateData lData = new StateData(new Dictionary<StateParameter, float>());

//        foreach (StateParameter p in target.Keys)
//        {
//            lData.stateValues[p] = target[p]-stateValues[p];
//        }
//        return lData;
//    }

//    public StateData GetChange(StateData target)
//    {
//        return GetChange(target.stateValues);
//    }
//    public static StateData Win
//    {
//        get => new StateData(new Dictionary<StateParameter, float>
//        {
//            { StateParameter.Gold , 16 },
//        });
//    }
//}

public class PreConditions
{
    Dictionary<StateParameter, Condition> conditions;

    public static PreConditions None = new PreConditions();
    
    public bool Satisfied(StateData current)
    {
        foreach(KeyValuePair<StateParameter,Condition> k in conditions)
        {
            if (!k.Value.Satisfied(current.stateValues[k.Key]))
            {
                return false;
            }
        }
        return true;
    }

    public Dictionary<StateParameter, (float, float)> ChangesNeeded(StateData current)
    {
        Dictionary<StateParameter, (float, float)> changes = new Dictionary<StateParameter, (float, float)>();
        foreach (KeyValuePair<StateParameter, Condition> k in conditions)
        {
            if (!k.Value.Satisfied(current.stateValues[k.Key]))
            {
                changes.Add(k.Key, (current.stateValues[k.Key], k.Value.target)) ;
            }
        }
        return changes;
        
    }

    
}

public class Condition
{
    public float target;
    delegate bool ConditionCheck(float value);
    ConditionCheck check;
    private Condition(float _param, ConditionCheck _check)
    {
        target = _param;
        check = _check;
    }
    public bool Satisfied(float value)
    {
        return check(value);
    }



    public static Condition LessThan(float p)
    {
        
        ConditionCheck c = new ConditionCheck((float v) => v <= p);
        return new Condition(p, c);
    }
    public static Condition MoreThan(float p)
    {

        ConditionCheck c = new ConditionCheck((float v) => v >= p);
        return new Condition(p, c);
    }
}


public enum StateParameter
{
    Gold,
    TroopValue,
}

