#version 450 core

// FILE:	GridLineShader.01-Vertex.c

layout(location = 0) in vec4 position;
layout(location = 1) uniform mat4 modelMatrix;
layout(location = 2) uniform vec4 diffuseColor;

out vec4 vs_color;

layout(std140, binding = 0) uniform ProjectionView
{
	mat4 projectionMatrix;
	mat4 viewMatrix;
	vec2 viewportSize;
	vec2 padding;
};

void main(void)
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;
	vs_color = diffuseColor;
}
