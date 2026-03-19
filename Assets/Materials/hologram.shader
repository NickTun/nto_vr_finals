Shader "Custom/hologram_unlit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0,1,1,0.3)
        _EmissionColor ("Emission Color", Color) = (0,1,1,1)
        _ScanlineSpeed ("Scanline Speed", Range(0,5)) = 1
        _ScanlineDensity ("Scanline Density", Range(1,50)) = 20
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _BaseColor;
            float4 _EmissionColor;
            float _ScanlineSpeed;
            float _ScanlineDensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Scanlines only (no lighting, no view dependency)
                float scan = abs(sin((i.worldPos.y + _Time.y * _ScanlineSpeed) * _ScanlineDensity));

                float3 col = _BaseColor.rgb + _EmissionColor.rgb * scan * 0.5;
                float alpha = _BaseColor.a;

                return float4(col, alpha);
            }
            ENDCG
        }
    }
}