/************************************************************************************

DepthKit Unity SDK License v1
Copyright 2016 Depth Kit Inc. All Rights reserved.  

Licensed under the Depth Kit Inc. Software Development Kit License Agreement (the "License"); 
you may not use this SDK except in compliance with the License, 
which is provided at the time of installation or download, 
or which otherwise accompanies this software in either electronic or hard copy form.  

You may obtain a copy of the License at http://www.depthkit.tv/license-agreement-v1

Unless required by applicable law or agreed to in writing, 
the SDK distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and limitations under the License. 

************************************************************************************/

//INCLUDE THIS IN YOUR INPUT STRUCT
#define DEPTHKIT_TEX_COORDS(idx, idx2, idx3) \
	float2 uv_MainTex : TEXCOORD##idx; \
	float2 uv2_MainTex2 : TEXCOORD##idx2; \
	float valid : TEXCOORD##idx3;

//INCLUDE THIS IF YOU ARE NOT USING A SURFACE SHADER
#define DEPTHKIT_TEX_ST \
			float4 _MainTex_ST; \
			float4 _MainTex2_ST;

static const float PI = 3.14159265f;
static const float epsilon = .03;
static const float2 filters = float2(.5, .9);
static const float maxDistance = .1; // no poly can fly more than x across the boundary

//PROPERTIES-- feel free to copy this into your shader, but not necessary

//		//Main texture is the combined color and depth video frame
//		_MainTex ("Texture", 2D) = "white" {}
//		_MainTex2 ("Texture", 2D) = "white" {} //we currently set the same texture twice due to a bug in unity to pass multiple texture coordinates
//		//Size of the actual texture being passed in
//		_TextureDimensions ("Texture Dimension", Vector) = (0, 0, 0, 0)
//		//Crop factor that shows where from the original depth frame the texture is sampling
//		_Crop ("Crop", Vector) = (0,0,0,0)
//		//Original depth frame image dimensions
//		_ImageDimensions ("Image Dimensions", Vector) = (0,0,0,0)
//		//Focal length X/Y in terms of pixels from the original depth image (_ImageDimensions)
//		_FocalLength ("Focal Length", Vector) = (0,0,0,0)
//		//Principal Point in terms of pixels from the original depth image (_ImageDimensions)
//		_PrincipalPoint ("Principal Point", Vector) = (0,0,0,0)
//		//Near and Far bounds of depth data range for this frame
//		_NearClip ("Near Clip", Float) = 0.0
//		_FarClip  ("Far Clip", Float) = 0.0
//		//Number of vertices (x/y) in textured mesh
//		_MeshDensity ("Mesh Density", Range(0,255)) = 128

//All DepthKit params
sampler2D _MainTex;
sampler2D _MainTex2;
float4 _MainTex_TexelSize;
float4 _MainTex2_TexelSize;

//float2 _TextureDimensions;
float4 _Crop;
float2 _ImageDimensions;
float2 _FocalLength;		
float2 _PrincipalPoint;
float _NearClip;
float _FarClip;
float4x4 _Extrinsics;
int _MeshDensity;
int _TextureFlipped;
int _LinearColorSpace;

fixed3 rgb2hsv(fixed3 c)
{
	fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	fixed4 p = lerp(fixed4(c.bg, K.wz), fixed4(c.gb, K.xy), step(c.b, c.g));
	fixed4 q = lerp(fixed4(p.xyw, c.r), fixed4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + epsilon)), d / (q.x + epsilon), q.x);

}

float depthForPoint(float2 texturePoint){

	float4 textureSample = float4( texturePoint.x, texturePoint.y, 0.0, 0.0);
	fixed4 depthsample = pow( tex2Dlod(_MainTex, textureSample), _LinearColorSpace == 1 ? 0.4545 : 1.0);
	fixed3 depthsamplehsv = rgb2hsv(depthsample.rgb);

	return depthsamplehsv.g > filters.x && depthsamplehsv.b > filters.y ? depthsamplehsv.r : 0.0;
}

