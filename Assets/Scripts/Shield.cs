using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield
{
    int id;
    float amount;
    // timer -1 = no
    float timer = 0;
    float duration;

    public Shield(float amount, float duration)
    {
        this.amount = amount;
        this.duration = duration;
    }
    public bool StillActive()
    {
        if(duration == -1)
        {
            if(amount > 0)
            {
                return true; 
            }
            else
            {
                return false;
            }
        }

        if(timer < duration)
        {
            if (amount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public void AddTime(float timePassed)
    {
        timer += timePassed;
    }
    public void SetAmount(float amount)
    {
        this.amount = amount;
    }
    public float GetAmount()
    {
        return amount;
    }
    public override string ToString()
    {
        return "Shield Amount: " + amount + " , Duration: " + duration + " , Timer: " + timer + " .";
    }
}
