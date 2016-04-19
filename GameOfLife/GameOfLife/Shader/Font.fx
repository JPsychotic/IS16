const SamplerState PointSampler : register(s1);
const Texture2D FontTexture : register(b0);

struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR0;
};


struct PS_IN
{
	float4 pos : SV_POSITION;
	float2 tex : TEXCOORD0;
	float4 col : COLOR0;
};

PS_IN VS(VS_IN input)
{
	PS_IN output;

	output.pos = float4(input.pos.xy, 0, 1);
	output.tex = input.pos.zw;
	output.col = input.col;

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	//return float4(1,1,1,1);
	//return FontTexture.Sample(PointSampler, input.tex).r * input.col;

	// richtiger:
	//return input.col;
	float mask = FontTexture.Sample(PointSampler, input.tex).a;
	clip(mask - 0.5f);
	float3 color = input.col.rgb * mask;
	return float4(color, 1);
}
