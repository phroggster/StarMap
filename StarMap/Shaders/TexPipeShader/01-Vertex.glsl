#version 450 core

// FILE:	TexPipeShader.01-Vertex.c
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

in vec4 Position;
in vec2 TexCoord;
out gl_PerVertex	// v450 built-in
{
	vec4 gl_Position;
	float gl_PointSize;
	float gl_ClipDistance[];
	float gl_CullDistance[];
};
out vec2 vs_textureCoordinate;

void main(void)
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * Position;
	vs_textureCoordinate = TexCoord;
}
