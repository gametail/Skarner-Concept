using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.XR;

public class Movement : MonoBehaviour
{
    NavMeshAgent agent;
    private bool isImpaled;
    public Animator anim;
    private bool isWalking;

    public float rotateSpeedMovement = 0.1f;
    float rotateVelocity;
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isWalking", agent.hasPath);

        isImpaled = GetComponent<Abilities>().impaling;

        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                agent.SetDestination(hit.point);
                if(agent.isStopped == false && isImpaled == false)
                {
                    Quaternion rotationToLookAt = Quaternion.LookRotation(hit.point - transform.position);
                    float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationToLookAt.eulerAngles.y, ref rotateVelocity, rotateSpeedMovement * Time.deltaTime * 5);

                    transform.eulerAngles = new Vector3(0, rotationY, 0);
                }
            }

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            agent.SetDestination(transform.position);
        }
    }
}
