using UnityEngine;

public class ParentScript : MonoBehaviour
{

    public void CollisionDetected(ChildScript childScript)
    {
        Debug.Log("child collided");
    }

}