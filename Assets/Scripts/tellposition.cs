using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class tellposition : MonoBehaviour
{
    private void Update()
    {
        Debug.Log(gameObject.transform.position);
    }
}