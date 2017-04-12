#version 450 core

// FILE:	GridLineShader.05-Fragment.c
#define CoarseVertCount 34
#define M_PI 3.1415926535897932384626433832795

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
layout(std140) uniform GridLineData
{
	vec4 coarseColor;
	vec4 fineColor;
	int coarseVertCount;
};

// Custom attribute pins
in vec4 vs_color;
out vec4 color;

// version 450 built-ins
in vec4 gl_FragCoord;	// screen coordinates + depth

/*float distfromcenter(vec2 fragpos)
{
	return clamp(abs(length((viewportSize / 2) - fragpos)), 0, 1);

	//return clamp(length(fragpos / viewportSize), 0, 1);
}*/

void main(void)
{
	color = vs_color;
	//color = vs_color * distfromcenter(gl_FragCoord.xy);
	//color = vec4(vec3(vs_color), distfromcenter(gl_FragCoord.xy));
	//color = vec4(vec3(vs_color), clamp((2.0 - sec(0.75 * M_PI * gl_FragCoord.x)), 0.125, 0.875));
	//clamp((2.0 - sec(0.75 * M_PI * gl_FragCoord.x)), 0.125, 0.875)
	//color = vec4(vec3(vs_color), clamp(log(sin(M_PI * viewportSize.x / gl_FragCoord.x)) * vs_color.a, 0.125, 0.875));
}
