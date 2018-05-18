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

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace DepthKit
{

	public enum PlayerType {
		MPMP = 2,
		AVProVideo = 3,
		UnityVideoPlayer = 4
	};

	/// <summary>
	/// The type of player that will be used as the backend for DepthKit playback 
	/// </summary>
	/// <remarks>
	/// Users can choose any of these, but playback will not be successful unless
	/// the user has imported a give player into the project.
	/// </remarks>
	public enum AvailablePlayerType {
#if DK_USING_MPMP
		MPMP = PlayerType.MPMP,
#endif
#if DK_USING_AVPRO
		AVProVideo = PlayerType.AVProVideo,
#endif
		UnityVideoPlayer = PlayerType.UnityVideoPlayer
	}

    public class DepthKitPlugin {

		public static Dictionary<string, PlayerType> DirectiveDict = new Dictionary<string, PlayerType>(){
			{"DK_USING_AVPRO", PlayerType.AVProVideo},
			{"DK_USING_MPMP", PlayerType.MPMP}
		};

        public static string GetVersionID()
        {
            return "0.1.9";
        }

#if UNITY_EDITOR
        public static BuildTargetGroup[] SupportedPlatforms = new BuildTargetGroup[] {
            BuildTargetGroup.Android,
#if !UNITY_5_5_OR_NEWER
            BuildTargetGroup.Nintendo3DS,
#endif
            BuildTargetGroup.PS4,
            BuildTargetGroup.PSP2,
            BuildTargetGroup.Standalone,
            BuildTargetGroup.tvOS,
            BuildTargetGroup.WiiU,
            BuildTargetGroup.XboxOne
        };
#endif
    }
}