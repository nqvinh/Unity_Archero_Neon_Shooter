using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepRenderer : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    int dissolveProgressID = Shader.PropertyToID("_DissolveProgress");
    float dissolveProgress = 1.0f;

    public void SetVisible()
    {
        gameObject.SetActive(true);
        dissolveProgress = 1.0f;
        renderer.material.SetFloat(dissolveProgressID, dissolveProgress);
      
        StartCoroutine(Dissolve(0.5f));
    }

    IEnumerator Dissolve(float time)
    {
        float timePoint = Time.time;
        while ( Time.time - timePoint < time)
        {
            dissolveProgress -= 0.1f;
            renderer.material.SetFloat(dissolveProgressID, dissolveProgress);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
