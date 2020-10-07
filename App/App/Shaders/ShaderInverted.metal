#include <metal_stdlib>
#include <simd/simd.h>
#include <metal_matrix>
#include <metal_geometric>

using namespace metal;

struct VertUniformsInfo
{
	float4x4 View;
};

struct LightInfo
{
	float4 lightdirection;
	float4 lightambient;
	float4 lightdiffuse;
	float4 lightspecular;
};

struct MaterialInfo
{
	float4 materialdiffuse;
	float4 materialspecular;
	float4 viewDir;

	float4 materialshininess;
};

struct VertexInput{
	float3 Position [[attribute(0)]];
	float3 Normal [[attribute(1)]];
	float2 Tex [[attribute(2)]];
};

struct InOut {
	float4 Position [[position]];
	float3 Normal;
	float2 Tex;
};

vertex InOut shader_vertex(VertexInput in [[stage_in]], constant float4x4& ModelViewProjection [[buffer(0)]])
{
	InOut out;
	out.Position = ModelViewProjection * float4(in.Position,1);
	out.Normal = in.Normal;
	out.Tex = in.Tex;

	return out;
}

fragment float4 shader_fragment(
	InOut in [[stage_in]], 
	constant LightInfo& light [[buffer(1)]], 
	constant MaterialInfo& material [[buffer(2)]],
	texture2d<float, access::sample> texture [[texture(0)]],
	sampler texSampler [[sampler(0)]] 
)
{
	if (in.Tex.x < .0)
	{
		if (in.Tex.y < .0) { return float4(0); }//transparent
		return float4(0, 0, 0, 1);//pure black
	}
	
	float3 tmpColor = texture.sample(texSampler, in.Tex).xyz;

	float3 norm = normalize(in.Normal);

	float3 reflectDir = reflect(light.lightdirection.xyz, norm);

	float3 diffColor = tmpColor * (1 + material.materialdiffuse.xyz);
	float3 specColor = tmpColor * (1 + material.materialspecular.xyz);

	// ambient
	float3 ambient = diffColor * light.lightambient.xyz;

	// diffuse    
	float diff = max(dot(norm, -light.lightdirection.xyz), 0.0);
	float3 diffuse = diffColor * diff * light.lightdiffuse.xyz;

	// specular
	float spec = pow(max(dot(material.viewDir.xyz, reflectDir), 0.0), material.materialshininess.x);
	float3 specular = specColor * spec * light.lightspecular.xyz;

	float3 result = ambient + diffuse + specular;
	return float4(result.zyx,1);
}