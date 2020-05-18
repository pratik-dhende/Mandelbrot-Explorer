Shader "Explorer/Mandelbrot Set"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_MaxIterations("MaxIterations", int) = 255
		_Area("Area", vector) = (0, 0, 4, 4)
		_Angle("Angle", range(-3.1415, 3.1415)) = 0.0
		_Gradientcolor("Gradient Color", Color) = (0.3, 0.45, 0.65, 1)
		_GradientRange("Gradient Range", float) = 20.0
		[Toggle(USE_TEXTURE)] _OriginalMandelbrot("Original Mandelbrot", int) = 0
		[Toggle(USE_TEXTURE)] _FixMandelbrotColor("Fix Mandelbrot", int) = 0
		_MandelbrotColor("Mandelbrot Color", Color) = (0.0, 0.0, 0.0, 1)
		_OuterColor("Outer Color", Color) = (1.0, 1.0, 1.0, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			int _MaxIterations;
			float4 _Area;
			float _Angle;
			float4 _Gradientcolor;
			float _GradientRange;
			int _FixMandelbrotColor;
			float4 _MandelbrotColor;
			int _OriginalMandelbrot;
			float4 _OuterColor;

			float2 rotate(float2 uv, float2 pivot, float angle) {
				uv -= pivot;
				return float2((uv.x * cos(angle) - uv.y * sin(angle)), (uv.x * sin(angle) + uv.y * cos(angle))) + pivot;
			}

			float4 colorFunction(float col) {
				float4 colorGradient = sin(_Gradientcolor * col * _GradientRange) * 0.5f + 0.5f;

				return colorGradient;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float2 c = _Area.xy + (i.uv - 0.5) * _Area.zw;
				c = rotate(c, _Area.xy, _Angle);
				//c = rotate(c, float2(0.0f, 0.0f), _Angle);
				float2 z;

				int iterations;

				while (length(z) < 4 && iterations < _MaxIterations) {
					z = float2((z.x * z.x) - (z.y * z.y), 2 * z.x * z.y) + c;
					iterations += 1;
				}

				float4 col = sqrt(float(iterations) / float(_MaxIterations));

				if (iterations >= _MaxIterations && _FixMandelbrotColor)
						return _MandelbrotColor;

				if (_OriginalMandelbrot)
					return _OuterColor * col;

				return colorFunction(col);
            }
            ENDCG
        }
    }
}
