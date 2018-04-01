using UnityEngine;

public class EmbedExample : MonoBehaviour {
	
	public Texture2D embedTexture;
	public TextAsset embedText;
	
	public Texture2D extractTexture;
	public string extractText;

    void Start() {

        embedTexture = Steganography.EmbedTexture(embedTexture, embedText);

        // Debug Display of string that was retrieved
        Debug.Log("String read from image Channels: \n" + Steganography.ExtractFromTexture(extractTexture,embedTexture,extractText));
    }
}
