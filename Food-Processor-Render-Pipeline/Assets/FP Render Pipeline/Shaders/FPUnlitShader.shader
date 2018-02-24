
//Update your path name for selecting the shader here
Shader "Food Processor Shaders/Unlit Opaque Color"
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

	struct Attributes
	{
		float4 position : POSITION;
	};

	struct Varyings
	{
		float4 position : SV_POSITION;
	};

	Varyings Vertex(Attributes input)
	{
		Varyings output;
		output.position = UnityObjectToClipPos(input.position);
		return output;
	}

	half3 Fragment(Varyings input) : SV_Target
	{
		return _Color;
	}

		ENDHLSL
	}
	}
}