using UnityEngine;
using System.Collections;
using System;

public class MathHelper
{
    const float CONST_PARAMETER_WIND = 0.014f;

    const float BONUS_BOOMER_X = 0.9f;
    const float BONUS_BOOMER_Y = 0.1f;
    const float BONUS_BOOMER = 6.0f;
    const float RATIO_WIND = 90 / 30;
    const float BONUS_HOOK_SHOT_X = 2.0f;
    const float BONUS_BACK_SHOT_X = 0.8f;
    const float RATIO_DRAG_OFFSET =1f;

    public static float Atan2(Vector2 xy)
    {
        return Mathf.Atan2(xy.x, xy.y);
    }

    public static Vector2 forAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public static Vector2 calculatorVecPure(Vector2 posStart, float angle, float force, float iCurrentTime, float weightBullet = 10.0f)
    {
        Vector2 vPosition = Vector2.zero;
        vPosition.x = posStart.x + force * Mathf.Cos(angle) * iCurrentTime;
        vPosition.y = posStart.y + (force * Mathf.Sin(angle) * iCurrentTime) - (weightBullet * iCurrentTime * iCurrentTime) / 2;
        return vPosition;
    }

    public static Vector2 calculatorVecPureWithWind(Vector2 posStart, float angle, float force, float iCurrentTime, float weightBullet, Vector2 vecWind)
    {
        Vector2 vPosition = Vector2.zero;
        Vector2 vectorMovement = Vector2.zero;
        vectorMovement.x = force * Mathf.Cos(angle) * iCurrentTime + (vecWind.x * CONST_PARAMETER_WIND * iCurrentTime * iCurrentTime);
        vectorMovement.y = (force * Mathf.Sin(angle) * iCurrentTime) - (weightBullet * iCurrentTime * iCurrentTime) / 2 + (vecWind.y * CONST_PARAMETER_WIND * iCurrentTime * iCurrentTime);

        vPosition.x = posStart.x + vectorMovement.x;
        vPosition.y = posStart.y + vectorMovement.y;
        return vPosition;
    }

    public static Vector2 calculatorPosDrag(Vector2 pointStart, Vector2 pointTo, float angleShoot, float weightBullet, Vector2 vecWind)
    {
        if (((int)angleShoot - 90) % 180 == 0)
            return Vector2.zero;


        angleShoot = angleShoot * Mathf.Deg2Rad; /*CC_DEGREES_TO_RADIANS(angleShoot);*/

        float A = -((vecWind.x * CONST_PARAMETER_WIND * Mathf.Sin(angleShoot)) / Mathf.Cos(angleShoot) + weightBullet / 2.0f - vecWind.y * CONST_PARAMETER_WIND);
        float B = 0;
        float C = ((pointTo.x - pointStart.x) * Mathf.Sin(angleShoot)) / Mathf.Cos(angleShoot) - (pointTo.y - pointStart.y);



        float delta = -4 * A * C;
        if (delta < 0)
            return Vector2.zero;

        float iCurrentTime1 = Mathf.Sqrt(delta) / (2 * A);
        float iCurrentTime2 = -Mathf.Sqrt(delta) / (2 * A);

        float fVelocityAnpha1;
        float time = 0;

        bool NghiemKep = false;
        bool HaiNghiemPhanBiet = false;

        if (Mathf.RoundToInt(iCurrentTime1) > 0)
        {
            time = iCurrentTime1;
            HaiNghiemPhanBiet = true;
        }
        else if (Mathf.RoundToInt(iCurrentTime2) > 0)
        {
            time = iCurrentTime2;
            HaiNghiemPhanBiet = true;
        }
        else if (Mathf.RoundToInt(iCurrentTime1) == 0)
        {
            time = iCurrentTime1;
            NghiemKep = true;
        }
        else if (Mathf.RoundToInt(iCurrentTime2) == 0)
        {
            time = iCurrentTime2;
            NghiemKep = true;
        }
        else
            return Vector2.zero;
        //A * X^2 + B * X + C = 0

        Vector2 vDrag=Vector2.zero;

        if (HaiNghiemPhanBiet)
        {
            fVelocityAnpha1 = ((pointTo.x - pointStart.x) - vecWind.x * CONST_PARAMETER_WIND * time * time) / (Mathf.Cos(angleShoot) * time);


            Vector2 angle = forAngle(angleShoot);
            Vector2 temp = angle * fVelocityAnpha1;
            temp *= RATIO_DRAG_OFFSET;
            vDrag = (pointStart - temp);
        }


        //Vec2 test = calculatorVecPure(pointStart,vDrag,time,weightBullet,vecWind);

        return vDrag;
    }

