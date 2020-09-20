using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTransform : MonoBehaviour
{
    public GameObject obj;
    void Start()
    {
        this.transform.position = obj.transform.position;
        this.transform.rotation = obj.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = obj.transform.position;
        this.transform.rotation = obj.transform.rotation;
    }
}
