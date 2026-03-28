Shader "CarTrickRush/UI/RuleFade"
{
    Properties
    {
        _MainTex ("Mask", 2D) = "white" {}
        _Color ("Tint", Color) = (0,0,0,1)
        _Progress ("Progress", Range(0,1)) = 0
        _Softness ("Softness", Range(0.001,0.25)) = 0.04
        _UseSolid ("Use Solid (no mask)", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Progress;
            float _Softness;
            float _UseSolid;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_UseSolid >= 0.5)
                {
                    fixed4 c = _Color;
                    c.a *= saturate(_Progress);
                    return c;
                }

                half4 sampleTex = tex2D(_MainTex, i.texcoord);
                half m = max(sampleTex.r, sampleTex.a);
                half edge = max(_Softness, 0.0001);
                half cover = smoothstep(m - edge, m + edge, _Progress);
                fixed4 col = _Color;
                col.a *= cover;
                return col;
            }
            ENDCG
        }
    }
}
