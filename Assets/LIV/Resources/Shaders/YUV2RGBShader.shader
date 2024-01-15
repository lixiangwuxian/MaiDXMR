Shader "New Chromantics/YuvModified"
{
    Properties
    {
        _YTex ("Y Texture", 2D) = "white" {}
        _UVTex ("UV Texture", 2D) = "white" {}
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _YTex;
            sampler2D _UVTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv.y = 1 - o.uv.y;
                return o;
            }

            float4 YUVtoRGB(float Y, float U, float V)
            {
                float R = Y + 1.402 * V;
                float G = Y - 0.344 * U - 0.714 * V;
                float B = Y + 1.772 * U;
                return float4(R, G, B, 1.0);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float Y = tex2D(_YTex, i.uv).r;
                float2 UV = tex2D(_UVTex, i.uv).rg - 0.5;
                float4 rgb = YUVtoRGB(Y, UV.r, UV.g);
                return rgb;
            }
            ENDCG
        }
    }
}
