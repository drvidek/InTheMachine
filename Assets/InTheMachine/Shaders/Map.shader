Shader "Unlit/Map"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackgroundColorIn("Background Color In", Color) = (1,1,1,1)
        _FogColorIn("Fog Color In", Color) = (1,0,1,1)

        _BackgroundColor("Background Color", Color) = (0,0,0,1)
        _WallColor("Wall Color", Color) = (0,1,0,1)
        _FogColor("Fog Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            //Stencil
            // {
            //     Ref 1
            //     Comp Equal
            //
            //     Pass Keep
            // }  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BackgroundColor, _WallColor, _FogColor, _BackgroundColorIn, _FogColorIn;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                half3 deltaBG = abs(col.rgb - _BackgroundColorIn.rgb);
                half3 deltaFog = abs(col.rgb - _FogColorIn.rgb);
                return (deltaBG.r + deltaBG.g + deltaBG.b) < 0.001 ? _BackgroundColor :
                        (deltaFog.r + deltaFog.g + deltaFog.b) < 0.001 ? _FogColor :
                        _WallColor;
            }
            ENDCG
        }
    }
}