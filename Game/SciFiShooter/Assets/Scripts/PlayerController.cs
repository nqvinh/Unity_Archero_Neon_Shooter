using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Renderer stepQuad;

    [SerializeField]
    Transform leftLegPoint;

    [SerializeField]
    Transform rightLegPoint;

    [SerializeField]
    LayerMask groundMask;


    [SerializeField]
    Joystick joystick;

    [SerializeField]
    float moveSpeed;

    private Rigidbody rigidbody;

    [SerializeField]
    private PlayerAnimationController animator;

    

    [SerializeField]
    private float stepDistance = 0f;

    Vector3 offsetStep = new Vector3(0, 0.02f, 0);

    PoolObject poolObject = new PoolObject();
    float moveTimePoint = 0;
    int stepCount = 0;

    public Rigidbody Rigidbody { get { if (rigidbody == null) rigidbody = GetComponent<Rigidbody>(); return rigidbody; } }


    private void Start()
    {
        poolObject.CreatePool(stepQuad.gameObject, 10);
    }

    void FixedUpdate()
    {
        float h = 0, v = 0;
        if (joystick != null)
        {
            h = joystick.Horizontal;
            v = joystick.Vertical;
        }
        else
        {
          
        }

        bool running = h != 0f || v != 0f;

        if (running)
        {
            Move(h, v);
            animator.Shooting = false;

            if (moveTimePoint == 0 || Time.time - moveTimePoint > stepDistance)
            {
                OnStep();
                moveTimePoint = Time.time;
            }
        }
        else
        {
            animator.Shooting = true;
            moveTimePoint = 0;
            stepCount = 0;
        }
        animator.Running = running;
        
    }


    void Move(float h, float v)
    {      


        Vector3 movement = new Vector3(h, 0, v);
        
        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * moveSpeed * Time.deltaTime;
        movement.y = 0;
        Vector3 curPos = transform.position;
        curPos.y = 0;
        Rigidbody.MovePosition(curPos + movement);
        Turning();
    }

    void Turning()
    {
        if (joystick != null)
        {
            Vector3 moveVector = (Vector3.right * joystick.Horizontal + Vector3.forward * joystick.Vertical);
            Rigidbody.MoveRotation(Quaternion.LookRotation(moveVector));
        }
    }

    void OnStep()
    {
        RaycastHit raycastHit;
        if (stepCount % 2 == 0)
        {
            Physics.Raycast(leftLegPoint.position, -Vector3.up, out raycastHit, float.MaxValue, groundMask);
        }
        else
        {
            Physics.Raycast(rightLegPoint.position, -Vector3.up, out raycastHit, float.MaxValue, groundMask);
        }

        if (raycastHit.collider)
        {
            GameObject step = poolObject.GetNextObject();
            step.GetComponent<FootStepRenderer>().SetVisible();
            step.transform.position = raycastHit.point + offsetStep;
            step.transform.localEulerAngles = new Vector3(90, this.transform.localEulerAngles.y, 0);
        }

        stepCount++;
    }

    public void OnLeftFoot()
    {
        //OnStep();
    }

    public void OnRightFoot()
    {
        //OnStep();
    }
}
