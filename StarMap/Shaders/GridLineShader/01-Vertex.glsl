#version 450 core

// FILE:	GridLineShader.01-Vertex.c
layout(std140) uniform ProjectionView
{
	mat4 projectionMatrix;
	mat4 viewMatrix;
	vec2 viewportSize;
};
layout(std140) uniform Model
{
	mat4 modelMatrix;
};
layout(std140) uniform GridLineData
{
	vec4 coarseColor;
	vec4 fineColor;
	int coarseVertCount;
};

in vec4 Position;
in int gl_VertexID;	// v450 built-in
out gl_PerVertex	// v450 built-in
{
	vec4 gl_Position;
	float gl_PointSize;
	float gl_ClipDistance[];
	float gl_CullDistance[];
};
out vec4 vs_color;

void main(void)
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * Position;

	if (gl_VertexID >= coarseVertCount)
	{
		vs_color = fineColor;
	}
	else
	{
		vs_color = coarseColor;
	}

	// Normalized device coordinates (-1,-1,-1 to 1,1,1) are:
	//vec3(gl_Position.xyz/gl_Position.w);
}
