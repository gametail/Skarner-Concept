using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    [Header("CD reset")]
    public KeyCode resetCD;

    private Unit unit;

    [Header("Ability 1")]
    public string a1name;
    public float a1casttime;
    public Image ability1Image;
    public Image ability1CooldownFill;
    public Text ability1CooldownTimer;
    public float cooldown1 = 3;
    public int a1Cost = 20;
    public int a1Range = 1400;
    bool isCooldown1 = false;
    public KeyCode ability1;

    [Header("Ability 2")]
    public string a2name;
    public float a2casttime;
    public Image ability2Image;
    public Image ability2CooldownFill;
    public Text ability2CooldownTimer;
    public float cooldown2 = 8;
    public int a2Cost = 40;
    bool isCooldown2 = false;
    public KeyCode ability2;
    public float exoBuffTimer = 0;
    public float exoDuration = 3;
    private bool exoActive = false;
    public GameObject currentMat;
    public Material skarner;
    public Material exoSkarner;
    public GameObject[] spirePF;

    [Header("Ability 3")]
    public string a3name;
    public float a3casttime;
    public Image ability3Image;
    public Image ability3CooldownFill;
    public Text ability3CooldownTimer;
    public float cooldown3 = 12;
    public int a3Cost = 30;
    public int a3Range = 4000;
    bool isCooldown3 = false;
    public KeyCode ability3;
    public Transform fracture;


    [Header("Ability 4")]
    public string a4name;
    public float a4casttime;
    public Image ability4Image;
    public Image ability4CooldownFill;
    public Text ability4CooldownTimer;
    public float cooldown4 = 60;
    public int a4Cost = 100;
    public int a4Range = 1800;
    bool isCooldown4 = false;
    public KeyCode ability4;
    public GameObject ImpalePos;
    public bool impaling = false;
    private GameObject target = null;
    public float dragSpeed;
    public float dragRange;
    private float impaleTimer = 0;
    public float impaleDuration = 2;
    private float ImpaleMsBonus = 0;


    //Indicators

    [Header("Indicator")]
    Vector3 position;
    public Transform player;
    public GameObject indicatorPos;
    public Image indicatorImage;
    public Sprite[] indicatorSprites = new Sprite[4];
    public GameObject rCircle;
    private int indicatorShown = 0;
    public bool lockedIndicator = true;
    public float Range = 3.3f;


    public GameObject[] spellCollider = new GameObject[4];

    [Header("Casting Bar")]
    private bool isCasting = false;
    public GameObject castingBar;
    public GameObject Fill;
    public Text spellname;
    public Text time;

    public Animator anim;

    private int lastSpell = 0;
    private Vector3 tmpPos = Vector3.zero;
    private Quaternion tmpRot = Quaternion.identity;
    private Vector3 tmpForward = Vector3.zero;

    private float ms = 5;
    private float msBoost = 0.16f;
    private bool onSpire;
    private Vector3 lastSpirePos = Vector3.zero;
    private int maxSpires = 6;

    public float scalar = 0;
    void Start()
    {
        ability1CooldownFill.fillAmount = 0;
        ability1CooldownTimer.text = "";
        ability2CooldownFill.fillAmount = 0;
        ability2CooldownTimer.text = "";
        ability3CooldownFill.fillAmount = 0;
        ability3CooldownTimer.text = "";
        ability4CooldownFill.fillAmount = 0;
        ability4CooldownTimer.text = "";

        unit = GetComponent<Unit>();
        
}

    // Update is called once per frame
    void Update()
    {
        Ability1();
        Ability2();
        Ability3();
        Ability4();
        updateCooldowns();
        castingBar.SetActive(isCasting);
        player.GetComponent<NavMeshAgent>().isStopped = isCasting;

        //W
        if (exoActive && exoBuffTimer <= exoDuration)
        {
            Debug.Log("Exo Active");
            GetComponent<NavMeshAgent>().speed = Mathf.Lerp(ms , ms + ms * msBoost, exoBuffTimer / exoDuration);
            currentMat.GetComponent<SkinnedMeshRenderer>().material = exoSkarner;

            //if (!onSpire && Vector3.Distance(transform.position, lastSpirePos) > 2f)
            if (!onSpire && Vector3.Distance(transform.position + transform.forward * scalar, lastSpirePos) > 2f)
            {

                //GameObject newSpire = Instantiate(spirePF, transform.position, Quaternion.identity);
                int random = UnityEngine.Random.Range(0, spirePF.Length);
                GameObject newSpire = Instantiate(spirePF[random], transform.position + transform.forward * scalar, Quaternion.identity);
                float random2 = UnityEngine.Random.Range(0f, 360f);
                newSpire.transform.eulerAngles = new Vector3(newSpire.transform.eulerAngles.x, random2, newSpire.transform.eulerAngles.z);
                lastSpirePos = newSpire.transform.position;
                maxSpires--;

            }


            exoBuffTimer += Time.deltaTime;
        }
        else
        {
            currentMat.GetComponent<SkinnedMeshRenderer>().material = skarner;
            GetComponent<NavMeshAgent>().speed = ms;
            exoBuffTimer = 0;
            exoActive = false;
        }


        //R
        anim.SetBool("isImpaling", impaling);
        if (impaling && impaleTimer <= impaleDuration && target != null)
        {
            Debug.Log("Impaled");
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.angularSpeed = 0;
            ImpaleMsBonus = 0.2f * target.GetComponent<Unit>().GetMS();
            agent.speed += ImpaleMsBonus;



            var direction = (target.transform.position - player.position).normalized;
            var lookRotation = Quaternion.LookRotation(direction);
            player.rotation = Quaternion.Slerp(player.rotation, lookRotation, Time.deltaTime * 40);

            if(Vector3.Distance(player.position, target.transform.position) > dragRange)
            {
                target.transform.position = Vector3.Lerp(target.transform.position, ImpalePos.transform.position, dragSpeed * Time.deltaTime);
            }
            impaleTimer += Time.deltaTime;
        }
        else
        {
            impaling = false;
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.angularSpeed = 1200;
            agent.speed -= ImpaleMsBonus;
            ImpaleMsBonus = 0;
            impaleTimer = 0;
        }

        if (Input.GetKeyDown(resetCD))
        {
            ResetCD();
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //indicator
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }

            Vector3 dir = position - player.position;
            float angle = Mathf.Atan2(dir.z,dir.x) * Mathf.Rad2Deg;
            
            if(lockedIndicator == true && indicatorShown != 4)
            {
                
                indicatorPos.transform.position = player.position + new Vector3(0, 0.001f,0);
                indicatorPos.transform.eulerAngles = new Vector3(0, 90 - angle, 0);
                
            }
            else
            {
                var hitPosDir = (hit.point - transform.position).normalized;
                float distance = Vector3.Distance(hit.point, transform.position);

                distance = Mathf.Min(distance, Range);

                var newHitPos = transform.position + hitPosDir * distance;
                indicatorPos.transform.position = newHitPos + new Vector3(0, 0.001f, 0);
                //indicatorPos.transform.position = position + new Vector3(0, 0.001f, 0);
                indicatorPos.transform.eulerAngles = new Vector3(0, 90 - angle, 0);

                Debug.DrawLine(player.position + new Vector3(0, 0.001f, 0), newHitPos + new Vector3(0, 0.001f, 0));
            }
        
    }

    void Ability1()
    {
        if (Input.GetKey(ability1) && isCooldown1 == false && unit.GetCurrentMana() - a1Cost >= 0 && isCasting == false)
        {
            enableIndicator(1);

            //Spell
        }
        //"cancel spell" remove indicator
        if (indicatorShown == 1 && Input.GetMouseButtonDown(1))
        {
            disableIndicator();
        }
        //"confirm spell" disable indicator, set on cd, cast spell, [missing] functionality
        if (indicatorShown == 1 && Input.GetMouseButtonDown(0) && unit.GetCurrentMana() - a1Cost >= 0)
        {
            if (isCasting == false)
            {
                lastSpell = 1;
                tmpPos = indicatorPos.transform.position;
                tmpRot = indicatorPos.transform.rotation;

                StartCoroutine(StartCast(indicatorShown, a1casttime));
            }
        }
        //"missing mana" 
        else if (Input.GetKey(ability1) && isCooldown1 == false && unit.GetCurrentMana() - a1Cost < 0)
        {
            Debug.Log("not enough Mana for Q");
        }
        //"on cooldown"
        else if (Input.GetKey(ability1) && isCooldown1 == true)
        {
            Debug.Log("Q on Cooldown");
        }

        //change hud icons if "no mana"
        if (unit.GetCurrentMana() - a1Cost < 0)
        {
            ability1Image.color = Color.blue;
        }
        else
        {
            ability1Image.color = Color.white;
        }
    }
    void Ability2()
    {

        if (Input.GetKey(ability2) && isCooldown2 == false && unit.GetCurrentMana() - a2Cost >= 0)
        {
            lastSpell = 2;
            isCooldown2 = true;
            unit.SpellCast(a2Cost);
            unit.TimeShield(unit.GetMaxHp()*0.2f, exoDuration);
            ability2CooldownFill.fillAmount = 1;
            disableIndicator();
            exoActive = true;
            anim.SetTrigger("UseW");

            Debug.Log("Cast W " + "-" + a2Cost + " Mana");
        }
        //MANA MANAGEMENT
        else if (Input.GetKey(ability2) && isCooldown2 == false && unit.GetCurrentMana() - a2Cost < 0)
        {
            Debug.Log("not enough Mana for W");
        }
        else if (Input.GetKey(ability2) && isCooldown2 == true)
        {
            Debug.Log("W on Cooldown");
        }

        if (unit.GetCurrentMana() - a2Cost < 0)
        {
            ability2Image.color = Color.blue;
        }
        else
        {
            ability2Image.color = Color.white;
        }
    }
    void Ability3()
    {
        if (Input.GetKey(ability3) && isCooldown3 == false && unit.GetCurrentMana() - a3Cost >= 0 && isCasting == false)
        {
            enableIndicator(3);

            //Spell
        }
        //"cancel spell" remove indicator
        if (indicatorShown == 3 && Input.GetMouseButtonDown(1))
        {
            disableIndicator();
        }
        //"confirm spell" disable indicator, set on cd, cast spell, [missing] functionality
        if (indicatorShown == 3 && Input.GetMouseButtonDown(0) && unit.GetCurrentMana() - a3Cost >= 0 && !impaling)
        {
            if (isCasting == false)
            {
                anim.SetTrigger("UseE");
                lastSpell = 3;

                tmpPos = indicatorPos.transform.position;
                tmpRot = indicatorPos.transform.rotation;
                tmpForward = indicatorPos.transform.forward;

                StartCoroutine(StartCast(indicatorShown, a3casttime));
                player.rotation = tmpRot;

            }
        }
        //"missing mana" 
        else if (Input.GetKey(ability3) && isCooldown3 == false && unit.GetCurrentMana() - a3Cost < 0)
        {
            Debug.Log("not enough Mana for E");
        }
        //"on cooldown"
        else if (Input.GetKey(ability3) && isCooldown3 == true)
        {
            Debug.Log("E on Cooldown");
        }

        //change hud icons if "no mana"
        if (unit.GetCurrentMana() - a3Cost < 0)
        {
            ability3Image.color = Color.blue;
        }
        else
        {
            ability3Image.color = Color.white;
        }
    }
    void Ability4()
    {
        if (Input.GetKey(ability4) && isCooldown4 == false && unit.GetCurrentMana() - a4Cost >= 0 && isCasting == false)
        {
            enableIndicator(4);

            //Spell
        }
        //"cancel spell" remove indicator
        if (indicatorShown == 4 && Input.GetMouseButtonDown(1))
        {
            disableIndicator();
        }
        //"confirm spell" disable indicator, set on cd, cast spell, [missing] functionality
        if (indicatorShown == 4 && Input.GetMouseButtonDown(0) && unit.GetCurrentMana() - a4Cost >= 0)
        {
            if(isCasting == false)
            {
                lastSpell = 4;
                tmpPos = indicatorPos.transform.position;
                tmpRot = indicatorPos.transform.rotation;

                StartCoroutine(StartCast(indicatorShown,a4casttime));
                player.rotation = tmpRot;
            }
            
        }
        //"missing mana" 
        else if (Input.GetKey(ability4) && isCooldown4 == false && unit.GetCurrentMana() - a4Cost < 0)
        {
            Debug.Log("not enough Mana for R");
        }
        //"on cooldown"
        else if (Input.GetKey(ability4) && isCooldown4 == true)
        {
            Debug.Log("R on Cooldown");
        }

        //change hud icons if "no mana"
        if (unit.GetCurrentMana() - a4Cost < 0)
        {
            ability4Image.color = Color.blue;
        }
        else
        {
            ability4Image.color = Color.white;
        }
    }
    void enableIndicator(int id)
    {
        indicatorImage.sprite = indicatorSprites[id - 1];
        indicatorImage.SetNativeSize();
        indicatorPos.GetComponentInChildren<Canvas>().transform.localPosition = new Vector3(0, 0, indicatorSprites[id - 1].texture.height / 2);
        indicatorImage.enabled = true;
        indicatorShown = id;

        if(id == 4)
        {
            indicatorPos.GetComponentInChildren<Canvas>().transform.localPosition = new Vector3(0, 0, 0);
            rCircle.SetActive(true);
        }
        else
        {
            rCircle.SetActive(false);
        }
    }
    void disableIndicator()
    {
        rCircle.SetActive(false);
        indicatorShown = 0;
        indicatorImage.enabled = false;



    }
    private void updateCooldowns()
    {
        if (isCooldown1)
        {
            ability1CooldownFill.fillAmount -= 1 / cooldown1 * Time.deltaTime;

            if (cooldown1 * ability1CooldownFill.fillAmount > 1)
            {
                ability1CooldownTimer.text = ((int)(cooldown1 * ability1CooldownFill.fillAmount + 0.5f)).ToString();
            }
            else
            {
                ability1CooldownTimer.text = Math.Round(cooldown1 * ability1CooldownFill.fillAmount, 1, MidpointRounding.ToEven).ToString();
            }

            ability1Image.color = Color.grey;

            if (ability1CooldownFill.fillAmount <= 0)
            {
                ability1CooldownFill.fillAmount = 0;
                ability1CooldownTimer.text = "";

                ability1Image.color = Color.white;

                isCooldown1 = false;
            }
        }
        if (isCooldown2)
        {
            ability2CooldownFill.fillAmount -= 1 / cooldown2 * Time.deltaTime;

            if (cooldown2 * ability2CooldownFill.fillAmount > 1)
            {
                ability2CooldownTimer.text = ((int)(cooldown2 * ability2CooldownFill.fillAmount + 0.5f)).ToString();
            }
            else
            {
                ability2CooldownTimer.text = Math.Round(cooldown2 * ability2CooldownFill.fillAmount, 1, MidpointRounding.ToEven).ToString();
            }

            ability2Image.color = Color.grey;

            if (ability2CooldownFill.fillAmount <= 0)
            {
                ability2CooldownFill.fillAmount = 0;
                ability2CooldownTimer.text = "";

                ability2Image.color = Color.white;

                isCooldown2 = false;
            }
        }
        if (isCooldown3)
        {

            ability3CooldownFill.fillAmount -= 1 / cooldown3 * Time.deltaTime;

            if (cooldown3 * ability3CooldownFill.fillAmount > 1)
            {
                ability3CooldownTimer.text = ((int)(cooldown3 * ability3CooldownFill.fillAmount + 0.5f)).ToString();
            }
            else
            {
                ability3CooldownTimer.text = Math.Round(cooldown3 * ability3CooldownFill.fillAmount, 1, MidpointRounding.ToEven).ToString();
            }

            ability3Image.color = Color.grey;

            if (ability3CooldownFill.fillAmount <= 0)
            {
                ability3CooldownFill.fillAmount = 0;
                ability3CooldownTimer.text = "";

                ability3Image.color = Color.white;

                isCooldown3 = false;
            }
        }
        if (isCooldown4)
        {

            ability4CooldownFill.fillAmount -= 1 / cooldown4 * Time.deltaTime;

            if (cooldown4 * ability4CooldownFill.fillAmount > 1)
            {
                ability4CooldownTimer.text = ((int)(cooldown4 * ability4CooldownFill.fillAmount + 0.5f)).ToString();
            }
            else
            {
                ability4CooldownTimer.text = Math.Round(cooldown4 * ability4CooldownFill.fillAmount, 1, MidpointRounding.ToEven).ToString();
            }

            ability4Image.color = Color.grey;

            if (ability4CooldownFill.fillAmount <= 0)
            {
                ability4CooldownFill.fillAmount = 0;
                ability4CooldownTimer.text = "";

                ability4Image.color = Color.white;

                isCooldown4 = false;
            }
        }
    }
    private IEnumerator StartCast(int id, float castTime)
    {
        isCasting = true;
        Fill.GetComponent<Image>().fillAmount = 0;

        

        if (id == 1)
        {
            spellname.text = a1name;
        }
        else if(id == 2)
        {
            spellname.text = a2name;
        }
        else if (id == 3)
        {
            spellname.text = a3name;
        }
        else if (id == 4)
        {
            spellname.text = a4name;
        }

        disableIndicator();
        StartCoroutine(Progress(id));

        yield return new WaitForSeconds(castTime);

        if(lastSpell == 3)
        {
            Transform fractureTransform = Instantiate(fracture, transform.position, Quaternion.identity);

            int random = UnityEngine.Random.Range(0, spirePF.Length);
            fractureTransform.GetComponent<Fracture>().Setup(transform.position, tmpForward, a3Range, spirePF[random]);
        }
        else if(lastSpell == 1 || lastSpell == 4)
        {
            StartCoroutine(CheckCollider(id));
        }
        

        StartCooldown(id);


        isCasting = false;
    }
    private IEnumerator CheckCollider(int id)
    {
        

        if (id == 1)
        {
            spellCollider[0].transform.position = tmpPos;
            spellCollider[0].transform.rotation = tmpRot;
            spellCollider[0].SetActive(true);
            spellCollider[3].SetActive(false);
            Debug.Log(id + " collider check");

        }
        else if (id == 4)
        {
            spellCollider[3].transform.position = tmpPos;
            spellCollider[3].transform.rotation = tmpRot;

            spellCollider[3].SetActive(true);
            spellCollider[0].SetActive(false);

            Debug.Log(id + " collider check");
        }
        else
        {
            spellCollider[0].SetActive(false);
            spellCollider[3].SetActive(false);

            Debug.Log("else collider check");
        }

        yield return new WaitForSeconds(0.02f);
        Debug.Log( id + " collider check done");
        spellCollider[0].SetActive(false);
        spellCollider[3].SetActive(false);

    }
    
    private IEnumerator Progress(int id)
    {
        float timePassed = Time.deltaTime;
        float rate = 0;
        if (id == 1)
        {
            rate = 1f / a1casttime;
        }
        else if (id == 2)
        {
            rate = 1f / a2casttime;
        }
        else if (id == 3)
        {
            rate = 1f / a3casttime;
        }
        else if (id == 4)
        {
            rate = 1f / a4casttime;
        }

        float progress = 0f;

        while(progress <= 1)
        {
            Fill.GetComponent<Image>().fillAmount = Mathf.Lerp(0,1,progress);
            if((1 / rate) - timePassed <= 0)
            {
                time.GetComponent<Text>().text = "";
            }
            else
            {
                time.GetComponent<Text>().text = (1 / rate - timePassed).ToString("F2").Replace(',', '.');  
            }

            progress += rate * Time.deltaTime;

            timePassed += Time.deltaTime;

            yield return null;
        }
    }
    void ResetCD()
    {
            ability1CooldownFill.fillAmount = 0;
            ability2CooldownFill.fillAmount = 0;
            ability3CooldownFill.fillAmount = 0;
            ability4CooldownFill.fillAmount = 0;
    }
    private void StartCooldown(int id)
    {
        if (id == 1)
        {
            isCooldown1 = true;
            unit.SpellCast(a1Cost);
            ability1CooldownFill.fillAmount = 1;
            Debug.Log("Cast Q " + "-" + a1Cost + " Mana");
        }
        else if (id == 2)
        {
            isCooldown2 = true;
            unit.SpellCast(a2Cost);
            ability2CooldownFill.fillAmount = 1;
        }
        else if (id == 3)
        {
            isCooldown3 = true;
            unit.SpellCast(a3Cost);
            ability3CooldownFill.fillAmount = 1;
            Debug.Log("Cast E " + "-" + a3Cost + " Mana");
        }
        else if (id == 4)
        {
            isCooldown4 = true;
            unit.SpellCast(a4Cost);
            ability4CooldownFill.fillAmount = 1;
            Debug.Log("Cast R " + "-" + a4Cost + " Mana");
        }
    }

    private IEnumerator startTimer()
    {
        yield return new WaitForSeconds(.25f);
        ms = 5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //Hit
            target = other.gameObject;
            if(lastSpell == 1)
            {
                Debug.Log("Damage with Q");
                other.gameObject.GetComponent<Unit>().Damage(200);
                Debug.Log(other.gameObject.name);
            }
            else if(lastSpell == 4)
            {
                impaling = true;
                Unit emy = other.gameObject.GetComponent<Unit>();
                emy.Damage(emy.GetMaxHp() * 0.2f);
                unit.TimeShield(emy.GetMaxHp() * 0.2f, impaleDuration);
                Debug.Log(other.gameObject.name);
            }
        }

    }
    
    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Spire"))
        {
            ms = 380f/67f;
            msBoost = 0.3f;
            onSpire = true;
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Spire"))
        {
            msBoost = 0.16f;
            onSpire = false;
            StartCoroutine(startTimer());
        }
    }
}
