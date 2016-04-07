cbuffer frameInput : register(b2)
{
	const float4 FrameInfo; // window width, window height, LineThickness, nix
}

cbuffer frameInput2 : register(b3)
{
	const float4 StrokeInfo[10];
	const float4 StrokeInfo2[10];
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
	float4 Color = float4(1, 1, 1, 1);
	for (uint i = 0; i < 10 && StrokeInfo[i].x > 0; i++)
	{
		Color = StrokeInfo2[i];
		float2 vPos = floor(float2(input.tex.x * FrameInfo.x, input.tex.y * FrameInfo.y)); // pixel position
		float2 A = StrokeInfo[i].xy;                                                       // position punkt A der linie
		float2 B = StrokeInfo[i].zw;                                                       // position punkt B der linie
		float R = FrameInfo.z / 2;                                                         // Radius

		//if (round(vPos.x) == vPos.x && round(vPos.y) == vPos.y)
		//	return float4(1, 0, 0, 1);
		if (distance(vPos.xy, A) <= R || distance(vPos.xy, B) <= R) // start / end circle 
			return Color;
		else if (dot(vPos.xy - A, B - A) > 0 && dot(vPos.xy - B, A - B) > 0) // cutout between A & B 
		{
			float2 nAB = normalize(B - A);

			float sin_a = nAB.x;
			float cos_a = nAB.y;

			float2x2 M;
			M[0][0] = cos_a;
			M[0][1] = sin_a;
			M[1][0] = -sin_a;
			M[1][1] = cos_a;

			vPos.xy = mul((vPos.xy - A), M); // projection along AB 
			if (abs(vPos.x) <= R)
				return Color;
		}
	}
	discard;
	return Color;
}
