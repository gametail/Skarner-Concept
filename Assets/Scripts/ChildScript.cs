using UnityEngine;

public class ChildScript : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        transform.parent.GetComponent<ParentScript>().CollisionDetected(this);
    }

}
