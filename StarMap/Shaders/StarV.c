#version 450 core

layout(location = 0) in vec4 position;

out vec4 vs_color;

layout(location = 20) uniform mat4 projection;
layout(location = 21) uniform mat4 view;
layout(location = 22) uniform mat4 model;

void main(void)
{
	gl_Position = projection * view * position * model;
	vs_color = vec4(0.8, 0.8, 0.8, 0.8);
}
