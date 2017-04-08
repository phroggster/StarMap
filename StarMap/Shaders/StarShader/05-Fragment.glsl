#version 450 core

// FILE:	StarShader.05-Fragment.c

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
	//color = fract(sin(dot(vs_color.xyz, vec3(12.9898, 78.233, 0.7))) * 43758.5453);
	color = vs_color;
}
