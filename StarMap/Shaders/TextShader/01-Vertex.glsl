#version 450 core

// FILE:	TextShader.01-Vertex.c

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 textureCoordinate;
layout(location = 3) in vec2 textureOffset;

layout(std140, binding = 0) uniform ProjectionView
{
	mat4 projectionMatrix;
	mat4 viewMatrix;
	vec2 viewportSize;
};

uniform mat4 modelMatrix;

out vec2 vs_textureOffset;
out vec4 vs_color;

void main(void)
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;
	vs_color = color;
	vs_textureOffset = textureCoordinate + textureOffset;
}
