#version 450 core
in vec4 vs_color;
out vec4 color;

void main(void)
{
	//color = fract(sin(dot(vs_color.xyz, vec3(12.9898, 78.233, 0.7))) * 43758.5453);
	color = vs_color;
}
