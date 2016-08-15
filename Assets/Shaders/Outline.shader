Shader "Custom/OutlineToonShader" 
{
	Properties
	{
		_Amount("OutLine", Range(0.1, 2)) = 0.1
		_MainTex("Tex (base)", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
	}

		SubShader{
			Tags{ "RenderType" = "Opaque" }
			Cull Front
			Pass{
				CGPROGRAM
				#pragma vertex vert             
				#pragma fragment frag
				#include "UnityCG.cginc"

				float _Amount;
				sampler2D _MainTex;
				float4 _OutlineColor;

			struct vertInput 
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
			};
	
			struct vertOutput 
			{
				float4 pos : SV_POSITION;
				float4 color : SV_Target;
			};
	
			vertOutput vert(in appdata_full input) 
			{
				vertOutput o;

				//o.pos = mul(UNITY_MATRIX_MVP, input.pos);
				float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, input.normal));

				o.pos = input.vertex;
				//float3 norm = normalize(input.normal);

				//o.pos.x += normalize(norm.x) * _Amount;
				//o.pos.y += normalize(norm.y) * _Amount;
				//o.pos.z += normalize(norm.z) * _Amount;

				o.pos.x += normalize(input.vertex.x) * _Amount;
				o.pos.y += normalize(input.vertex.y) * _Amount;
				o.pos.z += normalize(input.vertex.z) * _Amount;
				o.pos = mul(UNITY_MATRIX_MVP, o.pos);

				//o.color = tex2Dlod(_MainTex, input.texcoord);
				o.color = _OutlineColor;

				//o.pos.z += normalize(o.pos.z);

				return o;
			}
	
			half4 frag(vertOutput output) : COLOR{
				return output.color;
			}
			ENDCG
		}
	}
}