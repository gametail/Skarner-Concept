using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fracture : MonoBehaviour
{
    private Vector3 init;
    private Vector3 shootDir;
    private float range;
    private float moveSpeed = 14f;
    private Vector3 lastSpire;
    private GameObject spirePF;

    public void Setup(Vector3 init, Vector3 shootDir, float range, GameObject spirePF)
    {
        this.shootDir = shootDir;
        transform.rotation = Quaternion.LookRotation(shootDir.normalized);
        this.init = init;
        this.range = range;
        this.spirePF = spirePF;
    }
    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(init,transform.position) > range*2f / 1000f)
        {
            Destroy(gameObject);
        }
        transform.position += shootDir * moveSpeed * Time.deltaTime;
        
        if(lastSpire == null)
        {
            GameObject spire = Instantiate(spirePF, transform.position, Quaternion.identity);
            spire.GetComponent<Spire>().createdByFracture = true;
            float random2 = UnityEngine.Random.Range(0f, 360f);
            spire.transform.eulerAngles = new Vector3(spire.transform.eulerAngles.x, random2, spire.transform.eulerAngles.z);
            lastSpire = spire.transform.position;

        }
        else if(Vector3.Distance(transform.position,lastSpire) > 1.8f)
        {
            GameObject spire = Instantiate(spirePF, transform.position, Quaternion.identity);
            spire.GetComponent<Spire>().createdByFracture = true;
            lastSpire = spire.transform.position;
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            Debug.Log(col.gameObject.name + " hit");
            moveSpeed *= 0.5f;
            Unit emy = col.gameObject.GetComponent<Unit>();
            emy.Damage(140f);
        }
        if (col.CompareTag("Spire"))
        {
            lastSpire = col.transform.position;
        }
    }
}
