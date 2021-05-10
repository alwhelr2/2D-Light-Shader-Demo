#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


matrix WorldViewProjection;
float4 walls[10];
float2 lightPos = float2(0, 0);
float4 lightColor = float4(1.0f, 1.0f, 1.0f, 1.0f);

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION0;
	float4 Color : COLOR0;
    float2 ScreenPos : TEXCOORD0;
};


int orientation(float2 p, float2 q, float2 r)
{
    int val = (q.y - p.y) * (r.x - q.x) -
		(q.x - p.x) * (r.y - q.y);
    if (val == 0)
        return 0;
    if (val > 0)
        return 1;
    return 2;
}

bool onSegment(float2 p, float2 q, float2 r)
{
    if (q.x <= max(p.x, r.x) && q.x >= min(p.x, r.x) &&
		q.y <= max(p.y, r.y) && q.y >= min(p.y, r.y))
        return true;
    return false;
}

bool intersect(float2 p1, float2 q1, float2 p2, float2 q2)
{
    int o1 = orientation(p1, q1, p2);
    int o2 = orientation(p1, q1, q2);
    int o3 = orientation(p2, q2, p1);
    int o4 = orientation(p2, q2, q1);

    if (o1 != o2 && o3 != o4)
        return true;

    if (o1 == 0 && onSegment(p1, p2, q1))
        return true;
    if (o2 == 0 && onSegment(p1, q2, q1))
        return true;
    if (o3 == 0 && onSegment(p2, p1, q2))
        return true;
    if (o4 == 0 && onSegment(p2, q1, q2))
        return true;

    return false;
}

float map(float value, float min1, float max1, float min2, float max2)
{
    return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}


VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
    
	output.Position = mul(input.Position, WorldViewProjection);
    float2 screenPos = input.Position.xy;
    float4 myColor = input.Color * lightColor;
    myColor.a = max(map(distance(screenPos, lightPos), 0.0f, 150.0f, 1.0f, 0.0f), 0.0f);
    
    [unroll(10)]
    for (uint i = 0; i < 10; i++)
    {
        float x1 = walls[i][0], y1 = walls[i][1];
        float x2 = walls[i][2], y2 = walls[i][3];
        if (intersect(screenPos, lightPos, float2(x1, y1), float2(x2, y2)) == true && (x1 != screenPos.x || y1 != screenPos.y) && (x2 != screenPos.x || y2 != screenPos.y))
        {
            myColor.a = 0.0f;
        }
    }
    
    output.Color = myColor;
    output.ScreenPos = screenPos;
	return output;
}


float4 MainPS(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};