using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepRenderer : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    MaterialPropertyBlock propertyBlock;
    int dissolveProgressID = Shader.PropertyToID("_DissolveProgress");
    float dissolveProgress = 1.0f;

    public void SetVisible()
    {
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();

        gameObject.SetActive(true);
        dissolveProgress = 1.0f;
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(dissolveProgressID, dissolveProgress);
        renderer.SetPropertyBlock(propertyBlock);
        StartCoroutine(Dissolve(0.5f));
    }

    IEnumerator Dissolve(float time)
    {
        float timePoint = Time.time;
        while ( Time.time - timePoint < time)
        {
            renderer.GetPropertyBlock(propertyBlock);
            dissolveProgress -= 0.1f;
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(dissolveProgressID, dissolveProgress);
            renderer.SetPropertyBlock(propertyBlock);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
