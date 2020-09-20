using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int  health = 10;

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public void Damage(int dmg)
    {
        Debug.Log(name + " -2");
        health -= dmg;
    }
}
