Shader "Unlit/BoxesShader"
{
	Properties
	{
		_Globification("Globification", Range(0, 1)) = 0
		_GlobeHeight("Globe Height", Range(-1, 1)) = 1
		_GlobeRadius("Globe Radius", Float) = 1
		_TesterX("TesterX", float) = 1
		_TesterY("TesterY", float) = 1
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
            };

			StructuredBuffer<PopulationPoint> _PopulationData;

			float4x4 _MasterTransform;
			float _MaxValue;
			float _Globification; 
			float _GlobeRadius;
			float _GlobeHeight;
			float _TesterX;
			float _TesterY;
			float _AnotherTester;
			
			float3 GetSphereizedPoint(float3 basePos)
			{
				float3 spherePos = basePos.zyx;
				spherePos.x *= _TesterX;
				spherePos.z *= _TesterY;
				spherePos.y = _GlobeRadius + basePos.y * _GlobeHeight;
				float sphericalX = spherePos.y * cos(spherePos.x) * cos(spherePos.z);
				float sphericalY = spherePos.y * cos(spherePos.x) * sin(spherePos.z);
				float sphericalZ = spherePos.y * sin(spherePos.x);
				float3 sphericalPos = float3(sphericalY, sphericalZ, sphericalX);
				return lerp(basePos, sphericalPos, _Globification);
			}

			float3 GetObjPos(float3 cubePos, PopulationPoint datum)
			{
				float retX = (cubePos.x / 360 + datum.xOffset) - .5;
				float retZ = (cubePos.z / 180 + (1 - datum.zOffset)) - .5;
				float retY = (cubePos.y + .5) * datum.populationValue / _MaxValue;
				return float3(retX, retY, retZ);
			}

			v2f vert(appdata_full v, uint inst : SV_InstanceID)
			{
				PopulationPoint datum = _PopulationData[inst];

				float3 objPoint = GetObjPos(v.vertex, datum);
				float3 sphereizedPoint = GetSphereizedPoint(objPoint);
				float3 worldPos = mul(_MasterTransform, float4(sphereizedPoint, 1));

                v2f o;

                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0f));
				o.popVal = datum.populationValue / _MaxValue;
				o.objSpace = v.vertex;
                return o;
            } 

            fixed4 frag (v2f i) : SV_Target
            {
				float yVal = i.objSpace.y + .5;
				return pow(i.popVal * yVal, .2);
            }
            ENDCG
        }
    }
}
