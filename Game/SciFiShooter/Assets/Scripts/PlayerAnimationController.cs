using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    enum AnimParam
    {
        Running,
        Shooting,
        COUNT
    }

    private Animator animator;
    public Animator Animator { get { if (animator == null) animator = GetComponent<Animator>(); return animator; } }

    public static int RunningID = Animator.StringToHash(AnimParam.Running.ToString());
    public static int ShootingID = Animator.StringToHash(AnimParam.Shooting.ToString());

    public bool Running
    {
        set
        {
            Animator.SetBool(RunningID, value);
        }
    }

    public bool Shooting
    {
        set
        {
            Animator.SetBool(ShootingID, value);
        }
    }


    private void Awake()
    {
        animator = this.GetComponentInChildren<Animator>();
    }

   

    public void OnLeftFoot()
    {
        playerController.OnLeftFoot();
    }

    public void OnRightFoot()
    {
        playerController.OnRightFoot();
    }
}
