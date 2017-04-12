#version 450 core

// FILE:	TexPipeShader.05-Fragment.c
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

uniform sampler2D textureObject;

in vec2 vs_textureCoordinate;
out vec4 color;

void main(void)
{
	color = texture(textureObject, vs_textureCoordinate);
}
