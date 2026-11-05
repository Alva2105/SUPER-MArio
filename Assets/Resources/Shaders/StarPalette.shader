// Sprite shader that swaps the three player colors, like the NES changes the
// sprite palette while Mario or Luigi have the star. Each color role accepts
// two source colors because the sprites mix the FCEUX and Nestopia palettes.
Shader "Mario/StarPalette"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            float _Swap;
            float4 _FromA1, _FromA2, _FromB1, _FromB2, _FromC1, _FromC2;
            float4 _ToA, _ToB, _ToC;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            bool Near(float3 a, float3 b)
            {
                return distance(a, b) < 0.02;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);

                if (_Swap > 0.5)
                {
                    if (Near(c.rgb, _FromA1.rgb) || Near(c.rgb, _FromA2.rgb)) c.rgb = _ToA.rgb;
                    else if (Near(c.rgb, _FromB1.rgb) || Near(c.rgb, _FromB2.rgb)) c.rgb = _ToB.rgb;
                    else if (Near(c.rgb, _FromC1.rgb) || Near(c.rgb, _FromC2.rgb)) c.rgb = _ToC.rgb;
                }

                c *= i.color;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
