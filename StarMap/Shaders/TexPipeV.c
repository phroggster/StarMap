#version 450 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec2 textureCoordinate;

out vec2 vs_textureCoordinate;

layout(location = 20) uniform mat4 projection;
layout(location = 21) uniform mat4 view;
layout(location = 22) uniform mat4 model;

void main(void)
{
	vs_textureCoordinate = textureCoordinate;
	gl_Position = projection * view * model * position;
}
