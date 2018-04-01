using UnityEngine;
using System.IO;
using System.Diagnostics;

/// <summary>
/// A class containing methods using standard runtime texturing
/// <author>Richard Gono</author>
/// </summary>
public class SimpleTexturing : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // for testing purposes
        //testApplyTexture();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // press ESC to quit
        if (Input.GetKey("escape"))
          Application.Quit();

        // use WASD + QE to move
        float xAxisValue = Input.GetAxis("Horizontal");
        float zAxisValue = Input.GetAxis("Vertical");
        bool turnLeft = Input.GetKey(KeyCode.Q);
        bool turnRight = Input.GetKey(KeyCode.E);
        if (Camera.current != null)
        {
            Camera.current.transform.Translate(new Vector3(xAxisValue / 2, 0.0f, zAxisValue / 2));
            if (turnLeft)
                Camera.current.transform.Rotate(0, -1.5f, 0);
            if (turnRight)
                Camera.current.transform.Rotate(0, 1.5f, 0);
        }
    }

    /// <summary>
    /// Loads the texture and normal map and sets them 100 times in a row to get the average time
    /// </summary>
    void testApplyTexture()
    {
        double result = 0;
        for (int i = 0; i < 100; i++)
        {
            result += ApplyTexture();
        }

        UnityEngine.Debug.Log("Normal texturing took on average " + result / 100 + "ms");
    }

    /// <summary>
    /// Loads the diffuse texture, sets it, loads the normal map, sets it
    /// </summary>
    /// <returns></returns>
    public long ApplyTexture ()
    {
        long millis = 0;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        // Load diffuse texture
        Texture2D diffuseTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        byte[] byteData = File.ReadAllBytes("assets/texture.png");
        diffuseTexture.LoadImage(byteData);

        diffuseTexture.Apply();

        sw.Stop();
        millis += sw.ElapsedMilliseconds;
        sw.Reset();
        sw.Start();

        // Load the normal map
        Texture2D normalTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        byteData = File.ReadAllBytes("assets/normal.png");
        normalTexture.LoadImage(byteData);

        normalTexture.Apply();

        sw.Stop();
        millis += sw.ElapsedMilliseconds;

        // Set the diffuse and normal map
        GetComponent<Renderer>().material.mainTexture = diffuseTexture;
        GetComponent<Renderer>().material.shaderKeywords = new string[1] { "_NORMALMAP" };
        GetComponent<Renderer>().material.SetFloat("_Glossiness", 0f);
        GetComponent<Renderer>().material.SetFloat("_BumpScale", 0.55f);
        GetComponent<Renderer>().material.SetTexture("_BumpMap", normalTexture);

        return millis;
    }

    public void ApplySimpleTexturing()
    {
        ApplyTexture();
    }
}
