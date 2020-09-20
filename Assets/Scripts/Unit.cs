using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [SerializeField] private Color hpColor = Color.red;
    [SerializeField] private Slider hpSlider = null;
    [SerializeField] private RectTransform hpRect = null;
    [SerializeField] private Image hpFill = null;
    [SerializeField] private VisualSegments hpSeg = null;
    [SerializeField] private float maxHp = 5000f;
    private float currentHp;
    [SerializeField] private float hpreg = 5f;
    [SerializeField] private bool hasMana = true;
    [SerializeField] private Color manaColor = Color.blue;
    [SerializeField] private Slider manaSlider = null;
    //[SerializeField] private RectTransform manaRect = null;
    [SerializeField] private Image manaFill = null;
    [SerializeField] private VisualSegments manaSeg = null;
    [SerializeField] private float maxMana = 1000f;
    private float currentMana;
    [SerializeField] private float manareg = 5f;

    [SerializeField] private Color shieldColor = Color.gray;
    [SerializeField] private Slider shieldSlider = null;
    [SerializeField] private RectTransform shieldRect = null;
    [SerializeField] private Image shieldFill = null;
    private float shield;
    private bool hasShield;
    private Stack<Shield> shields = new Stack<Shield>();
    private List<Shield> shields2 = new List<Shield>();

    [SerializeField] private static float regenInterval = 1f;
    private float regenTimer = 0f;

    [SerializeField] private float ms = 5;


    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        currentHp = maxHp;
        hpSeg.value = maxHp;
        hpSlider.maxValue = maxHp;
        hpFill.color = hpColor;

        shieldSlider.maxValue = 0;
        shieldFill.color = shieldColor;
        

        if (hasMana)
        {
            currentMana = maxMana;
            manaSeg.value = maxMana;
            manaSlider.maxValue = maxMana;
            manaFill.color = manaColor;

        }
    }
    void Update()
    {
        Debug.Log(shields.Count);

        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }

        //Shield List 
        if(shields.Count() == 0)
        {
            hasShield = false;
        }
        else
        {
            hasShield = true;
            UpdateShieldStack(shields);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Damage(50);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            TimeShield(400, 10);
        }


        //resource regen -------------------------------------------------------------------------------
        if (regenTimer == 0)
        {
            //health regen
            if (currentHp + (hpreg / 5) < maxHp)
            {
                currentHp += (hpreg / 5);
            }
            else if (currentHp + (hpreg / 5) >= maxHp)
            {
                currentHp = maxHp;
            }

            //mana regen
            if (currentMana + (manareg / 5) < maxMana && hasMana)
            {
                currentMana += (manareg / 5);
            }
            else if (currentMana + (manareg / 5) >= maxMana && hasMana)
            {
                currentMana = maxMana;
            }

            regenTimer += Time.deltaTime;
        }
        else if (regenTimer > regenInterval)
        {
            regenTimer = 0;
        }
        else
        {
            regenTimer += Time.deltaTime;
        }
        // ---------------------------------------------------------------------------------------------
        
        //Update VISUALS     
        if(hasShield)
        {
            shieldSlider.enabled = true;

            if (currentHp + shield >= maxHp)
            {

                shieldSlider.value = shield;
                shieldSlider.maxValue = shield;

                hpSlider.value = currentHp;
                hpSlider.maxValue = currentHp + shield;

                //left + 
                shieldRect.offsetMin = new Vector2(1600 * (hpSlider.value/hpSlider.maxValue), hpRect.offsetMin.y);
            }       
            else
            {
                shieldSlider.value = shield;
                shieldSlider.maxValue = maxHp - currentHp;

                hpSlider.value = currentHp;
                hpSlider.maxValue = maxHp;

                //left + 
                shieldRect.offsetMin = new Vector2(1600 * (hpSlider.value / hpSlider.maxValue), hpRect.offsetMin.y);       
            }
        }
        else
        {
            //left + 
            shieldRect.offsetMin = new Vector2(1600, hpRect.offsetMin.y);
            shieldSlider.enabled = false;

            hpSlider.value = currentHp;
            hpSlider.maxValue = maxHp;

        }
        UpdateHpSegments(maxHp + shield);

        if (hasMana)
        {
            manaSlider.value = currentMana;
        }

    }

    public void Damage(float value)
    {

        if (hasShield)
        {
            if(shield - value == 0)
            {
                hasShield = false;
                shield = 0;
                shields.Clear();
                UpdateHpSegments(maxHp);
            }
            else if(shield - value < 0)
            {
                hasShield = false;
                shield -= value;
                currentHp += shield;
                shield = 0;
                shields.Clear();

                UpdateHpSegments(maxHp);
            }
            else
            {
                shield -= value;
                float value_copy = value;
                Shield last = shields.Pop();

                while (value_copy > 0)
                { 
                    if(last.GetAmount() - value_copy > 0)
                    {
                        last.SetAmount(last.GetAmount() - value_copy);
                        shields.Push(last);
                        value_copy = 0;
                    }
                    else
                    {
                        value_copy -= last.GetAmount();
                        last = shields.Pop();
                    }
                }
                
                UpdateHpSegments(maxHp + shield);
            }
        }
        else
        {
            currentHp -= value;
        }

        if(GetComponent<Animator>() != null)
        {
            anim.Play("Hit");
        }
    }
    public void SpellCast(float manaCost)
    {
        currentMana -= manaCost;
    }

    public void Shield(float value)
    {
        hasShield = true;
        shield += value;
        Debug.Log("Added shield " + value + " new Shield: " + shield);


        shields.Push(new Shield(value, -1));

        UpdateHpSegments(maxHp + shield);  
    }
    public void TimeShield(float value, float time)
    {
        StartCoroutine(ShieldTimer(value, time));
        hasShield = true;
        shield += value;

        shields.Push(new Shield(value, time));

        Debug.Log("Added shield " + value + " , for " + time + " seconds, new Shield: " + shield);

        UpdateHpSegments(maxHp + shield);
    }
    private IEnumerator ShieldTimer(float value, float time)
    {
        float oldShield = shield;

        yield return new WaitForSeconds(time);
        if(shield > oldShield)
        {
            shield -= shield - oldShield;
        }
    }
    public void UpdateHpSegments(float hp)
    {
        hpSeg.value = hp;
    }
    public float GetMaxHp()
    {
        return maxHp;
    }
    public float GetCurrentHp()
    {
        return currentHp;
    }public float GetCurrentMana()
    {
        return currentMana;
    }
    public float GetShield()
    {
        return shield;
    }

    public float GetMS()
    {
        return ms;
    }

    public float getShield()
    {
        float result = 0;

        foreach(Shield shield in shields)
        {
            result += shield.GetAmount();
        }

        return result;
    }

    public void UpdateShieldStack(Stack<Shield> shields)
    {
        if (shields != null)
        {
            Stack<Shield> newStack = new Stack<Shield>();
            float shieldSum = 0;
            foreach (Shield shield in shields.ToList())
            {
                if (shield.StillActive())
                {
                    shield.AddTime(Time.deltaTime);
                    shieldSum += shield.GetAmount();
                    newStack.Push(shield);
                }
                else
                {
                    Debug.Log("Removed " + shield);
                }
            }
            shield = shieldSum;
            shields = newStack;
        }
    }
}
