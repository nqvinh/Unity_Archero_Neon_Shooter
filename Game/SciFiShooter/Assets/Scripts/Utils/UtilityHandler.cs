using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;

public class UtilityHandler
{
    public enum Direction_Define
    {
        LEFT = 0,
        TOP,
        RIGHT,
        BOTTOM
    }

    public enum Face_Define
    {
        LEFT = 0,
        TOP,
        RIGHT,
        DOWN
    }

    private static bool hasSetRandSeed = false;
    private static int Randx = 0;
    private static int Randy = 0;
    private static int Randz = 0;
    private static int Randw;
    public static Color32[] topColorDesign = new Color32[]{new Color32(244, 67, 54,255) ,new Color32(233, 30, 99,255) ,new Color32(156, 39, 176,255) ,new Color32(103, 58, 183,255),new Color32(63, 81, 181,255),
                                                            new Color32(33, 150, 243,255),new Color32(3, 169, 244,255),new Color32(0, 188, 212,255),new Color32(0, 150, 136,255),new Color32(76, 175, 80,255),
                                                           new Color32(139, 195, 74,255),new Color32(205, 220, 57,255),new Color32(255, 235, 59,255),new Color32(255, 193, 7,255), new Color32(255, 152, 0,255),
                                                           new Color32(255, 87, 34,255),new Color32(121, 85, 72,255),new Color32(158, 158, 158,255),new Color32(96, 125, 139,255)};

    private static Camera mainCam;


    static string _lookupStringL = string.Empty;
    static string _lookupStringU = string.Empty;

    public static int GetARandomNumber()
    {
        if (!hasSetRandSeed)
        {
            Randx = UnityEngine.Random.Range(1, 213214454);
            Randy = UnityEngine.Random.Range(1, 131313212);
            Randz = UnityEngine.Random.Range(1, 65014874);
            hasSetRandSeed = true;
        }
        int t = Randx ^ (Randx << 11);
        Randx = Randy; Randy = Randz; Randz = Randw;
        return Randw = Randw ^ (Randw >> 19) ^ t ^ (t >> 8);
    }

    public static int GetARandomNumber(int minNumber, int maxNumber)
    {
        if (minNumber == maxNumber)
            return minNumber;
        int randNumber = GetARandomNumber() % maxNumber;
        return randNumber + minNumber + 1;
    }

    public static long GetARandomNumber(long minNumber, long maxNumber)
    {
        long randNumber = GetARandomNumber() % maxNumber;
        return randNumber + minNumber + 1;
    }

    public static double GetAClampDouble()
    {
        int maxValue = GetARandomNumber();
        int value = GetARandomNumber(0, maxValue);
        double res = value * 1.0f / maxValue;
        return res;
    }


