// Grayscale.fx - Nick Monaco
// A grayscale shader effect. Makes sprites grayscale.

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
	// Get color of the sprite
	float4 color = tex2D(s0, input.TextureCoordinates);
	float avg = (color.r + color.g + color.b) / 3.0f;
	float4 finalColor = float4(avg, avg, avg, color.a);

	// Average the tint color applied to the sprite
	float tintAvg = (input.Color.r + input.Color.g + input.Color.b) / 3.0f;
	float4 tint = float4(tintAvg, tintAvg, tintAvg, input.Color.a);

	// Multiply them together for final color
	return finalColor * tint;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};