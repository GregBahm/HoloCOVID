Shader "Unlit/HandPanelShellShader"
{
    Properties
    {
		_HighColor("High Color", Color) = (1,1,1,1)
		_LowColor("Low Color", Color) = (1,1,1,1)
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float3 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 viewDir : TEXCOORD1;
				float3 color : COLOR0;
            };

			float4 _HighColor;
			float4 _LowColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.normal = v.normal;
				o.viewDir = UnityObjectToViewPos(v.vertex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				i.viewDir = normalize(i.viewDir);
				float theDot = dot(i.viewDir, i.normal);
				float shade = (theDot + 1) * .25;
				shade = pow(shade, .5) * .5;
				return shade;
            }
            ENDCG
        }
    }
}
