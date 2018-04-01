using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;

/// <summary>
/// A class containing methods using stego runtime texturing
/// <author>Richard Gono</author>
/// </summary>
public class StegoTexturing : MonoBehaviour {

	//void Start () {        testDecodeTextureWithNormal();/* for testing purposes */    }
	
    /// <summary>
    /// For testing purposes - repeats decoding a 100 times to ge taverage decoding time
    /// </summary>
    /// 
    /*
    void testDecodeTextureWithNormal(string savePath)
    {
        double result = 0;
        for (int i = 0; i < 100; i++)        {            result += DecodeTextureWithNormal(File.ReadAllBytes(savePath));        }
        UnityEngine.Debug.Log("Decoding took on average " + result/100 + "ms");
   //DecodeTextureWithNormal(File.ReadAllBytes("assets/stego2.png"));// reads the file, sets the diffuse texture, decodes and sets the normal map
    }
    */

    /// <summary>
    /// Encodes a texture with a normal map, saves the combined stego texture into a .png file, 
    /// then reads said file, sets it as the diffuse texture, 
    /// decodes the normal map from it and sets the normal map as the bump map
    /// </summary>
    public static void ApplyStegoTexturing(byte[] diffuseBytes, byte[] normalBytes, string savePath)
    {
        Texture2D newTexture = Steganography.EncodeImage(diffuseBytes,normalBytes);// combine texture with it's normal map, saves it to a file
        File.WriteAllBytes(savePath, newTexture.EncodeToPNG());
    }

    public static void ApplyStegoTexturingTest(byte[] diffuseBytes, string savePath) {
        Texture2D newTexture = Steganography.EncodeImageTest(diffuseBytes);
        File.WriteAllBytes(savePath, newTexture.EncodeToPNG());
    }

    public static void DecodeStegoTextureTest(byte[] diffuseBytes, string savePath) {
        Texture2D newTexture = Steganography.DecodeTextureWithNormal(diffuseBytes);
        File.WriteAllBytes(savePath, newTexture.EncodeToPNG());
    }
   
}
