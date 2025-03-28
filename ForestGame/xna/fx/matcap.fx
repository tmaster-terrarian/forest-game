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
float LightIntensity;
float VertexColorIntensity;
float MatcapIntensity;
float MatcapPower;
int MatcapBlendMode;
int VertexColorBlendMode;
float3 WorldSpaceCameraPos;
float2 ScreenResolution;
Texture2D MatcapTex;
sampler2D MatcapSampler = sampler_state
{
    Texture = <MatcapTex>;
    Filter = Linear;
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

static const float PI = 3.14159265f;

float2 MatcapUv(float3 worldNorm, matrix modelMatrix, matrix viewMatrix, float4 pos)
{
    float3 viewNorm = normalize(mul( float4(worldNorm, 0), viewMatrix));

    // get view space position of vertex
    float3 viewPos = mul(mul( pos, modelMatrix), viewMatrix);
    float3 viewDir = normalize(viewPos);

    // get vector perpendicular to both view direction and view normal
    float3 viewCross = cross(viewDir, viewNorm);

    // swizzle perpendicular vector components to create a new perspective corrected view normal
    viewNorm = float3(-viewCross.y, -viewCross.x, 0.0);

    return viewNorm.xy * 0.5 + 0.5;
}

float2 SphereMapUv(float3 direction) {

    const float PIm2 = 2.0 * PI;
    direction = normalize(direction);
    float2 uv = float2((atan2(direction.z, direction.x) / PIm2) + 0.5, acos(-direction.y) / PI);
    return uv;
}

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    float4 wvp = mul(mul(mul(input.Position, WorldMatrix), ViewMatrix), ProjectionMatrix);
    float3 snap = float3(ScreenResolution.xy / wvp.w, 1/wvp.z);
    float4 pos = float4(trunc(wvp.xyz * snap) / snap, wvp.w);
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
    float3 depthVector = WorldSpaceCameraPos - input.WorldPosition;
    // float depth = saturate((depthVector.x * depthVector.x + depthVector.y * depthVector.y + depthVector.z * depthVector.z) / (25 * 25));
    float depth = saturate(length(depthVector) / 25);

    float3 viewDir = normalize(WorldSpaceCameraPos.xyz - input.WorldPosition.xyz);

    float3 lightColor = float3(1, 0.9, 0.8) * 0.7;
    float3 ambientColor = float3(0.2, 0.25, 0.35) * 0.8;
    float3 ambient = lerp(ambientColor * 0.4, ambientColor, (wn.y + 1)/2);

    float3 matcapColor = tex2D(MatcapSampler, input.MatcapUV).rgb;
    float3 matcapAdjusted = pow(lerp(0, matcapColor, MatcapIntensity), MatcapPower);

    float3 vertColor = lerp(float3(1,1,1), input.Color.rgb, VertexColorIntensity);
    float4 texSample = tex2D(MainTexSampler, input.UV);
    float3 vertColorGamma = pow(vertColor, 2.2);
    float3 albedo = lerp(vertColorGamma + texSample.rgb, vertColorGamma * texSample.rgb, VertexColorBlendMode);

    float3 lightDir = float3(-1, -1, -1);
	float directionalLight = lightColor * max(0.1, smoothstep(-0.3, -0.1, dot(wn.xyz, -normalize(lightDir) * 2)));

    float3 diffuse = albedo * lerp(1, directionalLight + ambient, LightIntensity);

	float3 finalColor = lerp(diffuse + matcapAdjusted, diffuse * matcapAdjusted, MatcapBlendMode);

    return float4(finalColor, 1);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
