using UnityEngine;

public class EncoderExample : MonoBehaviour {
	
	public string stringToEncode = "æbcdefghijklmnøpqrstuvwxyz - ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    void Start() {

        Debug.Log("Raw encoded bytes: \n" + Steganography.EncodeString(stringToEncode));

        // display debug data        
        Debug.Log("Raw decoded bytes: \n" + Steganography.DecodeString(Converter.EncodeStr(stringToEncode.ToCharArray())));
    }
}
