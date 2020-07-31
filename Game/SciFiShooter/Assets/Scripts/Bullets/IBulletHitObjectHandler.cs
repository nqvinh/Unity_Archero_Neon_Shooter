using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BulletHitType
{
    SingleTarget,
    HitBySplash,
    HitByChaining,
    HitBySpray,
    HitBySock,
    COUNT
}

public static class BulletHitFactory
{
    public static IBulletHitObjectHandler GetBulletHitObjectHandler(BulletHitType bulletHitType)
    {
        switch (bulletHitType)
        {
            case BulletHitType.SingleTarget:
                return new HitSingleTarget();
          
            default:
                break;
        }
        return null;
    }
}

public interface IBulletHitObjectHandler
{
    void ProcessHitObj(BaseBullet baseBullet, Transform hitObj,  Action<GameObject> cb);
}

public class HitSingleTarget : IBulletHitObjectHandler
{
    public void ProcessHitObj(BaseBullet baseBullet, Transform hitObj, Action<GameObject> cb)
    {
        EnemyController hitEnemy = hitObj.GetComponentInParent<EnemyController>();
        PlayerController hitPlayer = hitObj.GetComponentInParent<PlayerController>();
        baseBullet.gameObject.SetActive(false);

        //Show FX
        if (hitEnemy)
        {
            hitEnemy.OnHit(baseBullet.Damage);
        }else if(hitPlayer)
        {
            hitPlayer.OnHit(baseBullet.Damage);
        }
    }
}
