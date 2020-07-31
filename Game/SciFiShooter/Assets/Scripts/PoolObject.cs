using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject<T> where T:MonoBehaviour 
{
    List<T> poolObjs = new List<T>();
    int currentPoolCursor = 0;
    T refPrefab;
    Transform poolParent = null;

    public bool IsReady { get { return poolObjs.Count > 0; } }

    public void CreatePool(T prefab, int initNumber,System.Action<T> cbAfterCreateObject = null, Transform parent = null)
    {
        poolParent = parent;
        refPrefab = prefab;
        currentPoolCursor = 0;
        for (int i=0;i<initNumber;++i)
        {
            T go = GameObject.Instantiate(prefab);
            go.gameObject.SetActive(false);
            poolObjs.Add(go);
            go.transform.SetParent(parent);
            Object.DontDestroyOnLoad(go);
            if (cbAfterCreateObject!= null)
                cbAfterCreateObject(go);
        }
    }

    public T GetNextObject()
    {
        int poolCount = poolObjs.Count;
        T res = null ;
        for (int i=currentPoolCursor;i<poolCount;++i)
        {
            if (poolObjs[i].gameObject.activeSelf == false)
            {
                currentPoolCursor++;
                if (currentPoolCursor == poolCount - 1)
                    currentPoolCursor = 0;
                res =poolObjs[i];
                break;
            }
        }

        if (res == null)
        {
            currentPoolCursor = 0;
            res = GameObject.Instantiate(refPrefab, poolParent);
            poolObjs.Add(res);
        }
        res.gameObject.SetActive(true);
        return res;
    }
}
