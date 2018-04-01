using UnityEngine;

public class Steganography {

    // Use this for initialization
    public static Texture2D EmbedTexture(Texture2D embedTexture, TextAsset embedText)
    {
        Color useChannels = new Color(0, 0, 0, 1);
        // store text data from a file into the image
        embedTexture.packImageData(embedText.text, useChannels);
        return embedTexture;
    }

    public static string ExtractFromTexture(Texture2D extractTexture, Texture2D embedTexture, string extractText)
    {
        Color useChannels = new Color(0, 0, 0, 1);
        // retrieve the image and text from the texture
        embedTexture.unpackImageData(ref extractTexture, out extractText, useChannels);

        return extractText;
    }

    // Use this for initialization
    public static string EncodeString(string stringToEncode)
    {
        // turn string into bytes
        byte[] byte_arr = Converter.EncodeStr(stringToEncode.ToCharArray());

        // display debug data
        string displayBytes = "";
        foreach (byte b in byte_arr)
        {
            displayBytes += b.ToString() + ", ";
        }

        return displayBytes;
    }

    public static string DecodeString(byte[] byte_arr)
    {
        // read bytes to string
        string decode_str = Converter.DecodeStr(byte_arr);
        return decode_str;
    }

    /// <summary>
    /// Just for testing purposes, sets the 4 LSBs of all pixels of an image to zeroes.
    /// </summary>
    public static Texture2D EncodeImageTest(byte[] imageBytes)
    {
        // load the diffuse map
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        byte[] byteImage = imageBytes;
        texture.LoadImage(byteImage);

        // apply the diffuse texture
        texture.Apply();


        Color32[] texturePixels = texture.GetPixels32();

        // iterate through all the pixels (normal and diffuse maps should have the same size)
        for (int i = 0; i < texturePixels.Length; i++)
        {

            // shift right by 4 positions nad then left by 4 positions to zero out the 4 LSBs
            texturePixels[i].r = (byte)(texturePixels[i].r >> 4);
            texturePixels[i].r = (byte)(texturePixels[i].r << 4);

            // repeat for green and blue color components
            texturePixels[i].g = (byte)(texturePixels[i].g >> 4);
            texturePixels[i].g = (byte)(texturePixels[i].g << 4);

            texturePixels[i].b = (byte)(texturePixels[i].b >> 4);
            texturePixels[i].b = (byte)(texturePixels[i].b << 4);
        }

        // set the texture
        texture.SetPixels32(texturePixels);
        texture.Apply();
        return texture;
    }

    /// <summary>
    /// Encodes the normal map into the diffuse map.
    /// Puts the normals 4 MSBs into the 4 LSBs of the diffuse map.
    /// Also effectively shifts the blue color component of the normal map from the interval (128, 255) to (0, 127)
    /// </summary>
    public static Texture2D EncodeImage(byte[] imageBytes, byte[] normalBytes)
    {
        // load the diffuse map
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        byte[] byteImage = imageBytes;
        texture.LoadImage(byteImage);
        // apply the diffuse texture
        texture.Apply();

        // load the normal map
        Texture2D normalTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        byte[] byteNormalImage = normalBytes; //File.ReadAllBytes("assets/normal.png");
        normalTexture.LoadImage(byteNormalImage);

        Color32[] normalPixels = normalTexture.GetPixels32();
        Color32[] texturePixels = texture.GetPixels32();

        // iterate through all the pixels (normal and diffuse maps should have the same size)
        for (int i = 0; i < normalPixels.Length; i++)
        {
            // shift all the color bytes to the right to prepare them for encoding - the 4 MSBs are now zero
            byte redNormalByte = (byte)(normalPixels[i].r >> 4);
            byte greenNormalByte = (byte)(normalPixels[i].g >> 4);
            byte blueNormalByte = (byte)((normalPixels[i].b >> 4) & 247);// the shifted blue component is effectively moved from the interval (128, 255) to (0, 127) by the mask 1b110111

            // shift right by 4 positions nad then left by 4 positions to zero out the 4 LSBs
            texturePixels[i].r = (byte)(texturePixels[i].r >> 4);
            texturePixels[i].r = (byte)(texturePixels[i].r << 4);
            texturePixels[i].r = (byte)(texturePixels[i].r | redNormalByte);// bitwise OR "connects" the diffuse and normal byte

            // repeat for green and blue color components
            texturePixels[i].g = (byte)(texturePixels[i].g >> 4);
            texturePixels[i].g = (byte)(texturePixels[i].g << 4);
            texturePixels[i].g = (byte)(texturePixels[i].g | greenNormalByte);

            texturePixels[i].b = (byte)(texturePixels[i].b >> 4);
            texturePixels[i].b = (byte)(texturePixels[i].b << 4);
            texturePixels[i].b = (byte)(texturePixels[i].b | blueNormalByte);
        }

        // set the texture
        texture.SetPixels32(texturePixels);
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Decodes the stego texture. Sets the remainder of the cover object as the diffuse texture.
    /// Sets the decoded part as the normal map. During decoding shifts the blue interval from (0, 127) to (128, 255)
    /// </summary>
    public static Texture2D DecodeTextureWithNormal(byte[] difusseData)
    {

        // load stego texture
        Texture2D diffuseTexture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        byte[] byteData = difusseData;
        diffuseTexture.LoadImage(byteData);

        diffuseTexture.Apply();

        // create a second texture object for the normal map
        Texture2D normalTexture = new Texture2D(diffuseTexture.width, diffuseTexture.height, TextureFormat.ARGB32, true);
        Color32[] pixels = diffuseTexture.GetPixels32();

        // iterate throught the pixels of the stego texture
        for (int i = 0; i < pixels.Length; i++)
        {
            // decode the normal maps pixels by shifting the stego pixels by 4, to move the 4 LSBs into the 4 MSBs
            pixels[i].r = (byte)(pixels[i].r << 4);
            pixels[i].g = (byte)(pixels[i].g << 4);
            pixels[i].b = (byte)((pixels[i].b << 4) | 128);// add the "128" bit to move the blue interval from (0, 127) to (128, 255)
        }
        // set pixels to the normal map
        normalTexture.SetPixels32(pixels);
        normalTexture.Apply();

        return normalTexture;
    }

}
