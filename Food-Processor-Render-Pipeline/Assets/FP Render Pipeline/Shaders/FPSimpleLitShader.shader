
//Update your path name for selecting the shader here
Shader "Food Processor Shaders/Simple Lit Opaque Color"
{
	Properties
	{
		_Color("Tint", Color) = (0.5, 0.5, 0.5)
	}



		SubShader
	{
		//Update to use your render pipeline. It should be named after the runtime class (LightweightPipeline), not the asset (LightweightPipelineAsset)!
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "FPRenderPipeline" }

		Pass
	{
		//Lightmode is the shader pass name
		Tags{ "LightMode" = "Base" }
		HLSLPROGRAM
#pragma multi_compile_fog
#pragma vertex Vertex
#pragma fragment Fragment

# include "UnityCG.cginc"

	half3 _Color;
	half4 _ambientColor;
	half4 _LightColor;
	half3 _LightDirection;

	struct Attributes
	{
		float4 position : POSITION;
		float3 normal : NORMAL;
	};

	struct Varyings
	{
		float4 position : SV_POSITION;
		float3 normal : TEXCOORD0;
	};

	Varyings Vertex(Attributes input)
	{
		Varyings output;
		output.position = UnityObjectToClipPos(input.position);
		output.normal = input.normal;
		return output;
	}

	half3 Fragment(Varyings input) : SV_Target
	{
		if (any(saturate(_LightColor)))
			return _Color * (0.4 * smoothstep(0.4, 0.6, dot(input.normal, _LightDirection)) + 0.6) * _LightColor + _ambientColor;
		return _Color + _ambientColor;
	}

		ENDHLSL
	}
	}
}