using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{

    Vector3 newPosition;

    public float speed;
    public float walkRange;

    public GameObject model;
    void Start()
    {
        newPosition = this.transform.position;
        
        Instantiate(model, this.transform.position, Quaternion.identity, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        //right click -> raycast -> new pos
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.tag == "Ground")
            {
                newPosition = hit.point;

            }
        }

        
        if(Vector3.Distance(newPosition, this.transform.position) > walkRange)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, newPosition, speed * Time.deltaTime);
            Quaternion transRot = Quaternion.LookRotation(newPosition - this.transform.position, Vector3.up);
            this.GetComponentInChildren<Transform>().rotation = Quaternion.Slerp(transRot, this.GetComponentInChildren<Transform>().rotation, 0.7f);

        }
    }
}
