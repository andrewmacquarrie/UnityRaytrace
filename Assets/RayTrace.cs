using UnityEngine;
using System.Collections;
using System;

public class RayTrace : MonoBehaviour
{
    //How much of our screen resolution we render at
    float RenderResolution = 1;

    private Texture2D renderTexture;
    private RaycastHit?[,] hitStore;

    //Create render texture with screen size with resolution
    void Awake()
    {
        renderTexture = new Texture2D((int) Math.Floor(Screen.width * RenderResolution), (int) Math.Floor(Screen.height * RenderResolution));
    }

    //Do one raytrace when we start playing
    void Start()
    {
        hitStore = new RaycastHit?[Screen.width, Screen.height];
        DoRayTrace();
    }

    //Real Time Rendering
    void Update()
    {
        UpdateRayTrace();
    }   

    //Draw the render
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTexture);
    }

    //The function that renders the entire scene to a texture
    void DoRayTrace()
    {
        for (int x = 0; x < renderTexture.width; x += 1)
        {
            for (int y = 0; y < renderTexture.height; y += 1)
            {

                //Now that we have an x/y value for each pixel, we need to make that into a 3d ray
                //according to the camera we are attached to
                Ray ray = camera.ScreenPointToRay(new Vector3(x / RenderResolution, y / RenderResolution, 0));

                //Now lets call a function with this ray and apply it's return value to the pixel we are on
                //We will define this function afterwards
                renderTexture.SetPixel(x, y, TraceRay(x,y,ray));
            }
        }

        renderTexture.Apply();
    }

    //The function that renders the entire scene to a texture
    void UpdateRayTrace()
    {
        for (int x = 0; x < renderTexture.width; x += 1)
        {
            for (int y = 0; y < renderTexture.height; y += 1)
            {
                renderTexture.SetPixel(x, y, UseHit(x, y));
            }
        }

        renderTexture.Apply();
    }

    //Trace a Ray for a singple point
    Color TraceRay(int x, int y, Ray ray)
    {
        //The color we change throught the function
        Color returnColor = Color.black;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            //The material of the object we hit
            Material mat;

            hitStore[x, y] = hit;

            //Set the used material
            mat = hit.collider.renderer.material;

            //if the material has a texture
            if (mat.mainTexture)
            {
                //return the color of the pixel at the pixel coordinate of the hit
                returnColor += (mat.mainTexture as Texture2D).GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);
            }
            else
            {
                //return the material color
                returnColor += mat.color;
            }
        }

        //The color of this pixel
        return returnColor;
    }


    //Trace a Ray for a singple point
    Color UseHit(int x, int y)
    {
        //The color we change throught the function
        Color returnColor = Color.black;

        if (hitStore[x,y] != null)
        {

            //The material of the object we hit
            Material mat;
            RaycastHit hit = hitStore[x, y].Value;

            //Set the used material
            mat = hit.collider.renderer.material;

            //if the material has a texture
            if (mat.mainTexture)
            {
                //return the color of the pixel at the pixel coordinate of the hit
                returnColor += (mat.mainTexture as Texture2D).GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);
            }
            else
            {
                //return the material color
                returnColor += mat.color;
            }
        }

        //The color of this pixel
        return returnColor;
    }

}
