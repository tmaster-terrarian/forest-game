#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
matrix WorldMatrix;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
    float3 Normal : NORMAL0;
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 WorldPosition : TEXCOORD1;
	float4 Color : COLOR0;
    float3 Normal : TEXCOORD2;
    float2 UV : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	// output.Position = mul(input.Position, WorldViewProjection);
    output.Position = mul(input.Position, WorldViewProjection);
    output.WorldPosition = mul(input.Position, WorldMatrix);
	output.Color = input.Color;
    output.Normal = input.Normal;
    output.UV = input.UV;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return input.Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
