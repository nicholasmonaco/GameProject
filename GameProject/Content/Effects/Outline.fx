// Outline.fx - Nick Monaco
// A shader effect used to outline the border pixels of a sprite with a specified color.

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
//float4 OutlineColor;
//float2 PixelSize;
int WIDTH;
int HEIGHT;


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

bool DoOutline(VertexShaderOutput input) {
	int x;
	int y;

	//float2 pixelSize = float2(1.0f / WIDTH, 1.0f / HEIGHT);
	float2 pixelSize = float2(1, 1);

	for (x = -1; x <= 1; x++) {
		for (y = -1; y <= 1; y++) {
			// Check if the actual pixel is the currenty selected one
			if (x == 0 && y == 0) continue;

			// If any 8-way neighbor is transparent, then mark as true and break
			float2 off = pixelSize * float2(x, y);
			float4 pixelColor = tex2D(SpriteTextureSampler, input.TextureCoordinates + off) * input.Color;
			if (pixelColor.w == 0) {
				return true;
			}
		}
	}

	return false;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
	bool outlined = DoOutline(input);

	if (outlined == true) {
		return float4(1,0,0,1); //OutlineColor; //disabled for now, just black currently
	} else {
		return tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
	}	
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};