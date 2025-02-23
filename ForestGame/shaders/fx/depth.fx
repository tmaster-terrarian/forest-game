#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4 HueShift(float3 Color, float Shift)
{
    float3 P = float3(0.55735, 0.55735, 0.55735) * dot(float3(0.55735, 0.55735, 0.55735), Color);
    float3 U = Color - P;
    float3 V = cross(float3(0.55735, 0.55735, 0.55735), U);
    float3 col = (U * cos(Shift * 6.2832)) + (V * sin(Shift * 6.2832)) + P;
    return float4(col.rgb, 1.0);
}

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
matrix InverseViewMatrix;
matrix ProjectionMatrix;
matrix WorldViewProjection;
float3 ViewDir;
float Shininess;
float Metallic;
float MatcapIntensity;
float MatcapPower;
Texture2D MatcapTex;
sampler2D MatcapSampler = sampler_state
{
    Texture = <MatcapTex>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 Normal : NORMAL0;
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 WorldPosition : TEXCOORD1;
    float4 Color : COLOR0;
    float3 Normal : TEXCOORD2;
    float2 UV : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    // output.Position = mul(input.Position, WorldViewProjection);
    output.Position = mul(mul(mul(input.Position, WorldMatrix), ViewMatrix), ProjectionMatrix);
    output.WorldPosition = mul(input.Position, WorldMatrix);
    output.Color = input.Color;
    output.Normal = input.Normal;
    output.UV = input.UV;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 wn = mul(float4(input.Normal, 0), WorldMatrix);

    float3 lightColor = float3(1, 0.9, 0.8) * 0.9;
    float3 ambientColor = float3(0.2, 0.25, 0.35) * 0.8;
    float3 ambient = lerp(ambientColor * 0.4, ambientColor, (wn.y + 1)/2);

    float3 lightDir = float3(1, -1, 1);

    float3 albedo = pow(input.Color.rgb, 2.2);

    float fresnel = pow(saturate(1 - dot(wn.xyz, -ViewDir)), 4) * 0.5;
    float3 directionalLight = lightColor * max(0.1, smoothstep(-0.3, -0.1, dot(wn.xyz, -normalize(lightDir) * 2)));

    float3 light = normalize(lightDir);
    float3 normal = normalize(wn.xyz);
    float3 r = normalize(2 * dot(light, normal) * normal - light);
    float3 v = normalize(mul(normalize(ViewDir), WorldMatrix));

    float2 matcapUv = mul((float3x3)InverseViewMatrix, wn).xy * 0.5 + 0.5;
    float3 matcapColor = tex2D(MatcapSampler, matcapUv).rgb;
    float3 matcapAdjusted = pow(lerp(0, matcapColor, pow(Shininess, 0.5) * MatcapIntensity), MatcapPower);

    float specularDotProduct = saturate(dot(r, v));
    float inverseSpecularDotProduct = saturate(-dot(r, v));
    float3 specularTint = lerp(lightColor, albedo, Metallic * 0.5);
    float oneMinusReflectivity = 1 - Metallic;
    float3 metallicAlbedo = albedo * oneMinusReflectivity;
    float3 specular = max(pow(specularDotProduct, 100 * pow(Shininess, 2)), 0) * specularTint;
    float3 backLight = (max(pow(inverseSpecularDotProduct, 50 * pow(Shininess, 2)), 0) * specularTint) * ambientColor;

    float3 lighting = directionalLight + (ambient * oneMinusReflectivity);

    float3 diffuse = albedo * lighting;
    float3 reflective = (matcapAdjusted) * lerp(0.5, albedo, Metallic);
    float3 rim = step(0.7, 1 - saturate(dot(wn.xyz, -normalize(ViewDir)))) * 0.5 * HueShift(metallicAlbedo.rgb, -0.1);

    float3 finalColor = diffuse + specular + backLight + reflective + rim + fresnel;
    return float4(Posterize(Tonemap(finalColor), 8), 1);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
