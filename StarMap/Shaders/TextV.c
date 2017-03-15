#version 450 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec2 textureCoordinate;
layout(location = 2) in vec2 textureOffset;
layout(location = 3) in vec4 color;

out vec2 vs_textureOffset;
out vec4 vs_color;

layout(location = 20) uniform mat4 projection;
layout(location = 21) uniform mat4 view;
layout(location = 22) uniform mat4 model;

void main(void)
{
	vs_textureOffset = textureCoordinate + textureOffset;
	gl_Position = projection * view * model * position;
	vs_color = color;
}
