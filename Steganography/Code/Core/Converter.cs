using UnityEngine;
using System;
using System.Text;
using System.Collections;

public class Converter : MonoBehaviour {

	// function that accepts a string, converts it to chars, and returns a byte array
	// we can use the bytes to write to individual pixels (RGBA)
	static public byte[] EncodeStr(char[] char_arr) {
		// prepare encoding type (UTF8)
		Encoder uniEncoder = Encoding.UTF8.GetEncoder();
		
		// get amount of bytes required for all characters
        int byteCount = uniEncoder.GetByteCount(char_arr, 0, char_arr.Length, true);
        byte[] bytes = new byte[4+byteCount]; // create new byte array for all characters PLUS 4 extra bytes to store the total length
		
		// turn length (value) into bytes (4-bit)
		// stores the length of the byte array (inserted at the start, so we know when reading an image how many "pixels" to read)
		bytes[0] = (byte)(byteCount >>  0 & 0xFF);
		bytes[1] = (byte)(byteCount >>  8 & 0xFF);
		bytes[2] = (byte)(byteCount >> 16 & 0xFF);
		bytes[3] = (byte)(byteCount >> 24 & 0xFF);
		// store all characters into bytes array (starting a the 5th bit, so we dont override the length storage)
        uniEncoder.GetBytes(char_arr, 0, char_arr.Length, bytes, 4, true);
		
		return bytes;
	}
	
	// function that accepts a byte array, and turns it back into a string
	static public string DecodeStr(byte[] b_arr) {
		// prepare decoding type (UTF8)
		Decoder uniDecoder = Encoding.UTF8.GetDecoder();

		// turn bits (4) into length (value)
		UInt32 length = 
			(UInt32)(b_arr[0] <<  0) | 
			(UInt32)(b_arr[1] <<  8) | 
			(UInt32)(b_arr[2] << 16) | 
			(UInt32)(b_arr[3] << 24) ;
		
		// get amount of characters required from byte array (starting at index 4, because that's where we stored the length
        int charCount = uniDecoder.GetCharCount(b_arr, 4, (int)length);
        char[] chars = new char[charCount]; // create new array to store the right amount of characters
		// convert bytes into array of characters, starting at index 4 again, because we dont need to display the length data
        uniDecoder.GetChars(b_arr, 4, (int)length, chars, 0); 
		
		return new string(chars);
	}
}
