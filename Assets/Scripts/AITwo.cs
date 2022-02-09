using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

class AITwo : MonoBehaviour
{
    List<Unit> enemies;
    public Unit GetEnemyInRange(Vector3 position, float radius)
    {
        float s = radius + 1;
        Unit unit = null;
        foreach(Unit u in enemies)
        {
            float distance = (u.transform.position - position).magnitude;
            if ( distance <= s)
            {
                s = distance;
                unit = u;
            }
        }
        if(s< radius)
        {
            return unit;
        }
        return null;
    }
}
public struct StateData
{
    public Dictionary<StateParameter, float> stateValues;

    public StateData(Dictionary<StateParameter, float> _stateValues)
    {
        stateValues = _stateValues;
    }
    public StateData ChangeState(Dictionary<StateParameter, float> change)
    {
        StateData lData = this;
        foreach (StateParameter p in change.Keys)
        {
            stateValues[p] += change[p];
        }
        return lData;
    }

    public StateData ChangeState(StateData change)
    {
        return ChangeState(change.stateValues);
    }
    public StateData GetChange(Dictionary<StateParameter, float> target)
    {
        StateData lData = new StateData(new Dictionary<StateParameter, float>());

        foreach (StateParameter p in target.Keys)
        {
            lData.stateValues[p] = target[p] - stateValues[p];
        }
        return lData;
    }

    public StateData GetChange(StateData target)
    {
        return GetChange(target.stateValues);
    }
    public static StateData Win
    {
        get => new StateData(new Dictionary<StateParameter, float>
        {
            { StateParameter.Gold , 16 },
        });
    }
}

