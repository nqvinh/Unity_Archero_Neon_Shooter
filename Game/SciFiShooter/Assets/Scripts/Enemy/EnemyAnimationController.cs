using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;

    enum AnimParam
    {
        Idle,
        Running,
        Attacking,
        Dead,
        COUNT
    }

    private Animator animator;
    public Animator Animator { get { if (animator == null) animator = GetComponentInChildren<Animator>(); return animator; } }

    public static int RunningID = Animator.StringToHash(AnimParam.Running.ToString());
    public static int AttackingID = Animator.StringToHash(AnimParam.Attacking.ToString());
    public static int DeadId = Animator.StringToHash(AnimParam.Dead.ToString());

    public bool Running
    {
        set
        {
            Animator.SetBool(RunningID, value);
        }
    }

    public bool Attacking
    {
        set
        {
            Animator.SetBool(AttackingID, value);
        }
    }

    public bool Dead
    {
        set
        {
            if (value) Animator.SetTrigger(DeadId);
        }
    }

}
