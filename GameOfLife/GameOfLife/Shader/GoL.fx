SamplerState LinearSampler : register(s0);
SamplerState PointSampler : register(s1);

const Texture2D golTex : register(t0);

cbuffer frameInput : register(b2)
{
	const float4 FrameInfo; // abstand pixel X, abstand pixel Y, nix, nix
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
	const float4 dead = float4(0, 0, 0, 1);
	// ABC
	// EMD
	// FGH

	float4 pixelab = golTex.Sample(LinearSampler, input.tex + float2(-FrameInfo.x / 2, -FrameInfo.y));
	float4 pixelcd = golTex.Sample(LinearSampler, input.tex + float2(FrameInfo.x, -FrameInfo.y / 2));
	float4 pixelef = golTex.Sample(LinearSampler, input.tex + float2(-FrameInfo.x, FrameInfo.y / 2));
	float4 pixelgh = golTex.Sample(LinearSampler, input.tex + float2(FrameInfo.x / 2, FrameInfo.y));

	float4 pixelCenter = golTex.Sample(PointSampler, input.tex);
	float4 alive = pixelCenter;
	float4 sum = pixelab + pixelcd + pixelef + pixelgh;

	sum *= 2;

	if(sum.r >= sum.g && sum.r >= sum.b)
		alive = float4(1, 0, 0, 1);
	else if(sum.g >= sum.r && sum.g >= sum.b)
		alive = float4(0, 1, 0, 1);
	else if(sum.b >= sum.r && sum.b >= sum.g)
		alive = float4(0, 0, 1, 1);

	uint index = round(sum.r + sum.g + sum.b);
	if ((asuint(Rules.x) >> index) & 1 > 0) return alive;
	if ((asuint(Rules.y) >> index) & 1 > 0) return dead;
	if(pixelCenter.r == 0 && pixelCenter.g == 0 && pixelCenter.b == 0)
		return pixelCenter;
	return alive;
}

//float4 pixelOL = golTex.Sample(PointSampler, input.tex - FrameInfo.xy);
//float4 pixelOM = golTex.Sample(PointSampler, input.tex + float2(0, -FrameInfo.y));
//float4 pixelOR = golTex.Sample(PointSampler, input.tex + float2(FrameInfo.x, -FrameInfo.y));
//
//float4 pixelML = golTex.Sample(PointSampler, input.tex + float2(-FrameInfo.x, 0));
//float4 pixelCenter = golTex.Sample(PointSampler, input.tex);
//float4 pixelMR = golTex.Sample(PointSampler, input.tex + float2(FrameInfo.x, 0));
//
//float4 pixelUL = golTex.Sample(PointSampler, input.tex + float2(-FrameInfo.x, FrameInfo.y));
//float4 pixelUM = golTex.Sample(PointSampler, input.tex + float2(0, FrameInfo.y));
//float4 pixelUR = golTex.Sample(PointSampler, input.tex + FrameInfo.xy);
//
//float sum = pixelOL.x + pixelOM.x + pixelOR.x + pixelML.x + pixelMR.x + pixelUL.x + pixelUM.x + pixelUR.x;
//if (pixelCenter.x < 0.5f)
//{
//	//cell is dead
//	if (sum < 3.5f && sum > 2.5f)
//	{
//		return alive;
//	}
//}
//else
//{
//	//cell is alive
//	if (sum < 2) return dead;
//	if (sum > 3) return dead;
//}
//return pixelCenter;
