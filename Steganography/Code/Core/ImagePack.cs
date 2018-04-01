using UnityEngine;
using System.Collections;
using System;

public static class ImagePack {

	// function puts text in the alpha channel of specified image (and overloads)
	public static void packImageData(this Texture2D img, string in_str) {
		img.packImageData(in_str, new Color(0,0,0,1)); // use alpha channel only by default
	}
	// make sure to use RGBA_chan as a MASK (only 0 or 1 as value), it will get raised to the highest number, but makes your own code less readable
	public static void packImageData(this Texture2D img, string in_str, Color RGBA_chan) {
		// store the amount of channels to use
		int useChannels = Mathf.CeilToInt(RGBA_chan.r) + Mathf.CeilToInt(RGBA_chan.g) + Mathf.CeilToInt(RGBA_chan.b) + Mathf.CeilToInt(RGBA_chan.a);
		if(useChannels == 0) { Debug.LogError("No channel allocated to store data, make sure to use atleast 1 channel!"); return; }
		// create an indice array, which stores what index goes to which channel
		int[] channel_index = new int[useChannels];
		int chan_check=0;
		for(int c=0; c<4; ++c) { if(RGBA_chan[c] == 1) { channel_index[chan_check] = c; chan_check++; } }
		// check if amount of data fits in texture
		int checkMaxData = (img.width * img.height) * useChannels; // multiply by amount of allowed channels
		char[] CharCheck = in_str.ToCharArray();
		if(CharCheck.Length > checkMaxData) { Debug.LogError("texture size too small to fit required text!"); return; }
		
		// encode string to bytes for fitting into texture
		byte[] bytes = Converter.EncodeStr(CharCheck);
		
		// store pixel data for image
		// this is a flat array of color data, left to right, bottom to top (!)
		Color[] pixels = img.GetPixels();
		// write byte data from string (converted above) into corresponding channel (channel to use is passed as Color parameter)
		for(int i=0; i<bytes.Length; ++i) {
			byte b = bytes[i];
			// get which pixel we need to write to, based on the amount of available channels
			int pix = Mathf.FloorToInt(i / useChannels);
			int index = channel_index[i % useChannels];
			// color expects a value between 0-1 (where 1 = 255), so we convert our byte to that range
			// when we unpack data, we'll convert it back to what it was
			// in addition, we set the Color values as if they were an array, instead of using their .r.g.b.a components, slightly easier in this case
			pixels[pix][index] = b / 255f;
		}
		// apply the changes to the image, by writing the bytes into the texture
		img.SetPixels(pixels);
		img.Apply();
		
		// maybe we need to make a "save dialog" here, to store the image somewhere yourself, so changes dont get reverted in the editor
	}
	
	// funtion returns an image and a text (both seperate) from specified image
	public static void unpackImageData(this Texture2D img, ref Texture2D out_img, out string out_str) {
		img.unpackImageData(ref out_img, out out_str, new Color(0,0,0,1)); // use alpha channel only by default
	}
	public static void unpackImageData(this Texture2D img, ref Texture2D out_img, out string out_str, Color RGBA_chan) {
		// store the amount of channels to use
		int useChannels = Mathf.CeilToInt(RGBA_chan.r) + Mathf.CeilToInt(RGBA_chan.g) + Mathf.CeilToInt(RGBA_chan.b) + Mathf.CeilToInt(RGBA_chan.a);
		if(useChannels == 0) { Debug.LogError("No channel allocated to store data, make sure to use atleast 1 channel!"); out_str = null; return; }
		// create an indice array, which stores what index goes to which channel
		int[] channel_index = new int[useChannels];
		int chan_check = 0;
		for(int c=0; c<4; ++c) { if(RGBA_chan[c] == 1) { channel_index[chan_check] = c; chan_check++; } }
		// check if target and source are the same size (for writing the image into)
		Color[] pixels = img.GetPixels();
		byte[] bytes = new byte[pixels.Length * useChannels];
		
		for(int i=0; i<bytes.Length; ++i) {
			int pix = Mathf.FloorToInt(i / useChannels);
			int index = channel_index[i % useChannels];
			// color expects a value between 0-1 (where 1 = 255), so we convert our byte to that range
			// when we unpack data, we'll convert it back to what it was
			bytes[i] = Convert.ToByte(pixels[pix][index] * 255f);
		}
		
		// apply the changes to the image, by writing the bytes into the texture (if a texture to write to was sepcified)
		if(out_img) {
			out_img.SetPixels(pixels);
			out_img.Apply();
		}
		// convert the bytes into readable text
		out_str = Converter.DecodeStr(bytes);
	}
}
