struct VS_IN
{
    float4 pos : POSITION;
    float4 col : COLOR0;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float4 col : COLOR0;
};

PS_IN VS(VS_IN input)
{
    PS_IN output;
    output.pos = float4(input.pos.xy, 0, 1);
	output.col = input.col;
    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    return input.col;
}