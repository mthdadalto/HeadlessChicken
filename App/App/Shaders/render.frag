#version 450

precision highp float;
precision highp int;


layout(set = 0, binding = 1) uniform FragLightsUniformsInfo
{
    vec3 lightdirection;
    vec3 lightambient;
    vec3 lightdiffuse;
    vec3 lightspecular;
};

layout(set = 0, binding = 2) uniform FragMaterialUniformsInfo
{
    vec3 materialdiffuse;
    vec3 materialspecular;
    vec3 viewDir;

    float materialshininess;
    float materialshininess1;
    float materialshininess2;
    float materialshininess3;
    
};

layout(set = 1, binding = 0) uniform texture2D Tex;
layout(set = 1, binding = 1) uniform sampler Samp;

layout(location = 0) in vec3 Normal;
layout(location = 1) in vec2 TexCoords;

layout(location = 0) out vec4 FragColor;

void main()
{
    if(TexCoords.x < .0)	
	{
        if(TexCoords.y < .0)	//transparent
	    {
		    FragColor = vec4(.0);
		    return;
	    }
        //pure black
		FragColor = vec4(.0,.0,.0,1);
		return;
	}

    vec3 norm = normalize(Normal);
   // mediump vec3 viewDir = normalize(viewPos);
    //mediump vec3 lightDir = normalize(-lightdirection);
    vec3 reflectDir = reflect(lightdirection, norm);

    vec3 diffColor = vec3(texture(sampler2D(Tex, Samp), TexCoords)) * (1+materialdiffuse);
    vec3 specColor = vec3(texture(sampler2D(Tex, Samp), TexCoords)) * (1+materialspecular);
    //vec3 diffColor = Normal * (1+materialdiffuse);
    //vec3 specColor = Normal * (1+materialspecular);
    
    // ambient
    vec3 ambient = diffColor * lightambient;

    // diffuse    
    float diff = max(dot(norm, -lightdirection), 0.0); //mediump float diffuseIntensity = clamp(dot(Normal, lightDir), 0, 1);
    vec3 diffuse = diffColor * diff * lightdiffuse;

    // specular
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), materialshininess1);
    vec3 specular = specColor * spec * lightspecular;


    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1);
}
