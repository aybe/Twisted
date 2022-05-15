Shader "Twisted/DMDViewer"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		[HideInInspector]
		_Cutoff("Alpha cutoff", Range(0, 1)) = 0.5

		[Enum(UnityEngine.Rendering.CullMode)]
		_Cull ("Cull", Int) = 0
	}
	SubShader
	{
		Tags
		{
			"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"
		}
		LOD 100
		Cull[_Cull]
		Pass
		{
			CGPROGRAM

			// #pragma enable_d3d11_debug_symbols

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ DMD_VIEWER_TEXTURE
			#pragma multi_compile _ DMD_VIEWER_COLOR_VERTEX
			#pragma multi_compile _ DMD_VIEWER_COLOR_POLYGON
			#pragma multi_compile _ DMD_VIEWER_COLOR_ALPHA

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
#if DMD_VIEWER_TEXTURE
				float2 uv0 : TEXCOORD0;
#endif
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
#if DMD_VIEWER_TEXTURE
				float2 uv0 : TEXCOORD0;
#endif
#if DMD_VIEWER_COLOR_VERTEX
				float4 uv1 : TEXCOORD1;
#endif
#if DMD_VIEWER_COLOR_POLYGON
				float4 uv2 : TEXCOORD2;
#endif
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
#if DMD_VIEWER_TEXTURE
				o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
#endif
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
				fixed4 col = fixed4(1, 1, 1, 1);

#if DMD_VIEWER_TEXTURE
				col = tex2D(_MainTex, i.uv0);
#endif

#if DMD_VIEWER_COLOR_VERTEX
				col *= i.uv1;
#endif

#if DMD_VIEWER_COLOR_POLYGON
				col *= i.uv2;
#endif

#if DMD_VIEWER_COLOR_ALPHA
				clip(col.a - _Cutoff);
#endif

				return col;
			}
			ENDCG
		}
	}
}