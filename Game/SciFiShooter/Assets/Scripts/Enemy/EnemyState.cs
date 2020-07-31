using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : BaseState
{
    protected EnemyController _enemyController;
    public EnemyState(GameObject actor) : base(actor)
    {
        this._enemyController = actor.GetComponent<EnemyController>();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void Update()
    {
        base.Update();
    }
}

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(GameObject enemyController) : base(enemyController)
    {
    }


    public override void Enter()
    {
        base.Enter();
        this._enemyController.enemyAnimController.Running = false;
        this._enemyController.enemyAnimController.Attacking = false;
        Debug.Log("On Idle");
    }

    public override void Exit()
    {
        
    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!IsValidateState) _enemyController.GoToState(_enemyController.moveState);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (!IsValidateState) _enemyController.GoToState(_enemyController.moveState);
    }

    public override void Update()
    {
        base.Update();
        if (!IsValidateState) _enemyController.GoToState(_enemyController.moveState);
    }
}

public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(GameObject enemyController) : base(enemyController)
    {
    }


    public override void Enter()
    {
        base.Enter();
        this._enemyController.enemyAnimController.Running = true;
        this._enemyController.enemyAnimController.Attacking = false;
        Debug.Log("On Move");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!IsValidateState) _enemyController.GoToState(_enemyController.attackState);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (!IsValidateState) _enemyController.GoToState(_enemyController.attackState);
    }

    public override void Update()
    {
        base.Update();
        if (!IsValidateState) _enemyController.GoToState(_enemyController.attackState);
    }
}

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(GameObject enemyController) : base(enemyController)
    {
    }

    public override void Enter()
    {
        base.Enter();
        this._enemyController.enemyAnimController.Attacking = true;
        Debug.Log("On Attack");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!IsValidateState) _enemyController.GoToState(_enemyController.idleState);

    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (!IsValidateState) _enemyController.GoToState(_enemyController.idleState);

    }

    public override void Update()
    {
        base.Update();
        if (!IsValidateState) _enemyController.GoToState(_enemyController.idleState);

    }
}


public class EnemySpawningState : EnemyState
{
    public EnemySpawningState(GameObject actor) : base(actor)
    {
    }

    public override void Enter()
    {
        base.Enter();
        this._enemyController.enemyRenderState.SetActiveAppearFx(true);
        
        DOVirtual.Float(0, 1, 0.5f, (percent) =>
        {
            this._enemyController.enemyRenderState.SetAppearProgress(percent);
        }).onComplete=()=>
        {
            this._enemyController.GoToState(this._enemyController.idleState);
            this._enemyController.enemyRenderState.SetActiveAppearFx(false);
        };
    }
}

public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(GameObject actor) : base(actor)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }
}