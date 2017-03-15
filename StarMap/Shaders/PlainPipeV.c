﻿#version 450 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color;

out vec4 vs_color;

layout(location = 20) uniform mat4 projection;
layout(location = 21) uniform mat4 view;
layout(location = 22) uniform mat4 model;

void main(void)
{
	gl_Position = projection * view * model * position;
	vs_color = color;
}
