#version 450 core

// FILE:	PlainPipeShader.05-Fragment.c
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

in vec4 vs_color;
out vec4 color;

void main(void)
{
	color = vs_color;
}
