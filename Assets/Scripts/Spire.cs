using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Spire : MonoBehaviour
{
    private bool enemyAbove = false;
    public bool createdByFracture = false;
    float timer = 0;
    float lifetime = 0.1f;

    void Update()
    {
        if (createdByFracture)
        {
            lifetime = 2f;
            enemyAbove = true;
        }

        if (enemyAbove)
        {
            if(timer < lifetime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            timer = 0;
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy")){
            enemyAbove = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            enemyAbove = false;
        }
    }
}
