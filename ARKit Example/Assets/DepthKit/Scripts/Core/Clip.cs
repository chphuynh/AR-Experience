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
using System.Collections.Generic;
using System.Text;
using System.IO; 
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DepthKit {

	/// <summary>
    /// Where the video file is located relative to the selection </summary>
    /// <remarks>
    /// This is used for AVProVideo, but only exposed if that option is selected as the backend </remarks>
	public enum FileLocation {
		AbsolutePathOrURL,
		RelativeToProjectFolder,
		RelativeToStreamingAssetsFolder,
		RelativeToDataFolder,
		RelativeToPeristentDataFolder,
	}

	/// <summary>
    /// The type of rendering that should be used for DepthKit clip </summary>
    /// <remarks>
    /// Users can extend rendering by placing a new renderer in here. </remarks>
	public enum RenderType {
		Simple
	}
	
	/// <summary>
    /// A DepthKit clip </summary>
    /// <remarks>
    /// Class that holds DepthKit data and prepares clips for playback in the editor. </remarks>
	[RequireComponent (typeof(BoxCollider))]
	[ExecuteInEditMode] 
	public class Clip : MonoBehaviour {

		/// <summary>
		///  What kind of player backend is playing the Clip.</summary>
		[SerializeField]
		protected ClipPlayer _player;
		public ClipPlayer Player 
		{ 
			get
			{
				return _player;
			} 
			
			protected set
			{
				_player = value;
			}
		}

		/// <summary>
		/// Interface to control the player</summary>
		[SerializeField]
		public IDKController Controller 
		{ 
			get 
			{
				return (IDKController)Player;
			}
		}
		public PlayerEvents Events 
		{ 
			get 
			{
				if (Player != null) 
				{ 
					return Player.Events;
				}
				Debug.LogError("Unable to access events as player is currently null");
				return null;
			}
		}
		/// <summary>
		///  What kind of renderer backend is playing the Clip.</summary>
		[SerializeField]
		protected DepthKit.ClipRenderer _renderer;

		/// <summary>
		/// The bounding box collider</summary>
		[SerializeField]
		protected BoxCollider _collider;

		public static AvailablePlayerType _defaultPlayerType = AvailablePlayerType.UnityVideoPlayer;

		/// <summary>
		/// The type of player, as expressed through the Unity Inspector.</summary>
		public AvailablePlayerType _playerType;

		/// <summary>
		/// The type of renderer, as expressed through the Unity Inspector.</summary>
		public RenderType _renderType = RenderType.Simple;

		/// <summary>
		/// The metadata file that cooresponds to a given clip. This is exported from Visualize.</summary>
		public TextAsset _metaDataFile;

#region Imported DepthKit Data

		/// <summary>
		/// The poster frame for a DepthKit capture.</summary>
		public Texture2D _poster;
		/// <summary>
		/// Use a VideoClip if in version 5.6 as the default</summary>
		public UnityEngine.Video.VideoClip _videoClip;

		/// <summary>
		/// String path of movie. This field has different requirements depending on what player is being used.</summary>
		public string _moviePath;

		/// <summary>
		/// For AVProVideo, this says where the video is located relative to other aspects of the project.</summary>
		public FileLocation _fileLocation;

		/// <summary> Reference to the metadata object fromed from the imported metadata file</summary>
		private Metadata _metaData;
#endregion

		/// <summary>Should the player backend be updated</summary>
		public bool _needToResetPlayerType;
		/// <summary>Should the player values be updated</summary>
		public bool _needToRefreshPlayerValues;
		/// <summary>Should the renderer backend be updated</summary>
		public bool _needToRefreshRenderType;
		/// <summary>Should the renderer backend be updated</summary>
		public bool _needToRefreshMetadata;
		public bool _autoPlay = true;
		public bool _autoLoad = true;
		public float _delaySeconds = 0.0f;
		public bool _videoLoops = false;
		public float _volume = 0.5f;
		private bool _autoplayTriggered = false;
		private bool _autoLoadTriggered = false;
		/// <summary>Wheter or not the clip is fully setup</summary>
		public bool _clipSetup;
		/// <summary>Says whether or not the player has been setup.</summary>
		public bool _playerSetup;
		/// <summary>Says whether or not the metadata and poster have been setup.</summary>
		public bool _metaSetup;

		void Start () {	
			_autoplayTriggered = false;
			_autoLoadTriggered = false;

			_needToResetPlayerType = false;
			_needToRefreshMetadata = false;
			_needToRefreshRenderType = false;

			_collider = GetComponent<BoxCollider>();

		}

		void Reset()
		{
			_playerType = _defaultPlayerType;
		}


		void Update () {

			if (_renderer == null || _needToRefreshRenderType) {
				RefreshRenderer ();
				_needToRefreshRenderType = false;
			}

			if (_player == null || _needToResetPlayerType ) {
				ResetPlayer();
				_needToResetPlayerType = false;
			}

			if (_needToRefreshPlayerValues)
			{
				RefreshPlayerValues();
				_needToRefreshPlayerValues = false;
			}

			if (_needToRefreshMetadata)
			{
				RefreshMetaData();
				_needToRefreshMetadata = false;
			}

			if (_playerSetup && _metaSetup)
			{
				_clipSetup = true;
			}

			else 
			{
				_clipSetup = false;
			}


			if(_clipSetup)
			{
				//note everything here gets called during runtime 
				//this can occur when directive shifting occurs
				if(Player == null)
				{
					Debug.Log("player is null");
					_playerSetup = false;
					_needToResetPlayerType = true;
					return;
				}

				//set player params based on selected Clip settings
				Controller.SetLoop(_videoLoops);
				Controller.SetVolume(_volume);

				//Get the texture from the provider!
				_renderer.Texture = Controller.GetTexture();
				_renderer.TextureIsFlipped = Controller.IsTextureFlipped ();
			}


			//properly start the video for autoplay modes
			if (Application.isPlaying && _clipSetup)
			{
				if (_autoLoad && !_autoLoadTriggered)
				{
					_autoLoadTriggered = true;
					Controller.StartVideoLoad();
				}
				if (_autoPlay && !_autoplayTriggered && Player.VideoLoaded && Time.time > _delaySeconds)
				{
					Controller.Play();
					_autoplayTriggered = true;	
				}
			}	

		}

		void OnDrawGizmos() {

			if (Application.isPlaying && _metaData != null) {
				Gizmos.color = new Color (.5f, 1.0f, 0, 0.5f);
				Gizmos.DrawWireSphere (
					transform.localToWorldMatrix * new Vector4 (_metaData.boundsCenter.x, _metaData.boundsCenter.y, _metaData.boundsCenter.z, 1.0f), 
					transform.localScale.x * _metaData.boundsSize.x * .5f);
			}	
		}

		void OnApplicationQuit()
		{
			Controller.Stop();
		}

		/// <summary>
		/// Called when player vars are changed but player itself isn't changed </summary>
		public void RefreshPlayerValues()
		{
			switch (_playerType)
			{
#if DK_USING_MPMP
				case AvailablePlayerType.MPMP:
				{
					_playerSetup = ((MPMPPlayer)_player).Setup(_moviePath);
					break;
				}
#endif
#if DK_USING_AVPRO
				case AvailablePlayerType.AVProVideo:
				{
					_playerSetup = ((AVProVideoPlayer)_player).Setup(_moviePath, _fileLocation);
					break;
				}
#endif
				case AvailablePlayerType.UnityVideoPlayer:
				{
					_playerSetup = ((UnityVideoPlayer)_player).Setup(_videoClip);
					break;
				}
            }
		}

		/// <summary>
		/// Called when the player itself changes </summary>
		public void ResetPlayer()
		{
			//try to set the player variable to any Player type component on this script
			Player = gameObject.GetComponent<ClipPlayer>();
			//if player not yet exists or
			//if a player is found that isn't the current targeted player
			if ((Player != null) && !_needToResetPlayerType) { return; }
			
			//destroy the components that player references
			//use a for loop to get around the component potentially shifting in the event of an undo
			IDKController[] attachedPlayers = GetComponents<IDKController>();
			for (int i = 0; i < attachedPlayers.Length; i++)
			{
				attachedPlayers[i].RemoveComponents();	
			}
			Player = null;

			//add the new components
			switch (_playerType)
			{
#if DK_USING_MPMP
				case AvailablePlayerType.MPMP:
				{
					MPMPPlayer newPlayer = gameObject.AddComponent<MPMPPlayer> ();
					_playerSetup = newPlayer.Setup(_moviePath);
					Player = newPlayer;
					break;
				}
#endif
#if DK_USING_AVPRO
				case AvailablePlayerType.AVProVideo:
				{
					AVProVideoPlayer newPlayer = gameObject.AddComponent<AVProVideoPlayer> ();
					_playerSetup = newPlayer.Setup(_moviePath, _fileLocation);
					Player = newPlayer;
					break;
				}
#endif
				case AvailablePlayerType.UnityVideoPlayer:
				{
					UnityVideoPlayer newPlayer = gameObject.AddComponent<UnityVideoPlayer> ();
					_playerSetup = newPlayer.Setup(_videoClip);
					Player = newPlayer;
					break;
				}
            }

        }

		public void RefreshRenderer(){

			_renderer = gameObject.GetComponent<DepthKit.ClipRenderer>();
			if (_renderer != null)
			{
				if (!Application.isPlaying) {
					DestroyImmediate (_renderer);
				}
				else {
					Destroy (_renderer);
				}
			}

			//add the new components
			switch (_renderType)
			{
			case RenderType.Simple:
				_renderer = gameObject.AddComponent<SimpleRenderer> ();
				break;
			/* Not implemented yet
			case RenderType.Hologram:
				Debug.Log("adding hologram renderer");
				_renderer = gameObject.AddComponent<HologramRenderer> ();
				break;
			case RenderType.PhysicallyBased:
				Debug.Log("adding physically based renderer");
				//TODO
				//renderer = gameObject.AddComponent<PhysicallyBasedRenderer> ();
				_renderer = gameObject.AddComponent<SimpleRenderer> ();
				break;
			*/
			default:
				Debug.Log ("adding simple renderer");
				_renderer = gameObject.AddComponent<SimpleRenderer> ();
				break;
			}

			RefreshMetaData ();
		}

		void RefreshMetaData()
		{
			if(_metaDataFile != null)
			{
				try {
					_metaData = Metadata.CreateFromJSON(_metaDataFile.text);

				} catch (System.Exception)
				{
					Debug.LogError("Invaid DepthKit Metadata Format. Make sure you are using the proper metadata export from DepthKit Visualize.");
					_metaSetup = false;
					return;
				}

				_collider.center = _metaData.boundsCenter;
				_collider.size = _metaData.boundsSize;

				if (_renderer) {
					_renderer.Metadata = _metaData;
					_renderer.Poster = _poster;
				}

				//small check to make sure poster is valid
				if (_poster != null)
				{
					_metaSetup = true;
					return;
				}

				_metaSetup = false;
			}

			_metaSetup = false;
			
		}
	}

}