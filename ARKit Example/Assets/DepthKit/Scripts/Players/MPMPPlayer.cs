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
using System;
using System.Collections;


namespace DepthKit
{
    /// <summary>
    /// Implementation of the DepthKit player with an MPMP-based backend </summary>
#if (!DK_USING_MPMP)
    public class MPMPPlayer : ClipPlayer {
#else
    public class MPMPPlayer : ClipPlayer, IDKController
    {
        // the MPMP player component 
        [SerializeField, HideInInspector]
        /// <summary>
        /// Reference to the MPMP component attached to this script. </summary>
        protected monoflow.MPMP _mediaPlayer;
        
        // [SerializeField, HideInInspector]
        /// <summary>
        /// The path to the movie for MPMP to play </summary>
        // protected string _moviePath;


        /// <param name="moviePath">Filename of the movie</param>
        public bool Setup(string moviePath)
        {
            // Debug.Log("setting up mpmp");
            _mediaPlayer = gameObject.GetComponent<monoflow.MPMP>();
            //if no mpmp component is already attached to this script
            if (_mediaPlayer == null)
            {
                // Debug.Log("no mpmp component found on script");
                //try adding an mpmp component
                try
                {
                    _mediaPlayer = gameObject.AddComponent<monoflow.MPMP>();
                }

                catch (Exception e)
                {
					Debug.LogError("MPMP not found in project: " + e.ToString());
                    return false;
                }
            }

            //ensure we are attached to the correct MPMP component
            // _mediaPlayer = gameObject.GetComponent<MPMP>();

            if (moviePath == "")
            {
                return false;
            }

            else
            {
                //set the path of the movie based on passed in params
                // _moviePath = moviePath;
                _mediaPlayer.videoPath = moviePath;
                //MPMP needs the video to be loaded
                // _mediaPlayer.Load(_moviePath);
                // Debug.Log("MPMPPlayer Setup");
                return true;
            }

        }

        // public override MPMPPlayer GetPlayer<MPMPPlayer>()
        // {
        //     return null;
        // }
        
        public IEnumerator Load()
        {
            //start the loading operation
            _mediaPlayer.Load(_mediaPlayer.videoPath);
            Events.OnClipLoadingStarted();

            //while the video is loading you can't play it
            while(_mediaPlayer.IsLoading())
            {
                VideoLoaded = false;
                yield return null;                
            }

            VideoLoaded = true;
            Events.OnClipLoadingFinished();
            yield return null;
        }

        public void StartVideoLoad()
        {
            StartCoroutine(Load());
        }

        public IEnumerator LoadAndPlay()
        {
            StartVideoLoad();
            while (!VideoLoaded)
            {
                yield return null;
            }
            Play();
            yield return null;
        }

        public void Play()
        {
            if(VideoLoaded)
            {
                _mediaPlayer.Play();
                Events.OnClipPlaybackStarted();
            }
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
            Events.OnClipPlaybackPaused();
        }
        public void Stop()
        {
            _mediaPlayer.Stop();
            Events.OnClipPlaybackStopped();
        }

        public void SetLoop(bool loopStatus)
        {
            _mediaPlayer.looping = loopStatus;
        }

        public void SetVolume(float volume)
        {
            _mediaPlayer.volume = volume;
        }
        public void SeekTo(float time)
        {
            time = time % (float)_mediaPlayer.GetDuration();
            _mediaPlayer.SeekTo((float)time, true);
        }

        public double GetCurrentTime()
        {
            return _mediaPlayer.GetCurrentPosition();
        }

        public double GetDuration()
        {
            return _mediaPlayer.GetDuration();
        }

        public Texture GetTexture()
        {
            return _mediaPlayer.GetVideoTexture();
        }
		public bool IsTextureFlipped ()
		{
#if (UNITY_ANDROID && !UNITY_EDITOR)
            return false;
#endif
			return true;
		}

        public bool IsPlaying()
        {
            return _mediaPlayer.IsPlaying();
        }

        public void RemoveComponents()
        {
            if(!Application.isPlaying)
            {
                DestroyImmediate(_mediaPlayer, true);
                DestroyImmediate(this, true);
            }
            else
            {
                Destroy(_mediaPlayer);
                Destroy(this);
            }
        }

        public AvailablePlayerType GetPlayerType()
        {
            return AvailablePlayerType.MPMP;
        }

        public monoflow.MPMP GetPlayerBackend()
        {
            return _mediaPlayer;
        }
#endif
    }
}