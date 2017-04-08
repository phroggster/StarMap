#version 450 core

// FILE:	TexPipeShader.05-Fragment.c

in vec2 vs_textureCoordinate;

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
	color = texture(textureObject, vs_textureCoordinate);
}
