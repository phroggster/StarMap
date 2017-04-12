#version 450 core

// FILE:	StarShader.05-Fragment.c
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
	//color = fract(sin(dot(vs_color.xyz, vec3(12.9898, 78.233, 0.7))) * 43758.5453);
	color = vs_color;
}
