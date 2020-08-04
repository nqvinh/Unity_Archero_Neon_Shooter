using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BulletMovementType
{
    StandStill,
    MoveForward,
    MoveParabol,
    COUNT
}

public static class BulletMovementFactory
{
    public static BulletMovementUpdater GetBulletMovementUpdater(BulletMovementType bulletMovementType)
    {
        switch (bulletMovementType)
        {
            case BulletMovementType.StandStill:
                return new StandStill();
            case BulletMovementType.MoveForward:
                return new MoveForward();
            case BulletMovementType.MoveParabol:
                return new MoveParabol();
            case BulletMovementType.COUNT:
                break;
            default:
                break;
        }
        return null;
    }
}

public abstract class BulletMovementUpdater
{
    protected Transform _target;
    protected Vector3 _beginShotPos;
    protected Vector3 _endShotPos;
    protected Vector3 _shotDir;
    protected float shotTimePoint;

    public float baseSpeed;
    public float accelery;
    public float sprialSpeed;
    public float jumpHeight;
    public float jumpDistance;
    public Transform origin;

    public abstract void Setup(BaseBullet bullet,Transform target, Vector3 beginShotPos);

    public abstract void Setup(BaseBullet bullet, Vector3 dir, Vector3 beginShotPos);


    public abstract void UpdateMovement(BaseBullet bullet);
}


public class StandStill : BulletMovementUpdater
{
    public override void Setup(BaseBullet bullet,Transform target, Vector3 beginShotPos)
    {
        
    }

    public override void Setup(BaseBullet bullet, Vector3 dir, Vector3 beginShotPos)
    {

    }
    public override void UpdateMovement(BaseBullet bullet)
    {
        
    }
}

public class MoveForward : BulletMovementUpdater
{
    public override void Setup(BaseBullet bullet,Transform target, Vector3 beginShotPos)
    {
        bullet.transform.position = beginShotPos;
        bullet.transform.forward = (target.position - beginShotPos).normalized;
       
    }

    public override void Setup(BaseBullet bullet, Vector3 dir, Vector3 beginShotPos)
    {
        bullet.transform.position = beginShotPos;
        bullet.transform.forward = dir;
    }



    public override void UpdateMovement(BaseBullet bullet)
    {
        bullet.transform.position += bullet.transform.forward * baseSpeed * Time.smoothDeltaTime;
        if (origin)
        {
            bullet.transform.RotateAround(origin.position, bullet.transform.up, sprialSpeed);
            bullet.transform.LookAt(bullet.transform.position);
        }
            
        
    }
}

public class MoveParabol : BulletMovementUpdater
{
    public override void Setup(BaseBullet bullet,Transform target, Vector3 beginShotPos)
    {
        this.shotTimePoint = Time.time;
        this._beginShotPos = beginShotPos;
        this._endShotPos = target.position;
        bullet.transform.forward = (target.position + new Vector3(0, 0.5f, 0) - beginShotPos).normalized;
    }

    public override void Setup(BaseBullet bullet, Vector3 dir, Vector3 beginShotPos)
    {
        bullet.transform.position = beginShotPos;
        bullet.transform.forward = dir;
        this._beginShotPos = beginShotPos;
        this._endShotPos = beginShotPos + dir * this.jumpDistance;
        this.shotTimePoint = Time.time;

    }


    public override void UpdateMovement(BaseBullet bullet)
    {
        float eslapedTime = Time.time - this.shotTimePoint;
        bullet.transform.position = (MathParabola.Parabola(this._beginShotPos, this._endShotPos, this.jumpHeight, eslapedTime));

        Vector3 nextPos = MathParabola.Parabola(this._beginShotPos, this._endShotPos, this.jumpHeight, eslapedTime + Time.deltaTime);
        bullet.transform.LookAt(nextPos);
    }
}