    public static List<T> FindObjectsOfTypeAll<T>()
    {
        List<T> results = new List<T>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.isLoaded)
            {
                var allGameObjects = s.GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++)
                {
                    var go = allGameObjects[j];
                    results.AddRange(go.GetComponentsInChildren<T>(true));
                }
            }
        }
        return results;
    }

    public static Bounds GetObjectBound(GameObject gameObject)
    {
        var b = new Bounds(gameObject.transform.position, Vector3.zero);
        if (gameObject.GetComponent<RectTransform>() != null)
        {
            return new Bounds(gameObject.transform.position, gameObject.GetComponent<RectTransform>().sizeDelta);
        }
        else
        {

            foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            {
                b.Encapsulate(r.bounds);
            }
        }

        return b;
    }

    public static IEnumerator WaitForFrames(int frameCount, System.Action cb = null)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
        if (cb != null)
            cb();
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public static Vector2 ConvertWorldPositionToScreenPosition(Vector3 worldPos, Canvas screenCanvas)
    {
        RectTransform CanvasRect = screenCanvas.GetComponent<RectTransform>();

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(worldPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        return WorldObject_ScreenPosition;
    }

    public static Vector2 ConvertWorldPositionToScreenPosition(Vector3 worldPos, Canvas screenCanvas, Camera camera)
    {
        RectTransform CanvasRect = screenCanvas.GetComponent<RectTransform>();

        Vector2 ViewportPosition = camera.WorldToViewportPoint(worldPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.rect.width) - (CanvasRect.rect.width * 0.5f)),
        ((ViewportPosition.y * CanvasRect.rect.height) - (CanvasRect.rect.height * 0.5f)));

        return WorldObject_ScreenPosition;
    }

    public static string FormatMoneyText(long money)
    {
        return money.ToString("N0", new System.Globalization.CultureInfo("en-US"));
    }


    public static string GetEnumName<T>(T value)
    {
        return Enum.GetName(value.GetType(), value);
    }

    public static float ChangeUnitToPixel(float a_fUnit, int PPU)
    {
        return (a_fUnit * PPU);
    }

    public static Vector2 ConvertUnitToPixel(Vector2 a_vUnit, int PPU)
    {
        return new Vector2(ChangeUnitToPixel(a_vUnit.x, PPU), Mathf.Abs(ChangeUnitToPixel(a_vUnit.y, PPU)));
    }

    public static Vector3 GetOutSideCameraViewPosition(Direction_Define dir, GameObject gameObject)
    {
        Camera camera = Camera.main;
        Vector3 initPos = Vector3.zero;
        var bound = UtilityHandler.GetObjectBound(gameObject);
        Vector3 res = gameObject.transform.position;

        switch (dir)
        {
            case Direction_Define.LEFT:
                initPos = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
                res.x = initPos.x - bound.extents.x - (Mathf.Abs(res.x - bound.center.x));
                break;
            case Direction_Define.TOP:
                initPos = camera.ViewportToWorldPoint(new Vector3(0, 1, camera.nearClipPlane));
                res.y = initPos.y + bound.extents.y + (Mathf.Abs(res.y - bound.center.y));
                break;
            case Direction_Define.RIGHT:
                initPos = camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane));
                res.x = initPos.x + bound.extents.x + (Mathf.Abs(res.x - bound.center.x));
                break;
            case Direction_Define.BOTTOM:
                initPos = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
                res.y = initPos.y - bound.extents.y - (Mathf.Abs(res.y - bound.center.y));
                break;
        }
        return res;
    }

    public static bool IsOutSideCameraBound(GameObject gameObject, Direction_Define fromDirect)
    {
        Camera camera = Camera.main;
        Vector3 checkPos = GetOutSideCameraViewPosition(fromDirect, gameObject);
        switch (fromDirect)
        {
            case Direction_Define.LEFT:
                return gameObject.transform.position.x <= checkPos.x;
            case Direction_Define.TOP:
                return gameObject.transform.position.y >= checkPos.y;
            case Direction_Define.RIGHT:
                return gameObject.transform.position.x >= checkPos.x;
            case Direction_Define.BOTTOM:
                return gameObject.transform.position.y <= checkPos.y;
        }
        return false;
    }

    public static void MoveDOTweanSpline(GameObject gameObject, Transform targetTransform, float midPointOffset, Direction_Define fromDirect, float time, Ease easeType = Ease.Linear, System.Action moveComplete = null)
    {
        int dirMove = gameObject.transform.position.x > targetTransform.position.x ? 1 : -1;
        Vector3 middlePos = (gameObject.transform.position + targetTransform.transform.position) / 2;
        if (fromDirect == Direction_Define.TOP || fromDirect == Direction_Define.BOTTOM)
            middlePos.x += (midPointOffset * dirMove);
        else if (fromDirect == Direction_Define.LEFT || fromDirect == Direction_Define.RIGHT)
            middlePos.y += (midPointOffset);

        Vector3[] wayPoint = new Vector3[] { gameObject.transform.position, gameObject.transform.position, middlePos, targetTransform.transform.position, targetTransform.transform.position };
        gameObject.transform.DOPath(wayPoint, time, PathType.CatmullRom).OnComplete(() => { if (moveComplete != null) moveComplete(); }).SetEase(easeType);

    }

    public static void MoveDOTweenSpline(GameObject gameObject, Vector3 targetPos, float midPointOffset, Direction_Define fromDirect, float time, Ease easeType = Ease.Linear, System.Action moveComplete = null)
    {
        int dirMove = gameObject.transform.position.x > targetPos.x ? 1 : -1;
        Vector3 middlePos = (gameObject.transform.position + targetPos) / 2;
        if (fromDirect == Direction_Define.TOP || fromDirect == Direction_Define.BOTTOM)
            middlePos.x += (midPointOffset * dirMove);
        else if (fromDirect == Direction_Define.LEFT || fromDirect == Direction_Define.RIGHT)
            middlePos.y += (midPointOffset);

        Vector3[] wayPoint = new Vector3[] { gameObject.transform.position, gameObject.transform.position, middlePos, targetPos, targetPos };
        gameObject.transform.DOPath(wayPoint, time, PathType.CatmullRom).OnComplete(() => { if (moveComplete != null) moveComplete(); }).SetEase(easeType);

    }

    public static Vector2 switchToRectTransform(RectTransform from, RectTransform to)
    {
        Vector2 localPoint;
        Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
        screenP += fromPivotDerivedOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
        return to.anchoredPosition + localPoint - pivotDerivedOffset;
    }

    public static Vector2 GetWorldPosOfRectTrans(RectTransform rectTrans)
    {
        return rectTrans.transform.TransformPoint(rectTrans.rect.center);
    }

    public static IEnumerator MoveOverSpeed(GameObject objectToMove, Vector3 end, float speed, Action cb)
    {
        // speed should be 1 unit per second
        while (objectToMove.transform.position != end)
        {
            objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        if (cb != null)
        {
            cb();
        }
    }
    public static IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds, Action cb)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
        if (cb != null)
        {
            cb();
        }
    }

    public static Vector2 calculatorVecPure(Vector2 posStart, float angle, float force, float iCurrentTime, float weightBullet = 10.0f)
    {
        Vector2 vPosition = Vector2.zero;
        vPosition.x = posStart.x + force * Mathf.Cos(angle) * iCurrentTime;
        vPosition.y = posStart.y + (force * Mathf.Sin(angle) * iCurrentTime) - (weightBullet * iCurrentTime * iCurrentTime) / 2;
        return vPosition;
    }

    public static UInt64 GenerateUserId()
    {
        string userId = SystemInfo.deviceUniqueIdentifier + DateTime.Now.ToFileTimeUtc().ToString();
        MD5 md5Hasher = MD5.Create();
        var hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(userId));
        UInt64 ivalue = BitConverter.ToUInt64(hashed, 0);
        return ivalue;
    }




    public static string FormatStringDelimiter(string text)
    {
        text = text.Replace("\\n", "\n");
        text = text.Replace("\\t", "\t");
        return text;
    }

    // manh.ng parse string to enum
    public static T ToEnum<T>(string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }


    private static void InitLookupChartable()
    {
        char[] lData = new char[128];
        char[] uData = new char[128];
        for (int i = 0; i < 128; i++)
        {
            char value = (char)i;
            if (!char.IsLetter(value))
            {
                lData[i] = value;
                uData[i] = value;
            }
            else
            {
                lData[i] = ToLowerFastIf(value);
                uData[i] = ToUpperFastIf(value);
            }
        }

        _lookupStringL = new string(lData);
        _lookupStringU = new string(uData);
    }

    public static char ToLowerFast(char c)
    {
        if (_lookupStringL == string.Empty || _lookupStringU == string.Empty)
        {
            InitLookupChartable();
        }
        // Use char lookup table.
        return _lookupStringL[c];
    }

    public static char ToUpperFast(char c)
    {
        if (_lookupStringL == string.Empty || _lookupStringU == string.Empty)
        {
            InitLookupChartable();
        }
        // Use char lookup table.
        return _lookupStringU[c];
    }

    public static string ToLowerFast(string str)
    {
        char[] array = str.ToCharArray();
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = ToLowerFast(array[i]);
        }
        string result = new string(array);
        return result;
    }

    public static string ToUpperFast(string str)
    {
        char[] array = str.ToCharArray();
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = ToLowerFast(array[i]);
        }
        string result = new string(array);
        return result;
    }

    public static char ToLowerFastIf(char c)
    {
        // Use if-statement.
        if (c >= 'A' && c <= 'Z')
        {
            return (char)(c + 32);
        }
        else
        {
            return c;
        }
    }

    public static char ToUpperFastIf(char c)
    {
        // Use if-statement.
        if (c >= 'a' && c <= 'z')
        {
            return (char)(c - 32);
        }
        else
        {
            return c;
        }
    }

    public static DateTime GetCurrentUsTime()
    {
        DateTime timeUtc = DateTime.UtcNow;
        try
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
            return cstTime;
        }
        catch (TimeZoneNotFoundException)
        {
            return timeUtc;
        }
        catch (InvalidTimeZoneException)
        {
            return timeUtc;
        }
    }

    public static string Base64Encode(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return "";
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        if (string.IsNullOrEmpty(base64EncodedData))
            return "";
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static void ShuffleList<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = GetARandomNumber(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static float Distance(Vector3 p1, Vector3 p2)
    {
        float d = (p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) + (p1.z - p2.z) * (p1.z - p2.z);
        return Mathf.Sqrt(d);
    }


    public static void SetLayerRecursively(GameObject go, int layerNumber)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    public static void FormatButtonState(Button btn, bool interectable, Material grayScale)
    {
        btn.interactable = interectable;

        //GlimmerUI glimmerUI = btn.GetComponentInChildren<GlimmerUI>();
        //if (glimmerUI != null)
        //    glimmerUI.gameObject.SetActive(interectable == true);
        Image[] img = btn.gameObject.GetComponentsInChildren<Image>();
        for (int i = 0; i < img.Length; ++i)
        {

            img[i].material = interectable ? null : grayScale;
        }
    }

    public static bool IsVisibleFrom(SphereCollider collider, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
    }
}