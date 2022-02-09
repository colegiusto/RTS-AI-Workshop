using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region instancevars
    [SerializeField] GameObject selected;
    [SerializeField] float speed, lockRange, attackRange;
    LinkedList<Vector3> path;
    Unit target;
    Vector3 moveTarget;
    AITwo ai;

    [HideInInspector]
    public float value;
    [HideInInspector]
    public bool assigned;

    UnitState state;
    public UnitState State
    {
        get => state;
        set
        {
            state = value;
            switch (state)
            {
                case UnitState.Idle:
                    target = null;
                    path = null;
                    break;
                case UnitState.MoveTo:
                    target = null;
                    break;
                case UnitState.Attack:
                    break;
                case UnitState.AttackMove:
                    target = null;
                    break;
                default:
                    break;
            }
        }
    }
    #endregion instancevars

    public Unit(float value)
    {
        this.value = value;
        this.assigned = false;
        this.State = UnitState.Idle;
    }
    void Start()
    {
        
    }

    void Update()
    {
        switch (State)
        {
            case UnitState.Idle:
                break;
            case UnitState.MoveTo:
                PathMove();
                break;
            case UnitState.Attack:
                AttackChase();
                break;
            case UnitState.AttackMove:
                PathMove();
                break;
            default:
                break;
        }

    }

    void AIUpdate()
    {
        State = TransitionCheck();
    }
    public void ForceTransition(UnitState _state)
    {
        State = _state;
    }
    UnitState TransitionCheck()
    {
        switch (State)
        {
            case UnitState.MoveTo:
                break;
            case UnitState.Idle:
                break;
            
            case UnitState.Attack:
                if(!(target != null && (target.transform.position - transform.position).magnitude < 1))
                {
                    return UnitState.Idle;
                }
                break;
            case UnitState.AttackMove:
               
                if (EnemyCheck(out Unit u))
                {
                    target = u;
                    return UnitState.AttackChase;
                }
                break;
            default:
                break;
        }
        return State;
    }
    bool EnemyCheck(out Unit u)
    {
        Unit un = ai.GetEnemyInRange(transform.position, lockRange);
        if (un != null)
        {
            u = un;
            return true;
        }
        u = null;
        return false;
    }

    void PathMove()
    {
        if (!(path?.Count>0))
        {
            GetPathSetter(moveTarget).Set();
        }
        if ((path.First.Value - transform.position).magnitude < .2f)
        {
            path.RemoveFirst();
            if (path.Count<1)
            {
                return;
            }
        }
        
        transform.Translate((path.First.Value-transform.position).normalized*speed*Time.deltaTime);
    }
    void AttackChase()
    {
        if (!(path?.Count > 0))
        {
            GetPathSetter(target.transform.position).Set();
        }
        if ((path.First.Value - transform.position).magnitude < .2f)
        {
            path.RemoveFirst();
            if (path.Count < 1)
            {
                return;
            }
        }
        if((path.First.Value - transform.position).magnitude < attackRange)
        {
            State = UnitState.Attack;
            return;
        }
        transform.Translate((path.First.Value - transform.position).normalized * speed * Time.deltaTime);

    }

    public PathSetter GetPathSetter(Vector2 target)
    {
        Vector3 target3 = new Vector3(target.x,target.y, transform.position.z);
        LinkedList <Vector3> lpath = new LinkedList<Vector3>();
        float cost = 0;

        lpath.AddLast(target3 - transform.position);
        cost += (target3 - transform.position).magnitude;
        return new PathSetter(
            cost,
            delegate
            {
                path = lpath;
            }
            );
    }
}
public struct PathSetter
{
    public float cost;
    public Action setter;

    public PathSetter(float cost, Action setter)
    {
        this.cost = cost;
        this.setter = setter;
    }

    public void Set()
    {
        setter();
    }
}

public enum UnitState
{
    Idle,
    MoveTo, 
    AttackChase,
    AttackMove,
    Attack
}