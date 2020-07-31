using DG.Tweening;
using UnityEngine;

public class BeamFx:MonoBehaviour
{
    [SerializeField]
    protected LineRenderer lineRenderer;

    [SerializeField]
    protected ParticleSystem beamImpact;

    protected bool isShoting = false;

    protected Transform startTrans, endTrans;
    protected float hitTimePoint = 0;
    protected float hitInterval = 0;

    public System.Action onHit;
    public System.Action onBeamEnd;
    protected float eslapedBeamTime = 0;
    protected float timeToLive = 0;


    public virtual void ShotBeam(Transform start, Transform end, float time2Live, float hitTimeInterval)
    {
        startTrans = start;
        endTrans = end;
        gameObject.SetActive(true);
        if (lineRenderer)
        {
            lineRenderer.SetPosition(0, start.position);
            lineRenderer.SetPosition(1, end.position);
        }
        eslapedBeamTime = 0;
        timeToLive = time2Live;
        hitTimePoint = Time.time;// -9999;
        hitInterval = hitTimeInterval;
        isShoting = true;

        if (beamImpact)
        {
            beamImpact.transform.position = end.position;
            beamImpact.Stop();
            beamImpact.Play();
        }


    }

    public virtual void StopBeam()
    {
        gameObject.SetActive(false);
        isShoting = false;
        onBeamEnd?.Invoke();
    }

    public virtual void UpdateBeam()
    {
        if (isShoting)
        {
            if (lineRenderer)
            {
                lineRenderer.SetPosition(0, startTrans.position);
                lineRenderer.SetPosition(1, endTrans.position);
                if (beamImpact)
                    beamImpact.transform.position = endTrans.position;
            }

            eslapedBeamTime += Time.deltaTime;
            if (eslapedBeamTime > timeToLive)
            {
                StopBeam();
            }
            else
            {
                if (Time.time - hitTimePoint >= hitInterval)
                {
                    hitTimePoint = Time.time;
                    onHit?.Invoke();
                }
            }
        }

        
    }
}
