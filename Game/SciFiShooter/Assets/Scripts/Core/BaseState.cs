using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    protected GameObject _actor;

   
    protected IBehaviour _behaviour;
    protected object[] @params;


    public BaseState(GameObject actor)
    {
        this._actor = actor;
    }

    public void SetBehaviour(IBehaviour behaviour,params object[] p)
    {
        this._behaviour = behaviour;
        this.@params = p;
    }

    public virtual void Enter()
    {
        if (_behaviour != null) _behaviour.OnEnter(this._actor,@params);
    }

    public virtual void Exit()
    {

    }

    public virtual void Update()
    {
        if (IsValidateState) _behaviour.Update(_actor);

    }

    public virtual void FixedUpdate()
    {
        if (IsValidateState) _behaviour.FixedUpdate(_actor);

    }

    public virtual void LateUpdate()
    {
        if (IsValidateState) _behaviour.LateUpdate(_actor);
    }

    
    protected bool IsValidateState { get { return _behaviour != null; } }
    
}
