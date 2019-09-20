// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Corruption"
{
    Properties
    {
        _Color ("Default Color", Color) = (1,1,1,1)
        _MainTex ("Default", 2D) = "white" {}
		_NormalMap("Normal", 2D) = "white" {}
		
		//noise
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_Cutoff("Cutoff" , Range(0,1)) = 0
		
		//color
		_ColorTex("Corruption Texture", 2D) = "white" {}
		_CorruptionColor("Corruption Color", Color) = (1,1,1,.1)

		//origin
		//_Origin("Origin Position", Vector) = (0,0,0,0)
		
		//_Offset("Offset" , Range(-30,30)) = 0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _NormalMap;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
			float2 uv_NormalMap;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _CorruptionColor;
		sampler2D _NoiseTex;
		half _Cutoff;
		//float3 _Origin;

		sampler2D _ColorTex;
		//half _Offset;
		//float _Radius;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			half dissolve = tex2D(_NoiseTex, IN.uv_MainTex).r;

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
			o.Smoothness = .3;
            o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));

			fixed4 d = tex2D(_ColorTex, IN.uv_MainTex) * _CorruptionColor;
			o.Emission = d.rgb * step(dissolve - _Cutoff, 0.05f); //emits white color with 0.05 border size

			//radial shader
			//if (distance(_Origin, IN.worldPos) >= _Offset) {
			//	o.Albedo = tex2D(_ColorTexx, IN.uv_MainTex).rgb;
			//}

			//if (distance(_Origin, IN.worldPos) < _Offset) {
			//	o.Albedo = tex2D(_ColorTexx, IN.uv_MainTex).rgb;
			//	if (dissolve <= _Cutoff) {
			//		o.Albedo = tex2D(_ColorTex, IN.uv_MainTex).rgb;
			//		/*fixed4 c = tex2D(_ColorTex, IN.uv_MainTex) * _Color;
			//		o.Albedo = c.rgb;
			//		discard;*/
			//	}
			//}

			//rear to front shader
			//if (localPos.z < _Offset) {
				//o.Albedo = tex2D(_ColorTex, IN.uv_MainTex).rgb;
				if (dissolve <= _Cutoff) {
					fixed4 c = tex2D(_ColorTex, IN.uv_MainTex) * _CorruptionColor;
					o.Albedo = c.rgb + o.Emission;
					//discard;
				}
			//}


        }
        ENDCG
    }
    FallBack "Diffuse"
}
