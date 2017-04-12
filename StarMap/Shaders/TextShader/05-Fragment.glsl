#version 450 core

// FILE:	TextShader.05-Fragment.c
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

in vec2 vs_textureOffset;
in vec4 vs_color;
out vec4 color;

void main(void)
{
	vec4 alpha = texture(textureObject, vs_textureOffset);
	color = vs_color;
	color.a = alpha.r;
}
