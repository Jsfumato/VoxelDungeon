Shader "Custom/ToonOutlineShader" {
	Properties{
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 1)) = .005
		_MainTex("Base (RGB)", 2D) = "white" { }
	_ToonShade("ToonShader Cubemap(RGB)", CUBE) = "" { }
	}

		CGINCLUDE
#include "UnityCG.cginc"


	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float3 tangent : TANGENT;
		float3 bitangent : BITANGENT;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		UNITY_FOG_COORDS(0)
			fixed4 color : COLOR;
	};

	uniform float _Outline;
	uniform float4 _OutlineColor;

	v2f vert(appdata_full v) {
		v2f o;
		o.pos = v.vertex;

		float3 bitangent = cross(v.normal, v.tangent.xyz) * v.tangent.w;

		//o.pos + v.tangent;
		o.pos.x += (v.normal.x + v.tangent.x + bitangent.x) * _Outline;
		o.pos.y += (v.normal.y + v.tangent.y + bitangent.y) * _Outline;
		o.pos.z += (v.normal.z + v.tangent.z + bitangent.z) * _Outline;

		//o.pos.xyz += v.normal * _Outline;
		/*o.pos.x += normalize(v.vertex.x) * _Outline;
		o.pos.y += normalize(v.vertex.y) * _Outline;
		o.pos.z += normalize(v.vertex.z) * _Outline;*/

		o.pos = mul(UNITY_MATRIX_MVP, o.pos);

		o.color = _OutlineColor;
		UNITY_TRANSFER_FOG(o, o.pos);
		return o;
	}
	ENDCG

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		UsePass "Toon/Basic/BASE"
		Pass{
		Name "OUTLINE"
		Tags{ "LightMode" = "Always" }
		Cull Front
		ZWrite On
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
		fixed4 frag(v2f i) : SV_Target
	{
		UNITY_APPLY_FOG(i.fogCoord, i.color);
	return i.color;
	}
		ENDCG
	}
	}

		Fallback "Toon/Basic"
}
