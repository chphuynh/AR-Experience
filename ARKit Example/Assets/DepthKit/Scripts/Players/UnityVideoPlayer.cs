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
using UnityEngine.Video;

namespace DepthKit
{
    /// <summary>
    /// Implmentation of the DepthKit player with the Unity VideoPlayer-based backend.
    /// </summary>
    public class UnityVideoPlayer : ClipPlayer, IDKController
    {
        //reference to the MovieTexture passed in through Clip
        [SerializeField, HideInInspector]
		protected VideoPlayer _player;
        [SerializeField, HideInInspector]
        AudioSource _audioSource;

        /// <param name="clip">VideoClip reference</param>
        public bool Setup(VideoClip clip)
        {
            if ( clip == null)
            {
                return false;
            }

            else
            {
                _player = gameObject.GetComponent<VideoPlayer>();
                _audioSource = gameObject.GetComponent<AudioSource>();

                if (_audioSource == null)
                {
                    _audioSource = gameObject.AddComponent<AudioSource>();
                }
                if (_player == null)
                {
                    _player = gameObject.AddComponent<VideoPlayer>();
                }

                _player.audioOutputMode = VideoAudioOutputMode.AudioSource;
                _player.SetTargetAudioSource(0, _audioSource);
                _player.renderMode = VideoRenderMode.APIOnly;
                _player.clip = clip;
                _player.EnableAudioTrack(0, true);
                _player.playOnAwake = false;
                _player.waitForFirstFrame = false;
                // Debug.Log(_player.controlledAudioTrackCount);
                return true;
            }
        }

        public void StartVideoLoad()
        {
            StartCoroutine(Load());
        }

        public IEnumerator Load()
        {
            _player.prepareCompleted += OnVideoLoadingComplete;
            _player.Prepare();
            Events.OnClipLoadingStarted();
            yield return null;
        }

        public void OnVideoLoadingComplete(VideoPlayer player)
        {
            VideoLoaded = true;
            Events.OnClipLoadingFinished();
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
            _player.Play();
            Events.OnClipPlaybackStarted();
        }
        public void Pause()
        {
            _player.Pause();
            Events.OnClipPlaybackPaused();
        }
        public void Stop()
        {
            _player.Stop();
            Events.OnClipPlaybackStopped();
        }

        public void SetLoop(bool loopStatus)
        {
            _player.isLooping = loopStatus;
        }

        public void SetVolume(float volume)
        {
            //need to set with audio source instead of trying to do directaudio
            // _player.volume = volume;
        }
        public void SeekTo(float time)
        {
            double seekFrame =_player.clip.frameRate * time;
            _player.frame = (long)seekFrame;
        }

        public double GetCurrentTime()
        {
            return _player.time;
        }

        public double GetDuration()
        {
            return _player.clip.length;
        }

		public Texture GetTexture()
        {
			return _player.texture;
        }
		public bool IsTextureFlipped ()
		{
			return false;
		}

        public bool IsPlaying()
        {
            return _player.isPlaying;
        }

        public void RemoveComponents()
        {
            if(!Application.isPlaying)
            {
                DestroyImmediate(_player, true);
                DestroyImmediate(_audioSource, true);
                DestroyImmediate(this, true);
            }
            else
            {
                Destroy(_player);
                Destroy(_audioSource);
                Destroy(this);
            }
        }
        public AvailablePlayerType GetPlayerType()
        {
            return AvailablePlayerType.UnityVideoPlayer;
        }

        public VideoPlayer GetPlayerBackend()
        {
            return _player;
        }
    }
}