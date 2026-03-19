// -------------------------------
//   Star Wars Hologram Shader
//   Built-in Render Pipeline
//   2025–2026 version
// -------------------------------

Shader "Effects/Hologram/StarWarsClassic"
{
    Properties
    {
        _MainTex ("Base Texture (optional)", 2D) = "white" {}
        _Color ("Holo Color", Color) = (0.2, 0.9, 1.0, 1.0)
        _Brightness ("Brightness", Range(0.1, 4.0)) = 1.8
        _FresnelPower ("Fresnel Power", Range(0.5, 8.0)) = 3.5
        _FresnelBoost ("Fresnel Boost", Range(0, 4.0)) = 1.4
        
        _ScanlineDensity ("Scanline Density", Range(1, 120)) = 48
        _ScanlineSpeed ("Scanline Speed", Range(-4, 4)) = -1.6
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.42
        
        _FlickerSpeed ("Flicker Speed", Range(0, 20)) = 8.5
        _FlickerStrength ("Flicker Strength", Range(0, 1)) = 0.28
        _NoiseScale ("Noise Scale", Range(0, 80)) = 22
        _NoiseStrength ("Noise Strength", Range(0, 0.4)) = 0.12
        
        _GlitchSpeed ("Glitch Speed", Range(0, 15)) = 3.2
        _GlitchAmount ("Glitch Amount", Range(0, 0.15)) = 0.028
        _DistortionAmount ("Distortion Strength", Range(0, 0.2)) = 0.04
        
        _Cutoff ("Cutoff", Range(0,1)) = 0.08
        _AlphaBoost ("Alpha Boost", Range(0.5, 3.0)) = 1.45
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
        }
        
        LOD 200
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD1;
                float fresnel : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Brightness;
            float _FresnelPower;
            float _FresnelBoost;
            
            float _ScanlineDensity;
            float _ScanlineSpeed;
            float _ScanlineIntensity;
            
            float _FlickerSpeed;
            float _FlickerStrength;
            float _NoiseScale;
            float _NoiseStrength;
            
            float _GlitchSpeed;
            float _GlitchAmount;
            float _DistortionAmount;
            
            float _Cutoff;
            float _AlphaBoost;

            // Simple 3D noise
            float hash12(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * float3(0.1031, 0.1030, 0.0973));
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f*f*(3.0-2.0*f);
                return lerp(lerp(hash12(i + float2(0,0)),
                                 hash12(i + float2(1,0)), f.x),
                            lerp(hash12(i + float2(0,1)),
                                 hash12(i + float2(1,1)), f.x), f.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                
                // Very cheap old-school glitch/distortion
                float glitchTime = _Time.y * _GlitchSpeed;
                float glitch = step(0.97, sin(glitchTime * 12.0 + v.vertex.z * 3.0)) * 
                              (sin(glitchTime * 77.0) * 0.5 + 0.5);
                
                float2 noiseUV = worldPos.xy * 0.3 + _Time.y * float2(0.7, 1.3);
                float n = noise(noiseUV * 3.0) * 2.0 - 1.0;
                float distortion = n * _DistortionAmount * (1.0 + glitch * 3.0);
                
                float4 distortedPos = v.vertex;
                distortedPos.xz += distortion * float2(1, 0.4);
                
                o.vertex = UnityObjectToClipPos(distortedPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = viewDir;
                
                // Quick fresnel approximation (done in vertex for perf)
                float VdotN = dot(viewDir, o.worldNormal);
                o.fresnel = pow(1.0 - saturate(VdotN), _FresnelPower) * _FresnelBoost;
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // -------------------------------
                // 1. Base texture / color
                // -------------------------------
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed3 col = tex.rgb * _Color.rgb * _Brightness;
                
                // -------------------------------
                // 2. Scanlines
                // -------------------------------
                float scan = sin(i.uv.y * _ScanlineDensity * 6.2831 + _Time.y * _ScanlineSpeed * 30.0);
                scan = saturate(scan * 0.5 + 0.5);
                col *= lerp(1.0, 0.4 + scan * 0.6, _ScanlineIntensity);
                
                // -------------------------------
                // 3. Noise / film grain
                // -------------------------------
                float n = noise(i.uv * _NoiseScale + _Time.y * float2(13.7, 7.1));
                col += (n * 2.0 - 1.0) * _NoiseStrength;
                
                // -------------------------------
                // 4. Flicker (the heartbeat of SW holo)
                // -------------------------------
                float flicker = 0.8 + 0.2 * sin(_Time.y * _FlickerSpeed * 20.0);
                flicker += 0.15 * sin(_Time.y * _FlickerSpeed * 47.0 + 2.1);
                flicker += 0.08 * sin(_Time.y * _FlickerSpeed * 91.0 + 7.7);
                flicker = saturate(flicker);
                col *= lerp(1.0, flicker, _FlickerStrength);
                
                // -------------------------------
                // 5. Fresnel glow + edge boost
                // -------------------------------
                col += _Color.rgb * i.fresnel * 1.4;
                
                // -------------------------------
                // 6. Final alpha composition
                // -------------------------------
                float alpha = tex.a * _Color.a;
                alpha *= i.fresnel * _AlphaBoost + 0.35;     // always some visibility
                alpha = saturate(alpha);
                alpha = smoothstep(_Cutoff, _Cutoff + 0.12, alpha);
                
                // Very slight random blinking (like bad connection)
                float badConnection = step(0.995, sin(_Time.y * 11.0 + i.uv.x * 30.0));
                alpha *= lerp(1.0, 0.1, badConnection * 0.6);
                
                fixed4 finalColor = fixed4(col, alpha);
                
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}