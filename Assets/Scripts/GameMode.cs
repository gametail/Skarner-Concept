using UnityEngine;

public class GameMode : MonoBehaviour
{
    public int framerate = 144;
    public GameObject dummy; 
    // Start is called before the first frame update
    private void Awake()
    {
        Application.targetFrameRate = framerate;
        //QualitySettings.vSyncCount = 0;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(dummy, Vector3.zero, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
