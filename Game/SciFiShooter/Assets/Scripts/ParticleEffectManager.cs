using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectManager :MonoBehaviour
{
    public static ParticleEffectManager Instance;
    public ParticleSystem[] particlesToPreload = new ParticleSystem[0];
    public int[] particlesPoolCount = new int[0];
    public bool hideObjectsInHierarchy;

    private bool allObjectsLoaded;
    private Dictionary<int, List<ParticleSystem>> instantiatedObjects = new Dictionary<int, List<ParticleSystem>>();
    private Dictionary<int, int> poolCursors = new Dictionary<int, int>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        allObjectsLoaded = false;

        for (int i = 0; i < particlesToPreload.Length; i++)
        {
            PreloadObject(particlesToPreload[i], particlesPoolCount[i]);
        }

        allObjectsLoaded = true;
    }


    void PreloadObject(ParticleSystem sourceObj, int poolSize = 1)
    {
        addObjectToPool(sourceObj, poolSize);
    }

    private void addObjectToPool(ParticleSystem sourceObject, int number)
    {
        int uniqueId = sourceObject.GetInstanceID();

        //Add new entry if it doesn't exist
        if (!instantiatedObjects.ContainsKey(uniqueId))
        {
            instantiatedObjects.Add(uniqueId, new List<ParticleSystem>());
            poolCursors.Add(uniqueId, 0);
        }

        //Add the new objects
        ParticleSystem newObj;
        for (int i = 0; i < number; i++)
        {
            newObj = Instantiate(sourceObject);
            var mainPart = newObj.main;
            mainPart.playOnAwake = false;
            newObj.gameObject.SetActive(false);
            instantiatedObjects[uniqueId].Add(newObj);
            if (hideObjectsInHierarchy)
                newObj.hideFlags = HideFlags.HideInHierarchy;
        }
    }



    ParticleSystem GetNextObject(ParticleSystem sourceObj, bool activateObject = true)
    {
        int uniqueId = sourceObj.GetInstanceID();

        if (!poolCursors.ContainsKey(uniqueId))
        {
            ////////Debug.LogError("Object hasn't been preloaded: " + sourceObj.name + " (ID:" + uniqueId + ")");
            return null;
        }

        ParticleSystem returnObj = null;
        int cursor = poolCursors[uniqueId];
        int poolCount = instantiatedObjects[uniqueId].Count;
        bool hasFound = false;
        for (int i=cursor;i< poolCount;++i)
        {
            if (instantiatedObjects[uniqueId][i] == null)
            {

            }
            else if (instantiatedObjects[uniqueId][i].gameObject.activeSelf == false)
            {
                hasFound = true;
                poolCursors[uniqueId]=i+1;
                returnObj = instantiatedObjects[uniqueId][i];
                break;
            }
        }

        
        if (poolCursors[uniqueId] >= poolCount)
        {
            poolCursors[uniqueId] = 0;
        }

        if (hasFound)
        {
            if (returnObj == null)
                returnObj = instantiatedObjects[uniqueId][cursor] = Instantiate(sourceObj);
            if (activateObject)
                returnObj.gameObject.SetActive(true);
        }
        else
        {
            returnObj = Instantiate(sourceObj);
            instantiatedObjects[uniqueId].Add(returnObj);
        }
      
        

        return returnObj;
    }

   
    public ParticleSystem ShowFx(string fxName, Vector3 position, float time2Live)
    {
        ParticleSystem fx = Array.Find(particlesToPreload, particle => particle.name == fxName);
        if (fx)
        {
            ParticleSystem playFx = GetNextObject(fx, true);
            playFx.transform.position = position;
            playFx.Play();
            StartCoroutine(WaitToHide(playFx, time2Live));
            return playFx;
        }
        else
        {
            ////////Debug.LogError("EffectManager not found FX!! " + fxName);
            return null;
        }
    }

    public ParticleSystem ShowFx(string fxName, Transform parent,Vector3 position, float time2Live)
    {
        ParticleSystem fx = Array.Find(particlesToPreload, particle => particle.name == fxName);
        if (fx)
        {
            ParticleSystem playFx = GetNextObject(fx, true);
            playFx.transform.SetParent(parent);
            playFx.transform.position = position;
            playFx.Stop();
            playFx.Play();
            StartCoroutine(WaitToHide(playFx, time2Live));
            return playFx;
        }
        else
        {
            ////////Debug.LogError("EffectManager not found FX!! " + fxName);
            return null;
        }
    }
   
    IEnumerator WaitToHide(ParticleSystem playingFX, float timeToLive)
    {
        yield return new WaitForSeconds(timeToLive);
        playingFX.Stop();
        playingFX.transform.SetParent(null);
        playingFX.gameObject.SetActive(false);
    }

}
