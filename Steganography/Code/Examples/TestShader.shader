Shader "Tutorial/Textured Colored" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,0.5)
		_MainTex("Texture", 2D) = "white" { }
		// represents the bit shift needed to move the 4 LSBs to MSBs in 256 bit color channel, 16 means a 4 bit shift to the left
		_Shift("Shift", float) = 1
	}
	SubShader{
		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float _Shift;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float4 _MainTex_ST;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				
				// sample texture at the current position
				fixed4 texcol = tex2D(_MainTex, i.uv);
				// get the red, green and blue color components and shift them to the left to uncover the LSBs
				fixed rstego = fmod(floor(texcol.r * 256) * _Shift, 256) / 256;
				fixed gstego = fmod(floor(texcol.g * 256) * _Shift, 256) / 256;
				fixed bstego = fmod(floor(texcol.b * 256) * _Shift, 256) / 256;
				// apply newly decoded color
				fixed4 newtexcol = fixed4(rstego, gstego, bstego, texcol.a);
				return newtexcol * _Color;
			}
			ENDCG
		}
	}
}
