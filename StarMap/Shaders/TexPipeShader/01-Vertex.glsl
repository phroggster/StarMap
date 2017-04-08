#version 450 core

// FILE:	TexPipeShader.01-Vertex.c

layout(location = 0) in vec4 position;
layout(location = 1) in vec2 textureCoordinate;

layout(std140, binding = 0) uniform ProjectionView
{
	mat4 projectionMatrix;
	mat4 viewMatrix;
	vec2 viewportSize;
};

uniform mat4 modelMatrix;

out vec2 vs_textureCoordinate;

void main(void)
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * position;
	vs_textureCoordinate = textureCoordinate;
}
