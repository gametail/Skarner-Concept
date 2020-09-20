using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Resources : MonoBehaviour
{
    private const float BASE_HEALTH_SKARNER = 600f;
    private const float PL_HEALTH_SKARNER = 90f;
    private const float BASE_HEALTHREGEN_SKARNER = 9f;
    private const float PL_HEALTHREGEN_SKARNER = 0.85f;
    private const float BASE_MANA_SKARNER = 320f;
    private const float PL_MANA_SKARNER = 40f;
    private const float BASE_MANAREGEN_SKARNER = 7.5f;
    private const float PL_MANAREGEN_SKARNER = 0.45f;
    private const float BASE_ARMOR_SKARNER = 38f;
    private const float PL_ARMOR_SKARNER = 3.8f;
    private const float BASE_MR_SKARNER = 35f;
    private const float PL_MR_SKARNER = 1.25f;
    private const float BASE_MS_SKARNER = 335f;
    private const float BASE_AD_SKARNER = 65f;
    private const float PL_AD_SKARNER = 4.5f;
    private const float BASE_AS_SKARNER = 0.625f;
    private const float PL_AS_SKARNER = 2.1f / 100f;
    private const float BASE_RANGE_SKARNER = 125f;

    private enum DmgType{Physical, Magic, True};

    public GameObject player;

    [Header("Mods")]
    public KeyCode levelUp;
    public KeyCode levelDown;

    [Header("StatsUI")]
    public GameObject Stats;

    [Header("Health Sliders")]
    public Slider HealthSlider3D;
    public Slider HealthSlider2D;
    public Text HealthDisplay;
    public Text HpRegDisplay;
    public VisualSegments hpSeg;

    [Header("Mana Sliders")]
    public Slider ManaSlider3D;
    public Slider ManaSlider2D;
    public Text ManaDisplay;
    public Text ManaRegDisplay;
    public VisualSegments manaSeg;

    [Header("Level")]
    public Text LevelDisplay;

    public static int level = 1;
    public static int xp = 0;
    public static int nextLexel = 50;
    private float regenTimer = 0;
    public static float regenInterval = 1f;

    public static float maxHealth;
    public static float currentHealth;
    public static float currentHpReg;

    public static float maxMana;
    public static float currentMana;
    public static float currentManaReg;

    public static float currentArmor;
    public static float currentMR;

    private int refreshTimer = 0;
    private int refreshFrames = 30;
    void Start()
    {
        //init values
        maxHealth = BASE_HEALTH_SKARNER;
        currentHealth = maxHealth;
        currentHpReg = BASE_HEALTHREGEN_SKARNER;

        maxMana = BASE_MANA_SKARNER;
        currentMana = maxMana;
        currentManaReg = BASE_MANAREGEN_SKARNER;

        currentArmor = BASE_ARMOR_SKARNER;
        currentMR = BASE_MR_SKARNER;

        hpSeg.value = maxHealth;
        manaSeg.value = maxMana;


        //VISUAL
        HealthSlider3D.maxValue = HealthSlider2D.maxValue = maxHealth;

        ManaSlider3D.maxValue = ManaSlider2D.maxValue = maxMana;

        LevelDisplay.text = "1";

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(levelUp))
        {
            LevelUp();
        }
        if (Input.GetKeyDown(levelDown))
        {
            LevelDown();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Stats.SetActive(!Stats.activeSelf);
        }

        if(refreshTimer < 30)
        {
            refreshTimer++;
        }
        else
        {
            Stats.GetComponentInChildren<Text>().text = Mathf.Round((player.GetComponent<NavMeshAgent>().speed / (5f/335f))).ToString();
            refreshTimer = 0;
        }

        //RESOURCE REGEN
        if (regenTimer == 0)
        {
            //health regen
            if(currentHealth + (currentHpReg / 5) < maxHealth)
            {
                currentHealth += (currentHpReg / 5);
            }
            else if(currentHealth + (currentHpReg / 5) >= maxHealth)
            {
                currentHealth = maxHealth; 
            }

            //mana regen
            if (currentMana + (currentManaReg / 5) < maxMana)
            {
                currentMana += (currentManaReg / 5);
            }
            else if (currentMana + (currentManaReg / 5) >= maxMana)
            {
                currentMana = maxMana;
            }

            regenTimer += Time.deltaTime;
        }
        else if(regenTimer > regenInterval)
        {
            regenTimer = 0;
        }
        else
        {
            regenTimer += Time.deltaTime;
        }

            //Update VISUALS

            LevelDisplay.text = level.ToString();

            HealthSlider3D.value = HealthSlider2D.value = currentHealth;

            ManaSlider3D.value = ManaSlider2D.value = currentMana ;

            HealthDisplay.text = (int)(currentHealth + 0.5f) + "/" + (int)(maxHealth + 0.5f);
            ManaDisplay.text = (int)(currentMana + 0.5f) + "/" + (int)(maxMana + 0.5f);

            //REG VISUAL
            if(currentHealth < maxHealth)
            {
                double tmp = Math.Round((currentHpReg / 5), 1, MidpointRounding.ToEven);
                
                if(tmp % 1 == 0)
                {
                    HpRegDisplay.text = "+" + tmp.ToString() + ".0";
                }
                else
                {
                    HpRegDisplay.text = "+" + tmp.ToString().Insert(1, ".").Remove(2, 1);
            }
            }
            else
            {
                HpRegDisplay.text = "";
            }

            if (currentMana < maxMana)
            {
                double tmp = Math.Round((currentManaReg / 5), 1, MidpointRounding.ToEven);

                if(tmp % 1 == 0)
                {
                    
                    ManaRegDisplay.text = "+" + tmp.ToString() + ".0";
                }
                else
                {
                    ManaRegDisplay.text = "+" + tmp.ToString().Insert(1, ".").Remove(2, 1);
                }
            }
            else
            {
                ManaRegDisplay.text = "";
            }
    }

    public static void SpellCast(int manaCost)
    {
        currentMana -= manaCost;
    }
    static void TakeDamage(int dmgValue, DmgType dmgType)
    {
        if(dmgType == DmgType.Physical)
        {
            currentHealth -= (100 / (100 + currentArmor)) * dmgValue;
        }
        if(dmgType == DmgType.Magic)
        {
            currentHealth -= (100 / (100 + currentMR)) * dmgValue;
        }
        else
        {
            currentHealth -= dmgValue;
        }
    }
    public void LevelUp()
    {
        if(level < 18)
        {
            level++;
            currentHealth += PL_HEALTH_SKARNER;
            maxHealth += PL_HEALTH_SKARNER;
            currentHpReg += PL_HEALTHREGEN_SKARNER;

            currentMana += PL_MANA_SKARNER;
            maxMana += PL_MANA_SKARNER;
            currentManaReg += PL_MANAREGEN_SKARNER;

            HealthSlider3D.maxValue = HealthSlider2D.maxValue = maxHealth;
            ManaSlider3D.maxValue = ManaSlider2D.maxValue = maxMana;

            hpSeg.value = maxHealth;
            manaSeg.value = maxMana;
        }
    }
    public void LevelDown()
    {
        if (level > 1)
        {
            level--;
            maxHealth -= PL_HEALTH_SKARNER;
            maxMana -= PL_MANA_SKARNER;

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            if (currentMana > maxMana)
            {
                currentMana = maxMana;
            }

            currentHpReg -= PL_HEALTHREGEN_SKARNER;
            currentManaReg -= PL_MANAREGEN_SKARNER;

            HealthSlider3D.maxValue = HealthSlider2D.maxValue = maxHealth;
            ManaSlider3D.maxValue = ManaSlider2D.maxValue = maxMana;

            hpSeg.value = maxHealth;
            manaSeg.value = maxMana;
        }
    }
}
