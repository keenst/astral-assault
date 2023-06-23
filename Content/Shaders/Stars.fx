#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler s0;
float2 starPositions[150];

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
	float4 starColor = float4(1, 1, 1, 1);
	float2 pixelSize = 1.0 / float2(480, 270);

	float2 texCoord = input.TextureCoordinates + 0.5 * pixelSize;

	float4 color = tex2D(SpriteTextureSampler, texCoord);

	for (int i = 0; i < starPositions.Length; i++)
	{
		float2 position = starPositions[i];

		if (abs(position.x - texCoord.x) + pixelSize.x / 2 < pixelSize.x && 
			abs(position.y - texCoord.y) + pixelSize.y / 2 < pixelSize.y)
		{
			color = starColor;
		}
	}

	return color;
}

float4 DisabledPS(VertexShaderOutput input) : COLOR
{
	return tex2D(s0, input.TextureCoordinates);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}

	pass P1 
	{
        PixelShader = compile PS_SHADERMODEL DisabledPS();
	}
};