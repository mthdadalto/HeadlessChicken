#version 450

precision highp float;
precision highp int;

layout(set = 0, binding = 0) uniform VertUniformsInfo
{
    mat4 ModelViewProjection;
};

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec2 TexCoord;

layout(location = 0) out vec3 out_Normal;
layout(location = 1) out vec2 out_TexCoord;

void main()
{
    gl_Position = ModelViewProjection * vec4(Position, 1);

    out_Normal = Normal;

    out_TexCoord = TexCoord;
}
