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

namespace DepthKit
{
    public interface IDKController {
        /// <summary>
		/// Load the implemented player video.
		/// </summary>
        IEnumerator Load();

        /// <summary>
		/// Method to dispatch the video loader to start </summary>
        void StartVideoLoad();

        /// <summary>
		/// Load a video and then play through the implemented player.</summary>
        IEnumerator LoadAndPlay();

        /// <summary>
		/// Play through the implemented player. Worth using in combination with VideoLoaded to ensure playback will start when called. </summary>
        void Play();

        /// <summary>
		/// Pause through the implemented player. </summary>
        void Pause();

        /// <summary>
		/// Stop playback through the player. </summary>
        void Stop();

        /// <summary>
		/// Tell the implemented player wheter or not it should loop playback. </summary>
        void SetLoop(bool loopStatus);

        /// <summary>
		/// Set the volume of the video </summary>
        void SetVolume(float volume);

        /// <summary>
		/// Remove the player components from this GameObject. </summary>
        void RemoveComponents();

        /// <summary>
		/// Return the texture for DepthKit to use by Renderers </summary>
        Texture GetTexture ();

		/// <summary>
		/// Returns if texture generated is flipped </summary>
        bool IsTextureFlipped ();

        /// <summary>
		/// Return the type of player being used </summary>
        AvailablePlayerType GetPlayerType();

        /// <summary>
		/// Check if video is playing right now or not </summary>
        bool IsPlaying();

        /// <summary>
		/// Update the postion of the Playhead in the inspector </summary>
        //void UpdatePlayheadPosition();

        /// <summary>
		/// Go to a specific location in the provided video. Value should be in seconds. </summary>
        /// Values outside of the range will get mapped to their cooresponding position in the
        /// initial range.
        void SeekTo(float time);

        /// <summary>
		/// Get the current playback time of the video in seconds. </summary>
        double GetCurrentTime();

        /// <summary>
		/// Get duration of video in seconds </summary>
        double GetDuration();

        /// <summary>
		/// Get the player that implements this interface </summary>
        // T GetPlayer<T>() where T : ClipPlayer;

    }

}