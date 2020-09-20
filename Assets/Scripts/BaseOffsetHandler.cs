using System.Collections;
using UnityEngine;

/// <summary> /// Insures that agent will move as close to the ground as possible /// </summary> 
public class BaseOffsetHandler : MonoBehaviour {

UnityEngine.AI.NavMeshAgent agent;
public enum Frequency
{
    High = 0,
    Normal = 1,
    Low = 2,
}
public Frequency frequncy = Frequency.Low;

float _wait = 99;
float originalOffset = 0;
void OnEnable()
{
    StartCoroutine(UpdateOffset());
}

int iterationCounter = 0;
IEnumerator UpdateOffset()
{
    if (agent == null)
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            if (iterationCounter < 3)
            {
                yield return new WaitForSeconds(1);
                StartCoroutine(UpdateOffset());
            }
            else Destroy(this);
            iterationCounter++;
            yield break;
        }

        switch (frequncy)
        {
            case Frequency.High:
                _wait = Random.Range(0.5f, 0.75f);
                break;
            case Frequency.Normal:
                _wait = Random.Range(1f, 1.3f);
                break;
            case Frequency.Low:
                _wait = Random.Range(2f, 3f);
                break;
        }
        originalOffset = agent.baseOffset;
    }

    yield return new WaitForSeconds(_wait);
    if (agent != null && agent.enabled)
    {
        RaycastHit hit = GetHit(agent.nextPosition + Vector3.up, Vector3.up * -1, 5, LayerMask.GetMask(new string[] { "Default", "NavIgnored" }));
        if (hit.transform != null)
        {
            float yDiff = agent.nextPosition.y - hit.point.y;
            if (Mathf.Abs(yDiff) > 0.075f)
            {
                float offset = (yDiff * -1) + originalOffset;
                if (offset > 0.99f) offset = 0.99f;
                else if (offset < -0.99f) offset = -0.99f;
                if (Mathf.Abs(offset - agent.baseOffset) > 0.015f)
                {
                    agent.baseOffset = offset;
                    originalOffset = offset;
                }
            }
        }
    }
    StartCoroutine(UpdateOffset());
}

/// <summary>
/// A simple Raycast
/// </summary>
/// <param name="from">Ray start position</param>
/// <param name="direction">Ray direction</param>
/// <param name="maxDist">How far it can travel</param>
/// <param name="layerMask">The layers it can hit</param>
/// <returns></returns>
RaycastHit GetHit(Vector3 from, Vector3 direction, float maxDist, int layerMask)
{
    Ray ray = new Ray(from, direction);
    RaycastHit hit;
    Physics.Raycast(ray, out hit, maxDist, layerMask, QueryTriggerInteraction.Ignore);
    return hit;
}
}