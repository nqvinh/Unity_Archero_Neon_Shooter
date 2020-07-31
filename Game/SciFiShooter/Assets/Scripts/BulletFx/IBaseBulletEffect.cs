using DG.Tweening;
using UnityEngine;


public enum BulletEffect
{
    None,
    HitTint,
    Fire,
    EletricSock,
    FreezeSlow,
    COUNT
}

public struct BulletEffectData
{
    public float duration;
    public float dame;
    public float radius;
}

public interface  IBaseBulletEffect 
{
      //void OnApplyEffectToTarget(MobController target, BulletEffectData bulletEffectData);
}

public static class BulletEffectFactory
{
    public static IBaseBulletEffect GetBulletEffect(BulletEffect bulletEffect)
    {
        //switch (bulletEffect)
        //{
        //    case BulletEffect.None:
        //        break;
        //    case BulletEffect.HitTint:
        //        //return new  BloodAndHitTint();
        //    case BulletEffect.Fire:
        //       // return new FireOnMobEffect();
        //    case BulletEffect.EletricSock:
        //        //break;
        //    case BulletEffect.FreezeSlow:
        //       // return new BulletFrozenEffect();

        //}
        return null;
    }
}
