using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ConstantScale : MonoBehaviour
{

    public float scale = 1;
    void Update()
    {
        GetComponent<Transform>().localScale = Vector3.one * scale;
    }
}
