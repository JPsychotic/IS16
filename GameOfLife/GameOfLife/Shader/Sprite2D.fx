SamplerState LinearSampler : register(s0);

const Texture2D tex : register(t0);

cbuffer drawInput : register(b2)
{
	const float4 RectLocSize; // locx locy sizex sizey
	const float4 RectTex; // x > 0
	const float4 RectColor; // rgba
}

struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR0;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 tex : COLOR1;
};


PS_IN VS(VS_IN input)
{
	PS_IN output;
	output.pos = float4(RectLocSize.xy + float2(RectLocSize.z * input.pos.x, RectLocSize.w * input.pos.y)*2, 0, 1); // 
	output.tex = float4(input.pos.xy, float2(output.pos.xy) / 2 + float2(0.5, -0.5));
	return output;
}

float4 PS(PS_IN input) : SV_Target
{
    return RectColor;
	if (RectTex.x < 1) return RectColor;
	float4 col = tex.Sample(LinearSampler, input.tex.xy);
	return col;
}