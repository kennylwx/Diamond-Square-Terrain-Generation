// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "xin/TerrainShader" {
	Properties
	{
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
		_MountainColor("Mountain Color", Vector) = (0.0, 0.0, 0.0)
		_SandColor("Sand Color", Vector) = (0.0, 0.0, 0.0)
		_SnowColor("Snow Color", Vector) = (0.0, 0.0, 0.0)

		_SnowHeight ("Snow height", Float) = 160
		_TerrainUpper ("Upper terrain height", Float) = 120
		_TerrainLower ("Lower terrain height", Float) = 70
		_SandHeight ("Sand height", Float) = 50

	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float3 _PointLightColor;
			uniform float3 _PointLightPosition;
			float4 _MountainColor;
			float4 _SnowColor;
			float4 _SandColor;
			float _SandHeight;
			float _SnowHeight;
			float _TerrainUpper;
			float _TerrainLower;

			struct vertIn
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 worldVertex : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
			};

			// Implementation of the vertex shader
			vertOut vert(vertIn v)
			{
				vertOut o;

				// Convert Vertex position and corresponding normal into world coords.
				// Note that we have to multiply the normal by the transposed inverse of the world 
				// transformation matrix (for cases where we have non-uniform scaling; we also don't
				// care about the "fourth" dimension, because translations don't affect the normal) 
				float4 worldVertex = mul(unity_ObjectToWorld, v.vertex);
				float3 worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), v.normal.xyz));

				// Transform vertex in world coordinates to camera coordinates, and pass colour
				o.vertex = UnityObjectToClipPos(v.vertex);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				if (worldPos.y >= _SnowHeight)
					o.color = _SnowColor;
				
				//snow to mountain transition
				
				if (worldPos.y <= _SnowHeight && worldPos.y>=_TerrainUpper)
					o.color = lerp(_MountainColor, _SnowColor, (worldPos.y - _TerrainUpper )/(_SnowHeight - _TerrainUpper));
				if (worldPos.y <= _TerrainUpper && worldPos.y >=_TerrainLower)
					o.color = _MountainColor;
				if (worldPos.y <= _TerrainLower && worldPos.y >=_SandHeight)
					o.color = lerp(_SandColor, _MountainColor, (worldPos.y - _SandHeight)/(_TerrainLower - _SandHeight));
				if (worldPos.y <= _SandHeight)
					o.color = _SandColor;
					

				// Pass out the world vertex position and world normal to be interpolated
				// in the fragment shader (and utilised)
				o.worldVertex = worldVertex;
				o.worldNormal = worldNormal;

				return o;
			}

			// Implementation of the fragment shader
			fixed4 frag(vertOut v) : SV_Target
			{
				// Our interpolated normal might not be of length 1
				float3 interpNormal = normalize(v.worldNormal);

				// Calculate ambient RGB intensities
				float Ka = 0.3;
				float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;
				
				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = 1;
				float Kd = 1;
				float3 L = normalize(_PointLightPosition - v.worldVertex.xyz);
				float LdotN = dot(L, interpNormal);
				float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN);

				// Calculate specular reflections
				float Ks = 0.1;
				float specN = 1; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos - v.worldVertex.xyz);
				
				// Using Blinn-Phong approximation:
				specN = 25; // We usually need a higher specular power when using Blinn-Phong
				float3 H = normalize(V + L);
				float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(interpNormal, H)), specN);

				// Combine Phong illumination model components
				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
				returnColor.a = v.color.a;

				return returnColor;
			}
			ENDCG
		}
	}
}