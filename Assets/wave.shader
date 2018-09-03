Shader "xin/wave"
{
	Properties
	{
		_WaveHeight("Wave Height", float) = 3.5
		[NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 0.5)
	}
	SubShader
	{
		Tags{"Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			float  _WaveHeight;
			float4 _Color;

			#include "UnityCG.cginc"
            #include "Lighting.cginc"

			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				SHADOW_COORDS(1) 
				UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION;
				fixed3 diff : COLOR0;
				fixed3 ambient : COLOR1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata_base  v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);


				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.pos.y += sin(worldPos.x+ _Time.w)*_WaveHeight;
				o.pos.y += cos(worldPos.z+ _Time.w)*_WaveHeight;

				o.uv = v.texcoord;
				UNITY_TRANSFER_FOG(o,o.pos);

				///////////////////
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0.rgb;
				o.ambient = ShadeSH9(half4(worldNormal,1));
				TRANSFER_SHADOW(o)

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D (_MainTex, i.uv)*_Color//tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
                fixed shadow = SHADOW_ATTENUATION(i);
                // darken light's illumination with shadow, keep ambient intact
                fixed3 lighting = i.diff * 1 + i.ambient;
                col.rgb *= lighting;
				col.a = _Color.a;
				return col;
			}
			ENDCG
		}
	}
}