    internal static float calculatorAngleTwoVector(Vector2 vec1, Vector2 vec2)
    {
        //Get the dot product
        //float dot = Vector3.Dot(vec1, vec2);
        //// Divide the dot by the product of the magnitudes of the vectors
        //dot = dot / (vec1.magnitude * vec2.magnitude);
        ////Get the arc cosin of the angle, you now have your angle in radians 
        //var acos = Mathf.Acos(dot);
        ////Multiply by 180/Mathf.PI to convert to degrees
        //var angle = acos * 180 / Mathf.PI;
        ////Congrats, you made it really hard on yourself.
        //return angle;

        float angle = Vector2.Angle(vec1, vec2);
        return angle;
    }

    public static Vector2 calculatorPosDrag(Vector2 pointStart, Vector2 pointTo, float angleShoot, float weightBullet, Vector2 vecWind,out float timeCollision)
    {
        timeCollision = 0;
        if (((int)angleShoot - 90) % 180 == 0)
        {
            return Vector2.zero;
        }
            


        angleShoot = angleShoot*Mathf.Deg2Rad;

        float A = -((vecWind.x * CONST_PARAMETER_WIND * Mathf.Sin(angleShoot)) / Mathf.Cos(angleShoot) + weightBullet / 2.0f - vecWind.y * CONST_PARAMETER_WIND);
        float B = 0;
        float C = ((pointTo.x - pointStart.x) * Mathf.Sin(angleShoot)) / Mathf.Cos(angleShoot) - (pointTo.y - pointStart.y);


        float A1 = -weightBullet / 2.0f;
        float B1 = 0;
        float C1 = (((pointTo.x - pointStart.x) - vecWind.x * CONST_PARAMETER_WIND) * Mathf.Sin(angleShoot)) / Mathf.Cos(angleShoot) - vecWind.y * CONST_PARAMETER_WIND - (pointTo.y - pointStart.y);


        float delta = -4 * A * C;

        float delta1 = -4 * A1 * C1;

        if (delta < 0)
            return Vector2.zero;

        float iCurrentTime1 = Mathf.Sqrt(delta) / (2 * A);
        float iCurrentTime2 = -Mathf.Sqrt(delta) / (2 * A);

        float time1 = Mathf.Sqrt(delta1) / (2 * A1);
        float time2 = Mathf.Sqrt(delta1) / (2 * A1);

        float fVelocityAnpha1;
        float time = 0;

        bool NghiemKep = false;
        bool HaiNghiemPhanBiet = false;

        if ((int)iCurrentTime1 > 0)
        {
            time = iCurrentTime1;
            HaiNghiemPhanBiet = true;
        }
        else if ((int)iCurrentTime2 > 0)
        {
            time = iCurrentTime2;
            HaiNghiemPhanBiet = true;
        }
        else if ((int)iCurrentTime1 == 0)
        {
            time = iCurrentTime1;
            NghiemKep = true;
        }
        else if ((int)iCurrentTime2 == 0)
        {
            time = iCurrentTime2;
            NghiemKep = true;
        }
        else
            return Vector2.zero;
        //A * X^2 + B * X + C = 0

        Vector2 vDrag=Vector2.zero;

        if (HaiNghiemPhanBiet)
        {
            fVelocityAnpha1 = ((pointTo.x - pointStart.x) - vecWind.x * CONST_PARAMETER_WIND * time * time) / (Mathf.Cos(angleShoot) * time);

            Vector2 angleVector = forAngle(angleShoot) * fVelocityAnpha1 * RATIO_DRAG_OFFSET;
            vDrag = (pointStart - angleVector);

            timeCollision = time;
        }


        //Vec2 test = calculatorVecPure(pointStart,vDrag,time,weightBullet,vecWind);

        return vDrag;
    }

    public static float calculatorForce(Vector2 pointStart, Vector2 pointTarget, float angle)
    {
        Vector2 direction = pointStart - pointTarget;
        //direction = direction - pointEnd;


        float fAngleAnpha = angle;
        float fVelocityAnpha = direction.magnitude;


        float v0 = Mathf.Sqrt(fVelocityAnpha * 3 / Mathf.Sin(2 * fAngleAnpha));
        float v1 = Mathf.Sqrt(-fVelocityAnpha * 3 / Mathf.Sin(2 * fAngleAnpha));

        return v0 > 0 ? v0 / 2 : v1 / 2;
    }

    public static float calculatorTimeToHmax(Vector2 pointStart, Vector2 pointEnd, Vector2 wind, float weightBullet)
    {
        Vector2 direction = pointStart - pointEnd;
        //direction = direction - pointEnd;

        direction = direction / RATIO_DRAG_OFFSET;

        float fAngleAnpha = Vector2.Angle(pointStart, pointEnd); ;
        float fVelocityAnpha = direction.magnitude;

        float time = (fVelocityAnpha * Mathf.Sin(fAngleAnpha)) / (weightBullet) - wind.y * CONST_PARAMETER_WIND * wind.magnitude;
        time = Mathf.Abs(time);
        return time;
    }

    public static float calculatorAngle(Vector2 point1,Vector2 point2)
    {
       return Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) * Mathf.Rad2Deg;
    }

}