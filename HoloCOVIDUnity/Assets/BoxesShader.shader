Shader "Unlit/BoxesShader"
{
	Properties
	{
		_Globification("Globification", Range(0, 1)) = 0
		_GlobeHeight("Globe Height", Range(0, 1)) = 1
		_FlatMapHeight("Flat Map Height", Range(0, 1)) = 1
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

#define Rows 180
#define Columns 360

			struct PopulationPoint
			{
				float xOffset;
				float zOffset;
				float populationValue;
			};

            struct v2f
            {
				float popVal : TEXCOORD0;
				float3 objSpace : TEXCOORD1;
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 flatMapPosition : TEXCOORD2;
            };

			StructuredBuffer<PopulationPoint> _PopulationData;

			float4x4 _FlatMapTransform;
			float4x4 _GlobeTransform;
			float _MaxValue;
			float _Globification; 
			float _GlobeHeight;
			float _FlatMapHeight;

			#define PI 3.141
			
			float3 GetSphereizedPoint(float3 basePos)
			{
				float3 spherePos = basePos.zyx;
				spherePos.x *= PI;
				spherePos.z *= -2 * PI;
				spherePos.y = .5 + basePos.y * _GlobeHeight;
				float sphericalX = spherePos.y * cos(spherePos.x) * cos(spherePos.z);
				float sphericalY = spherePos.y * cos(spherePos.x) * sin(spherePos.z);
				float sphericalZ = spherePos.y * sin(spherePos.x);
				return float3(sphericalY, sphericalZ, sphericalX);
			}

			float3 GetBasePos(float3 cubePos, PopulationPoint datum)
			{
				float retX = (cubePos.x / Columns + datum.xOffset) - .5;
				float retZ = (cubePos.z / Rows + (1 - datum.zOffset)) - .5;
				float retY = (cubePos.y + .5) * datum.populationValue / _MaxValue;
				return float3(retX, retY, retZ);
			}

			v2f vert(appdata_full v, uint inst : SV_InstanceID)
			{
				PopulationPoint datum = _PopulationData[inst];

				float3 basePoint = GetBasePos(v.vertex, datum);
				float3 globePoint = GetSphereizedPoint(basePoint);
				float3 globeWorldPos = mul(_GlobeTransform, float4(globePoint, 1));
				float3 flatPoint = basePoint * float3(1, _FlatMapHeight, 1);
				float3 flatWorldPos = mul(_FlatMapTransform, float4(flatPoint, 1));

				float3 worldPos = lerp(flatWorldPos, globeWorldPos, _Globification);

                v2f o;

                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0f));
				o.popVal = datum.populationValue / _MaxValue;
				o.objSpace = v.vertex + float3(0, .5, 0);
				o.normal = v.normal;
				o.flatMapPosition = basePoint;
                return o;
            } 

            fixed4 frag (v2f i) : SV_Target
            {
				float lutVal = i.popVal * i.objSpace.y;
				float hmm = lutVal * .5 + .5;
				float someVal = pow(lutVal, .1);

				float zorp = pow(lutVal, .3);
				float anotherInterestingValue = lerp(zorp, hmm, saturate(i.normal.y));

				float someOtherVal = pow(lutVal, 2);
				float3 col = float3(someOtherVal, someVal, someVal + .5);
				col = lerp(col.zyx, someVal, saturate(i.normal.y));

				float3 background = lerp(float3(0, 0, 0), float3(1, 0, .5), i.flatMapPosition.z + .5) ;
				//background = pow(saturate(background), 2);
				col = lerp(anotherInterestingValue, background, saturate(-i.normal.y));
				return float4(col, 1);
            }
            ENDCG
        }
    }
}
