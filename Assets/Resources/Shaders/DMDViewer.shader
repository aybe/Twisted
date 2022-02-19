Shader "Twisted/DMDViewer"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		[Enum(UnityEngine.Rendering.CullMode)]
		_Cull ("Cull", Int) = 0
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}
		LOD 100
		Cull[_Cull]
		Pass
		{
			CGPROGRAM

			// #pragma enable_d3d11_debug_symbols

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ DMD_VIEWER_COLOR_VERTEX
			#pragma multi_compile _ DMD_VIEWER_COLOR_POLYGON

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv0 : TEXCOORD0;
#if DMD_VIEWER_COLOR_VERTEX
				float4 uv1 : TEXCOORD1;
#endif
#if DMD_VIEWER_COLOR_POLYGON
				float4 uv2 : TEXCOORD2;
#endif
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv0 : TEXCOORD0;
#if DMD_VIEWER_COLOR_VERTEX
				float4 uv1 : TEXCOORD1;
#endif
#if DMD_VIEWER_COLOR_POLYGON
				float4 uv2 : TEXCOORD2;
#endif
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
#if DMD_VIEWER_COLOR_VERTEX
				o.uv1 = v.uv1;
#endif
#if DMD_VIEWER_COLOR_POLYGON
				o.uv2 = v.uv2;
#endif
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv0);

#if DMD_VIEWER_COLOR_VERTEX
				col *= i.uv1;
#endif

#if DMD_VIEWER_COLOR_POLYGON
				col *= i.uv2;
#endif

				return col;
			}
			ENDCG
		}
	}
}