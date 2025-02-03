Shader "Universe/BlitCopy"
{
    Properties
    {
        _MainTex ("Texture", 2DArray) = "white" {}
        _ArrayIndex ("Array Index", Int) = 0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 uv : TEXCOORD0;
            };

            UNITY_DECLARE_TEX2DARRAY(_MainTex);
            int _ArrayIndex;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = float3(v.texcoord.xy, _ArrayIndex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return UNITY_SAMPLE_TEX2DARRAY(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
