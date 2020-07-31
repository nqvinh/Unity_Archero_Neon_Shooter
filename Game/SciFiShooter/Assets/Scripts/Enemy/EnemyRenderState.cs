using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRenderState : MonoBehaviour
{
    [SerializeField] Renderer renderer;

    int mainColorID = Shader.PropertyToID("_MainColor");
    int progressAppearId = Shader.PropertyToID("_ProgressAppear");
    int isAppearID = Shader.PropertyToID("_isAppear");
    int isHighLightID = Shader.PropertyToID("_isHighLight");


    public void SetAppearProgress(float progress)
    {
        renderer.material.SetFloat(progressAppearId, progress);
    }

    public void SetActiveAppearFx(bool active)
    {
        renderer.material.SetFloat(progressAppearId, 0);
        renderer.material.SetFloat(isAppearID, active?1:0);
        renderer.material.SetFloat(isHighLightID, active ? 1 : 0);
    }

    public void SetActiveHighLightFx(bool highLight,Color color)
    {
        renderer.material.SetFloat(isHighLightID, highLight ? 1 : 0);
        renderer.material.SetColor(mainColorID, color);
    }

}
