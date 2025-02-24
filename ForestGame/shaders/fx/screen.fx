#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4 ScreenResolution;

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    float4 col = tex2D(SpriteTextureSampler, uv) * input.Color;

    float4x4 ditherMatrix = float4x4(
        0,  8,  2, 10,
        12, 4, 14,  6,
        3, 11,  1,  9,
        15, 7,  13, 5
    );

    float2 pos = uv * ScreenResolution;

    float ditherValue = (ditherMatrix[pos.x % 4][pos.y % 4]) / 16;

    return floor(float4(col.rgb + ditherValue * 0.05, col.a) * 8) / 8;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
