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
matrix InverseWorldMatrix;
float3 ViewDir;
float Shininess;
float SpecularIntensity;
float Metallic;

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

float4 HueShift(float3 Color, float Shift)
{
    float3 P = float3(0.55735, 0.55735, 0.55735) * dot(float3(0.55735, 0.55735, 0.55735), Color);

    float3 U = Color - P;

    float3 V = cross(float3(0.55735, 0.55735, 0.55735), U);

    float3 col = (U * cos(Shift * 6.2832)) + (V * sin(Shift * 6.2832)) + P;

    return float4(col.rgb, 1.0);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 wn = mul(float4(input.Normal, 0), WorldMatrix);

    // float4 ambient = float4(0.25, 0.25, 0.35, 0);
    float4 ambient = lerp(float4(0.1, 0.1, 0.12, 0), float4(0.25, 0.25, 0.35, 0), (wn.y + 1)/2);

    float3 lightDir = float3(1, -1, 1);

    float4 diff = float4(pow(input.Color.rgb, 2.2), 1);
    float fresnel = pow(saturate(1 - dot(wn.xyz, -ViewDir)), 4) * 0.5;
    float4 directionalLight = float4(1,1,1,1) * max(0.1, smoothstep(-0.3, -0.1, dot(wn.xyz, -normalize(lightDir)) * 2));

    float3 light = normalize(lightDir);
    float3 normal = normalize(wn.xyz);
    float3 r = normalize(2 * dot(light, normal) * normal - light);
    float3 v = normalize(mul(normalize(ViewDir), WorldMatrix));

    float dotProduct = saturate(dot(r, v));

    float specular = max(pow(dotProduct, 200 * pow(Shininess, 2)), 0) * SpecularIntensity;

    float4 lighting = float4((directionalLight + ambient).rgb, 1);

    float4 rim = step(0.7, 1 - saturate(dot(wn.xyz, -normalize(ViewDir)))) * 0.5 * HueShift(diff.rgb, -0.1);

    return floor((((diff * lighting) + (lighting * 0.3 * (1 - Metallic))) + (ambient * fresnel) + specular + rim) * 8) / 8;
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
