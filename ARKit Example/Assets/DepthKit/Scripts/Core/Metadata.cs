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

using UnityEngine;
using System.Collections;

namespace DepthKit {

    [System.Serializable]
	public class Metadata
	{
		public int _versionMajor;
		public int _versionMinor;
		public int numAngles;

		public string format;
		public Vector2 depthImageSize;
		public Vector2 depthPrincipalPoint;
		public Vector2 depthFocalLength;
		public float farClip;
		public float nearClip;

		public int textureWidth;
		public int textureHeight;

		public Matrix4x4 extrinsics;
		public Vector3 boundsCenter;
		public Vector3 boundsSize;
		public Vector4 crop;
	

		public static Metadata CreateFromJSON(string jsonString)
		{
			Metadata md = JsonUtility.FromJson<Metadata>(jsonString);
			if (md.format == "perpixel" && md._versionMajor == 0 && md._versionMinor == 1) {
				
				//set final image dimensions
				md.textureWidth  = (int)(md.depthImageSize.x);
				md.textureHeight = (int)(md.depthImageSize.y)*2;


				//calculate bounds
				md.boundsCenter = new Vector3 (0f, 0f, (md.farClip - md.nearClip) / 2.0f + md.nearClip);
				md.boundsSize   = new Vector3(
					md.depthImageSize.x * md.farClip / md.depthFocalLength.x, 
					md.depthImageSize.y * md.farClip / md.depthFocalLength.y, 
					md.farClip - md.nearClip);

				md.numAngles = 1;
				md.crop = new Vector4 (0.0f, 0.0f, 1.0f, 1.0f);

				md.extrinsics = Matrix4x4.identity;
			}

			md.extrinsics = Matrix4x4.Inverse (md.extrinsics);
			return md;
		}


	}
}