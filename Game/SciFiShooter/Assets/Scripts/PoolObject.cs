using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject 
{
    List<GameObject> poolObjs = new List<GameObject>();
    int currentPoolCursor = 0;
    GameObject refPrefab = null;
    Transform poolParent = null;

    public void CreatePool(GameObject prefab, int initNumber,System.Action<GameObject> cbAfterCreateObject = null, Transform parent = null)
    {
        poolParent = parent;
        refPrefab = prefab;
        currentPoolCursor = 0;
        for (int i=0;i<initNumber;++i)
        {
            GameObject go = GameObject.Instantiate(prefab);
            go.SetActive(false);
            poolObjs.Add(go);
            go.transform.SetParent(parent);
            //DontDestroyOnLoad(go);
            Object.DontDestroyOnLoad(go);
            if (cbAfterCreateObject!= null)
                cbAfterCreateObject(go);
        }
    }

    public GameObject GetNextObject()
    {
        int poolCount = poolObjs.Count;
        GameObject res = null;
        for (int i=currentPoolCursor;i<poolCount;++i)
        {
            if (poolObjs[i].activeSelf == false)
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
