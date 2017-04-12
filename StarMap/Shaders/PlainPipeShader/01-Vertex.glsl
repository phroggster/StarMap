#version 450 core

// FILE:	PlainPipeShader.01-Vertex.c
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

in vec4 position;
in vec4 color;
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
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;
	vs_color = color;
}
