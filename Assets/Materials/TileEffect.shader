Shader "Unlit/TileEffect1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FinalScale ("FinalScale", float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			uniform float _Scale = 0.1f;
			uniform float _ttt = 0.2f;
			uniform float _FinalScale;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col = 1-col;
				//float f = (1.0f-i.uv.x/50.0f) - _FinalScale;
				float f = (i.uv.x/20.0f) - _FinalScale;
				if(f<0.0f && i.uv.y>0.35f){
					col = 1.0f-col;
				}

				//col = (1.0f - i.uv.x/50.0f)-_FinalScale;
				//col = _FinalScale;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
