Shader "CubeInstanced"
{
    Properties
    {
        _FarColor("Far color", Color) = (.2, .2, .2, 1)
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

            float4 _FarColor;
            float3 _NoiseDirection;
            float _NoiseSize;
            float _NoiseFreq;
            float _Speed;


            StructuredBuffer<float4> position_buffer_1;
            StructuredBuffer<float4> position_buffer_2;
            StructuredBuffer<float3> scale_buffer;

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

            varyings vert(attributes v, const uint instance_id : SV_InstanceID)
            {
                float4 start = position_buffer_1[instance_id];
                float4 end = position_buffer_2[instance_id];
                float3 random = scale_buffer[instance_id];

                const float t = repeat((_Time.x + start.w) * _Speed * end.w, 1);

                float strength = 1 - pow(1 - abs(2 * t), 2);
                float3 scaledVertex = v.vertex * end.w;
                const float3 world_start = start.xyz + scaledVertex;
                const float3 world_end = end.xyz + scaledVertex;
                float3 pos = lerp(world_start, world_end, t);

                const float3 offset = cross(end - start, float3(-_NoiseDirection.y, -_NoiseDirection.x, 0));
                pos += SimplexNoise(float2(t * _NoiseFreq, t * _Time.x * _NoiseFreq)) * _NoiseSize * _NoiseDirection *
                    strength +
                    offset * strength;

                pos+= random * strength;
                varyings o;
                o.vertex = UnityObjectToClipPos(float4(pos, 1));
                o.diffuse = saturate(dot(v.normal, _WorldSpaceLightPos0.xyz));
                o.color = _FarColor;

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