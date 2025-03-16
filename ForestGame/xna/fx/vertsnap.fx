#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float3 Posterize(float3 c, float levels)
{
    return floor(c * levels) / levels;
}

float3 Tonemap(float3 c)
{
    float lengthOver = max(length(c) - 1, 0) * 0.3;
    return c + lengthOver;
}

matrix WorldMatrix;
matrix ViewMatrix;
matrix ProjectionMatrix;
float2 ScreenResolution;
Texture2D MainTex;
sampler2D MainTexSampler = sampler_state
{
    Texture = <MainTex>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 WorldPosition : TEXCOORD1;
    float4 Color : COLOR0;
    float2 UV : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 wvp = mul(mul(mul(input.Position, WorldMatrix), ViewMatrix), ProjectionMatrix);
    float3 snap = float3(ScreenResolution.xy / wvp.w, 1/wvp.z);
    output.Position = float4(trunc(wvp.xyz * snap) / snap, wvp.w);
    output.WorldPosition = mul(input.Position, WorldMatrix);
    output.Color = input.Color;
    output.UV = input.UV;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 texSample = tex2D(MainTexSampler, input.UV);
    clip(texSample.a - 0.3f);
    return float4(pow(input.Color.rgb, 2.2) * texSample.rgb, 1);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
