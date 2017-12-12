#version 120

uniform float time;
uniform sampler2D textureVar;
uniform sampler3D textureVar3D;

uniform float testValue;
uniform float threshValue;
uniform float startThresh;

uniform vec3 LightPosition;
uniform int viewNormals;//1 if draw normals, 0 otherwise
uniform float numProxies;
uniform int origNumProxies;
uniform int isOpaque;
uniform int calcNormsOnFly;

uniform vec3 dim;//width,height,length

varying vec2 v_texCoord;
varying float xpos;
varying float ypos;
varying float zpos;


//material values
const vec3 Diffuse=vec3(1,0,0);
const vec3 Specular=vec3(0.5,0.5,0.5);
const float shininess=8;
//lighting values
const vec3 position=vec3(1,1,1);//for now not used, usinf view direciton as light direction
const vec3 diffuseColor=vec3(1,1,1);
const vec3 diffusePower=vec3(1,1,1);
const vec3 specularColor=vec3(1,1,1);
const vec3 specularPower=vec3(0.5,0.5,0.5);


uniform float hithresh=0.1;
float transferOpacity(float orig){
float newOpacity=orig;
if(isOpaque==1){
		newOpacity=1;
}
	if(orig<(startThresh+threshValue)){
		newOpacity=(orig*orig)/20;//20 is a temp value representing the number of initial slices
	}
	newOpacity=1-pow((1-newOpacity),(300.0)/numProxies);//the correciton term for opacity
	return newOpacity;
}



void main(void){
	bool try=true;
	vec3 loc=vec3(gl_TexCoord[0]);
	gl_FragColor  = texture3D(textureVar3D, loc).rgba; 

	if(try){

    	//stuff for lighting
    	//vec3 LightPosition=vec3(0,0.8,-1);//will need to pass this in as a uniform
		vec3 ambient = vec3(0,0.2,0);
		vec3 specular = vec3(0,1,1);
		vec3 emission = vec3(0.0,0,0);
		
	    	float x = gl_FragColor.g;//0.0f;
    	float y = gl_FragColor.b;//0.0f;
    	float z = gl_FragColor.a;//1.0f;
    	vec3 newNormal=vec3(x,y,z);
    	//calculate normal as gradient

    	
    	if(calcNormsOnFly==1){
    		float texelDiffx=1/dim.x;
    		float texelDiffy=1/dim.y;
    		float texelDiffz=1/dim.z;
    		float rdiffx=(texture3D(textureVar3D, loc-vec3(texelDiffx,0,0)).r) - (texture3D(textureVar3D, loc+vec3(texelDiffx,0,0)).r);
    		x=(rdiffx)/(2*texelDiffx);
    		float rdiffy=(texture3D(textureVar3D, loc-vec3(0,texelDiffy,0)).r) - (texture3D(textureVar3D, loc+vec3(0,texelDiffy,0)).r);
    		y=(rdiffy)/(2*texelDiffy);
    		float rdiffz=(texture3D(textureVar3D, loc-vec3(0,0,texelDiffz)).r) - (texture3D(textureVar3D, loc+vec3(0,0,texelDiffz)).r);
    		z=(rdiffz)/(2*texelDiffz);
    		newNormal=vec3(x,y,z);
    		
    	}
    	
    	if(length(newNormal)!=0){
    		newNormal=normalize(newNormal);
    	}
    	
		vec3 N=(newNormal);
    	vec3 L=normalize(LightPosition);
    	vec3 E = normalize(LightPosition); // we are in Eye Coordinates
    	vec3 R = normalize(reflect(-L,N)); 

		
   

		vec3 diffuse= vec3(1,1,1);//vec3(0.6,1,0.6);//
   		vec4 Ispec =vec4(specular,0) * pow(max(dot(R,E),0.0),shininess);
   		Ispec = clamp(Ispec, 0.0, 1.0); 
		vec3 Idiff = diffuse * max(dot(N,L), 0.0);  
    
    	float dist=sqrt(
    	(LightPosition.x- xpos)*(LightPosition.x- xpos)+
    	(LightPosition.y- ypos)*(LightPosition.y- ypos)+
    	(LightPosition.z- zpos)*(LightPosition.z- zpos));

    	Idiff = clamp(Idiff, 0.0, 1.0); 
    	vec4 Idiff2=vec4(Idiff,1);//store the distance from the camera in alpha channel
    
    	
    	gl_FragColor.a=transferOpacity(gl_FragColor.r);
  		gl_FragColor.rgb=vec4(Idiff2+Ispec).rgb;
	
		//intensity of diffuse light
		float NdotL=dot(N,L);
		float intensity=clamp(NdotL,0,1);

		//diffuse light factoring in light color, power, and attentuation (no attentuation for now)
		vec3 outDiffuse=intensity*diffuseColor*diffusePower;

		//the so called half vector
		vec3 H=L;//(L+V)/|L+V| since in my case L=V, H is just equal to L 

		//intensity of s[ecular light
		float NdotH = dot(N,H);
		intensity=pow(clamp(NdotH,0,1),shininess);

		//sum up specular light factoring
		vec3 outSpecular=intensity*specularColor*specularPower;

		//gl_FragColor.rgb=outSpecular+outDiffuse;
		

		//adjust the opacity--takes input from user
		//starts at 1, so it initially does nothing
    	//if(gl_FragColor.a>0.0f){
    		gl_FragColor.a= gl_FragColor.a*testValue;
    	//}	
    	
    	if(viewNormals==1){
			gl_FragColor.rgb=vec3(newNormal);
		}
    
    }
}