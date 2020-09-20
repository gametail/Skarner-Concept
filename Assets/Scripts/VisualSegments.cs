using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualSegments : MonoBehaviour
{
    public Texture2D tex;
    
    private int barWidth = 1600;
    private int barHeight = 200;

    //get max hp
    public float value = 1000;
    private float valueLastFrame;


    public int segmentSize = 100;
    public int lineWidth = 16;
    public bool update = false;

    private Color[] resetColorArray;
    Color32 resetColor = new Color32(255, 255, 255, 0);


    void Start()
    {
        tex = new Texture2D(1600, 200, TextureFormat.ARGB32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;

        resetColorArray = tex.GetPixels();

        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = resetColor;
        }

        valueLastFrame = value;
        UpdateLines();
        GetComponent<RawImage>().texture = tex;

    }

    // Update is called once per frame
    void Update()
    {
        //only update if value changes
        if (value != valueLastFrame || update == true)
        {
            UpdateLines();
            GetComponent<RawImage>().texture = tex;
            valueLastFrame = value;
            update = false;
        }
    }
    void UpdateLines()
    {
        tex.SetPixels(resetColorArray);

        

        float pixelsPerValue = barWidth / value;  
        int segmentOffset = (int)(pixelsPerValue * segmentSize); 
        int numberOfLines = (int)(value / segmentSize);  

        int smallLine = lineWidth;
        int bigLine = lineWidth * 2;

        for (int i = 1; i <= numberOfLines; i++)
        {
            int currentOffset = i * segmentOffset; 
            

            for (int y = 0; y < barHeight; y++)
            {
                
                //big
                if (i % 10 == 0)
                {
                    for (int x = currentOffset; x < currentOffset + bigLine; x++)
                    {
                        tex.SetPixel(x, y, Color.black);
                    }
                }
                //small
                else
                {
                    for (int x = currentOffset; x < currentOffset + smallLine; x++)
                    {
                        if(y > barHeight / 2f)
                        {
                            tex.SetPixel(x, y, Color.black);
                        }
                    }
                }
            }

        }
        tex.Apply();
    }
}
