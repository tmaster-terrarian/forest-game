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
matrix InverseWorldMatrix;
matrix ViewMatrix;
matrix InverseViewMatrix;
matrix ProjectionMatrix;
matrix WorldViewProjection;
float3 ViewDir;
float Shininess;
float Metallic;
float MatcapIntensity;
float MatcapPower;
float3 WorldSpaceCameraPos;
float2 ScreenResolution;
Texture2D MatcapTex;
sampler2D MatcapSampler = sampler_state
{
    Texture = <MatcapTex>;
};
Texture2D MainTex;
sampler2D MainTexSampler = sampler_state
{
    Texture = <MainTex>;
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
    float2 MatcapUV : TEXCOORD3;
};

float2 MatcapUv(float3 worldNorm, matrix modelMatrix, matrix viewMatrix, float4 pos)
{
    float3 viewNorm = normalize(mul( float4(worldNorm, 0), viewMatrix));

    // get view space position of vertex
    float3 viewPos = mul(mul( pos, modelMatrix), viewMatrix);
    float3 viewDir = normalize(viewPos);

    // get vector perpendicular to both view direction and view normal
    float3 viewCross = cross(viewDir, viewNorm);

    // swizzle perpendicular vector components to create a new perspective corrected view normal
    viewNorm = float3(-viewCross.y, viewCross.x, 0.0);

    return viewNorm.xy * 0.5 + 0.5;
}

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 wvp = mul(mul(mul(input.Position, WorldMatrix), ViewMatrix), ProjectionMatrix);
    float3 snap = float3(ScreenResolution.xy / wvp.w, 1/wvp.z);
    float4 pos = float4(floor(wvp.xyz * snap) / snap, wvp.w);
    output.Position = pos;
    output.WorldPosition = mul(input.Position, WorldMatrix);
    output.Color = input.Color;
    output.Normal = normalize(input.Normal);
    output.UV = input.UV;

    float3 wn = normalize(mul( normalize(input.Normal), WorldMatrix ));
    output.MatcapUV = MatcapUv(wn, WorldMatrix, ViewMatrix, input.Position);

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 wn = normalize(mul( normalize(input.Normal), WorldMatrix ));
    float3 vn = normalize(mul( float4(wn, 0), ViewMatrix));

    float3 worldNorm = normalize(mul( normalize(input.Normal), WorldMatrix ));
    float3 viewNorm = normalize(mul( float4(worldNorm, 0), ViewMatrix));

    // return float4(viewNorm, 1);


    float3 viewDir = normalize(WorldSpaceCameraPos.xyz - input.WorldPosition.xyz);

    float3 lightColor = float3(1, 0.9, 0.8) * 0.7;
    float3 ambientColor = float3(0.2, 0.25, 0.35) * 0.8;
    float3 ambient = lerp(ambientColor * 0.4, ambientColor, (wn.y + 1)/2);

    float3 lightDir = float3(-1, -1, -1);

    float3 albedo = pow(input.Color.rgb, 2.2) * tex2D(MainTexSampler, input.UV).rgb;

    float fresnel = pow(saturate(dot(wn.xyz, viewDir)), 4) * 0.5;
    float3 directionalLight = lightColor * max(0.1, smoothstep(-0.3, -0.1, dot(wn.xyz, -normalize(lightDir) * 2)));

    float3 matcapColor = tex2D(MatcapSampler, input.MatcapUV).rgb;
    float3 matcapAdjusted = pow(lerp(0, matcapColor, pow(Shininess, 0.5) * MatcapIntensity), MatcapPower);

    float3 halfVector = normalize(-lightDir + viewDir);
    float specularDotProduct = saturate(dot(halfVector, wn.xyz));
    float inverseSpecularDotProduct = saturate(-dot(halfVector, wn.xyz));
    float3 specularTint = lerp(lightColor, albedo, Metallic * 0.5);
    float oneMinusReflectivity = 1 - Metallic;
    float3 metallicAlbedo = albedo * oneMinusReflectivity;
    float3 specular = max(pow(specularDotProduct, 100 * Shininess), 0) * specularTint;
    float3 backLight = (max(pow(inverseSpecularDotProduct, 50 * Shininess), 0) * specularTint) * ambientColor;

    float3 lighting = directionalLight + (ambient * oneMinusReflectivity);

    float3 diffuse = albedo * lighting;
    float3 reflective = (matcapAdjusted) * lerp(0.5, albedo, Metallic);
    float3 rim = step(0.7, 1 - saturate(dot(wn.xyz, -normalize(viewDir)))) * 0.5 * HueShift(metallicAlbedo.rgb, -0.1);

    float3 finalColor = diffuse + specular + backLight + reflective + rim;
    return float4(Posterize(Tonemap(finalColor), 100), 1);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
