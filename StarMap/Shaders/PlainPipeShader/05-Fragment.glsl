#version 450 core

// FILE:	PlainPipeShader.05-Fragment.c

in vec4 gl_FragCoord;	// screen coordinates + depth
in vec4 vs_color;

layout(std140, binding = 0) uniform ProjectionView
{
	mat4 projectionMatrix;
	mat4 viewMatrix;
	vec2 viewportSize;
};

uniform mat4 modelMatrix;

out vec4 color;

void main(void)
{
	color = vs_color;
}
