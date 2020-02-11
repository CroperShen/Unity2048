Shader "Hidden/Round_Rectangle_shader"
{
	Properties
	{
		_R("Radius",Range(0,0.5)) = 0.1
		_Color("_Color",Color) = (1,0,0,1)
	}
		SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _R;
			fixed4 _Color;

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


			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float x = i.uv.x;
				float y = i.uv.y;
				if (x > 0.5){
					x = 1-x;
				}
				if (y > 0.5) {
					y = 1-y;
				}
				x = _R - x;
				y = _R - y;
				if (x>0 && y>0 && x*x+y*y>_R*_R) {
					discard;
				}
				if (i.uv.x < _R && i.uv.y<_R && (i.uv.y - _R) * (i.uv.y - _R) + (i.uv.x - _R) * (i.uv.x - _R)>_R* _R) {
					discard;
				}
				return _Color;
            }
            ENDCG
        }
    }
}