//Used in the vertex shader pass to extract texture coordinates and spatial position
//from the input texture
void dkVertexPass(float4 vertIn, inout float2 colorTexCoord, inout float2 depthTexCoord, inout float4 vertOut, inout float valid){


	float2 centerpix = _MainTex_TexelSize.xy * .5;
	float2 textureStep = float2( 1.0 / (_MeshDensity - 1.0), 1.0 / (_MeshDensity - 1.0) );
	float2 basetex = floor(vertIn.xy * _MainTex_TexelSize.zw) * _MainTex_TexelSize.xy;

	//flip texture
	if(_TextureFlipped == 1){
		depthTexCoord = basetex * float2(1.0, 0.5) + float2(0.0, 0.5) + centerpix;
		colorTexCoord = basetex * float2(1.0, 0.5) + centerpix;
		basetex.y = 1.0 - basetex.y;
	}
	else{
		depthTexCoord = basetex * float2(1.0, 0.5) + centerpix;
		colorTexCoord = basetex * float2(1.0, 0.5) + float2(0.0, 0.5) + centerpix;
	}

	//check neighbors
	float2 neighbors[8] = {
		float2(			   0.,  textureStep.y),
		float2( textureStep.x,	 		   0.),
		float2(			   0., -textureStep.y),
		float2(-textureStep.x,			   0.),
		float2(-textureStep.x, -textureStep.y),
		float2( textureStep.x,  textureStep.y),
		float2( textureStep.x, -textureStep.y),
		float2(-textureStep.x,  textureStep.y)
	};

    //texture coords come in as [0.0 - 1.0] for this whole plane
	float depth = depthForPoint(depthTexCoord);

	int i;
	float neighborDepths[8];
	for (i = 0; i < 8; i++) {
		neighborDepths[i] = depthForPoint(depthTexCoord + neighbors[i]);
	}

	valid = 1.0;
	int numDudNeighbors = 0;
	//search neighbor verts in order to see if we are near an edge
	//if so, clamp to the surface closest to us
	if(depth < epsilon || (1.0 - depth) < epsilon){
		float depthDif = 1.0f;
		float nearestDepth = depth;
		for(int i = 0; i < 8; i++){
			//float depthNeighbor = depthForPoint(depthTexCoord + neighbors[i]);
			float depthNeighbor = neighborDepths[i];
			if(depthNeighbor >= epsilon && (1.0 - depthNeighbor) > epsilon){
				float thisDif = abs(nearestDepth - depthNeighbor);
				if(thisDif < depthDif){
					depthDif = thisDif;
					nearestDepth = depthNeighbor;
				}
			}
			else {
				numDudNeighbors++;
			}
		}

		depth = nearestDepth;
		
		if (depth < epsilon || (1.0-depth) < epsilon) {
			valid = 0.0;
		}
		if(numDudNeighbors > 6) {
			valid = 0.0;
		}
	}

	float2 imageCoordinates = _Crop.xy + (basetex * _Crop.zw);

	float z = depth * (_FarClip - _NearClip) + _NearClip;
	vertOut = float4((imageCoordinates.x * _ImageDimensions.x - _PrincipalPoint.x) * z / _FocalLength.x,
           			 (imageCoordinates.y * _ImageDimensions.y - _PrincipalPoint.y) * z / _FocalLength.y, 
            		  z, vertIn.w);

	vertOut = mul(_Extrinsics,vertOut);
}

void dkFragmentPass(float2 depthTexCoord, float2 colorTexCoord, inout float4 col, inout float valid){
	float3 depth = tex2D (_MainTex, depthTexCoord).rgb;
	float3 depthhsv = rgb2hsv(depth);
	col.rgb = tex2D (_MainTex, colorTexCoord).rgb;

	//attenuate the alpha by the saturation & value. each pix should be fully saturated
	col.a = depthhsv.g * depthhsv.b; 
	valid *= (depthhsv.r > epsilon && depthhsv.g > filters.x && depthhsv.b > filters.y) ? 1.0 : 0.0;
}
