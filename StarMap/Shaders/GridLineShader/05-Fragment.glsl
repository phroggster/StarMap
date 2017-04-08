#version 450 core

// FILE:	GridLineShader.05-Fragment.c

#define M_PI 3.1415926535897932384626433832795

layout(location = 1) uniform mat4 modelMatrix;
layout(location = 2) uniform vec4 diffuseColor;

in vec4 gl_FragCoord;	// screen coordinates + depth
in vec4 vs_color;
out vec4 color;

layout(std140, binding = 0) uniform ProjectionView
{
	mat4 projectionMatrix;
	mat4 viewMatrix;
	vec2 viewportSize;
	vec2 padding;
};

float distfromcenter(vec2 fragpos)
{
	
	return clamp(abs(length((viewportSize / 2) - fragpos)), 0, 1);

	//return clamp(length(fragpos / viewportSize), 0, 1);
}

void main(void)
{
	color = vec4(vec3(vs_color), distfromcenter(gl_FragCoord.xy));
	//color = vec4(vec3(vs_color), clamp((2.0 - sec(0.75 * M_PI * gl_FragCoord.x)), 0.125, 0.875));
	//clamp((2.0 - sec(0.75 * M_PI * gl_FragCoord.x)), 0.125, 0.875)
	//color = vec4(vec3(vs_color), clamp(log(sin(M_PI * viewportSize.x / gl_FragCoord.x)) * vs_color.a, 0.125, 0.875));
}
