Shader "CubeInstanced"
{
    Properties
    {
        _FirstColor("Far color", Color) = (.2, .2, .2, 1)
        _SecondColor("Far color", Color) = (.2, .2, .2, 1)
        _NoiseDirection("Noise Direction", Vector) = (.2, .2, .2)
        _NoiseSize("Noise Size", Float) = 1
        _NoiseFreq("Noise Freq", Float) = 1
        _Speed("Speed", Float) = 1
    }
    SubShader
    {
        Pass
        {
            Tags
            {
                "RenderType"="Opaque"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise3D.hlsl"
            #include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"

            float repeat(const float t, const float length)
            {
                return clamp(t - floor(t / length) * length, 0.0f, length);
            }

            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float4 _FirstColor;
            float4 _SecondColor;
            float3 _NoiseDirection;
            float _NoiseSize;
            float _NoiseFreq;
            float _Speed;

            struct attributes
            {
                float3 normal : NORMAL;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct varyings
            {
                float4 vertex : SV_POSITION;
                float3 diffuse : TEXCOORD2;
                float3 color : TEXCOORD3;
            };

            struct mesh_data
            {
                float3 start;
                float3 end;
                float3 random;
                float2 time;
                bool inverted;
            };

            StructuredBuffer<mesh_data> data;

            varyings vert(attributes v, const uint instance_id : SV_InstanceID)
            {
                float3 start = data[instance_id].start;
                float3 end = data[instance_id].end;
                float3 random = data[instance_id].random;
                float2 time = data[instance_id].time;

                const float t = repeat((_Time.x + time.x) * _Speed * time.y, 1);

                float strength = 1 - pow(1 - abs(2 * t), 2);
                float3 scaledVertex = v.vertex * time.y;
                const float3 world_start = start.xyz + scaledVertex;
                const float3 world_end = end.xyz + scaledVertex;
                float3 pos = lerp(world_start, world_end, t);

                const float3 offset = cross(end - start, float3(-_NoiseDirection.y, -_NoiseDirection.x, 0));
                pos += SimplexNoise(float2(t * _NoiseFreq, t * _Time.x * _NoiseFreq)) * _NoiseSize * _NoiseDirection *
                    strength +
                    offset * strength;

                pos += random * strength;
                varyings o;
                o.vertex = UnityObjectToClipPos(float4(pos, 1));
                o.diffuse = saturate(dot(v.normal, _WorldSpaceLightPos0.xyz));
                o.color = data[instance_id].inverted ? _FirstColor : _SecondColor;

                return o;
            }

            half4 frag(const varyings i) : SV_Target
            {
                const float3 lighting = i.diffuse * 1.7;
                return half4(i.color * lighting, 1);;
            }
            ENDHLSL
        }
    }
}