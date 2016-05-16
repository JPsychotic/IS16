SamplerState LinearSampler : register(s0);
SamplerState PointSampler : register(s1);

const Texture2D golTex : register(t0);
const Texture2D randomTex : register(t1);

cbuffer frameInput : register(b2)
{
	const float4 FrameInfo; // abstand pixel X, abstand pixel Y, random, random
	const float4 Rules; //  regeln geburt, regeln tod, nix, nix
}

struct PS_IN
{
	float4 pos : SV_POSITION;
	float2 tex : TEXCOORD0;
};

struct VS_IN
{
	float4 pos : POSITION;
	float4 nor : NORMAL;
	//float2 tex : TEXCOORD;	
};

PS_IN VS(float4 position : POSITION)
{
	PS_IN output;
	output.pos = float4(position.xy, 0, 1);
	output.tex = position.ba;
	return output;
}


float4 PS(PS_IN input) : SV_Target
{
	// ABC
	// EMD
	// FGH

	float4 pixelab = golTex.Sample(LinearSampler, input.tex + float2(-FrameInfo.x / 2, -FrameInfo.y));
	float4 pixelcd = golTex.Sample(LinearSampler, input.tex + float2(FrameInfo.x, -FrameInfo.y / 2));
	float4 pixelef = golTex.Sample(LinearSampler, input.tex + float2(-FrameInfo.x, FrameInfo.y / 2));
	float4 pixelgh = golTex.Sample(LinearSampler, input.tex + float2(FrameInfo.x / 2, FrameInfo.y));

	float4 pixelCenter = golTex.Sample(PointSampler, input.tex);
	float4 random = randomTex.Sample(PointSampler, input.tex + FrameInfo.zw);
	float4 sum = pixelab + pixelcd + pixelef + pixelgh;
	
	sum = mad(sum, 2, random / 4);

	const float4 dead = float4(0, 0, 0, 1);
	float4 alive = float4(floor(saturate(sum.rgb - max(sum.r, max(sum.g, sum.b)) + 1)), 1);

	uint index = floor(sum.r + sum.g + sum.b);
	if ((asuint(Rules.x) >> index) & 1) return alive;
	if ((asuint(Rules.y) >> index) & 1) return dead;
	//if(pixelCenter.r == 0 && pixelCenter.g == 0 && pixelCenter.b == 0)
	return pixelCenter;
	//return alive;
}