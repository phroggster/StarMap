#version 450 core

// FILE:	TextShader.05-Fragment.c

in vec2 vs_textureOffset;
in vec4 vs_color;

layout(std140, binding = 0) uniform ProjectionView
{
	mat4 projectionMatrix;
	mat4 viewMatrix;
	vec2 viewportSize;
};

uniform mat4 modelMatrix;
uniform sampler2D textureObject;

out vec4 color;

void main(void)
{
	vec4 alpha = texture(textureObject, vs_textureOffset);
	color = vs_color;
	color.a = alpha.r;
}
