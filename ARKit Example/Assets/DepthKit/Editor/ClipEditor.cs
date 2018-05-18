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
using UnityEditor;

namespace DepthKit {
	
	[CustomEditor(typeof(Clip))]
	[CanEditMultipleObjects]
	public class ClipEditor : Editor
	{

		//PLAYER PROPERTIES
		SerializedProperty _playerTypeProp;
		SerializedProperty _movieProp; 
		SerializedProperty _videoClipProp; 
		SerializedProperty _moviePathProp;
		SerializedProperty _fileLocationProp; 
		SerializedProperty _posterProp;
		SerializedProperty _metaDataFileProp;
		SerializedProperty _autoPlayProp;
		SerializedProperty _autoLoadProp;
		SerializedProperty _delaySecondsProp;
		SerializedProperty _videoLoopsProp;
		SerializedProperty _animationProp;
		SerializedProperty _volumeProp;

		//RENDERER PROPERTIES
		// SerializedProperty _renderTypeProp;

		//CLIP REFRESH PROPERTIES
		SerializedProperty _resetPlayerTypeProp;
		SerializedProperty _refreshPlayerValuesProp;
		SerializedProperty _refreshRenderTypeProp;
		SerializedProperty _refreshMetadataProp; 
		Texture2D logo;

		int cachedPlayerType;
		bool _needToUndoRedo;

		void OnEnable () {

			// subscribe to the undo event
			Undo.undoRedoPerformed += OnUndoRedo; 
			_needToUndoRedo = false;

			//set the property types
			_playerTypeProp 	    = serializedObject.FindProperty ("_playerType"); 
			_movieProp		   	    = serializedObject.FindProperty ("_movie");
			_videoClipProp		   	= serializedObject.FindProperty ("_videoClip");
			_moviePathProp 	   	    = serializedObject.FindProperty ("_moviePath");
			_fileLocationProp  	    = serializedObject.FindProperty ("_fileLocation");
			_animationProp 		    = serializedObject.FindProperty ("_clipAnimation"); 
			_posterProp 	   	    = serializedObject.FindProperty ("_poster");
			_metaDataFileProp  	    = serializedObject.FindProperty ("_metaDataFile");
			_autoPlayProp	   	    = serializedObject.FindProperty ("_autoPlay");
			_autoLoadProp	   	    = serializedObject.FindProperty ("_autoLoad");
			_delaySecondsProp 	    = serializedObject.FindProperty ("_delaySeconds");
			_videoLoopsProp 	    = serializedObject.FindProperty ("_videoLoops");
			_volumeProp             = serializedObject.FindProperty("_volume");
			cachedPlayerType = _playerTypeProp.enumValueIndex;

			// _renderTypeProp         = serializedObject.FindProperty ("_renderType");

			_resetPlayerTypeProp  = serializedObject.FindProperty ("_needToResetPlayerType");
			_refreshPlayerValuesProp  = serializedObject.FindProperty ("_needToRefreshPlayerValues");
			_refreshRenderTypeProp  = serializedObject.FindProperty ("_needToRefreshRenderType");
			_refreshMetadataProp    = serializedObject.FindProperty ("_needToRefreshMetadata");

			logo = Resources.Load("dk-logo-32", typeof(Texture2D)) as Texture2D;

		}

		void OnUndoRedo()
		{
			_needToUndoRedo = true;
 		}

	    public override void OnInspectorGUI()
	    {
			//update the object with the object variables
			serializedObject.Update ();
			
			//set the clip var as the target of this inspector
			Clip clip = (Clip)target;

			// DK INFO
			OnInspectorGUI_DepthKitInfo();

			EditorGUILayout.BeginVertical("Box");
			{
				// PLAYER INFO
				OnInspectorGUI_PlayerOptions(clip);
				// META INFO
				OnInspectorGUI_PlayerMetaInfo();
				EditorGUILayout.Space();
				// PLAYER SETUP FEEDBACK
				OnInspectorGUI_PlayerSetupInfo(clip);

				// if(GUILayout.Button("Hard Refresh")) {setAllPropsToRefresh();}
			}
			EditorGUILayout.EndVertical();

			// PLAYBACK OPTIONS
			OnInspectorGUI_PlaybackOptions();
			EditorGUILayout.Space ();
			OnInspectorGUI_CheckForUndo();

			// APPLY PROPERTY MODIFICATIONS
			serializedObject.ApplyModifiedProperties();
		}

		void OnInspectorGUI_DepthKitInfo()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			Rect rect = GUILayoutUtility.GetRect(logo.width, logo.height); GUI.DrawTexture(rect, logo);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField ("DepthKit Clip Editor", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Version " + DepthKitPlugin.GetVersionID());
			EditorGUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

		void OnInspectorGUI_PlayerSetupInfo(Clip clip)
		{
			if (clip._clipSetup)
			{
				GUI.backgroundColor = Color.green;
				EditorGUILayout.BeginVertical();
				// GUI.color = Color.black;
				EditorGUILayout.HelpBox("DepthKit clip is setup and ready for playback", 
										MessageType.Info);
			}

			else
			{
				GUI.backgroundColor = Color.red;
				EditorGUILayout.BeginVertical();
				// GUI.color = Color.black;
				EditorGUILayout.HelpBox("DepthKit clip is not setup. \n"
										+ string.Format("Clip Setup: {0} | Metadata Setup: + {1} | Player Setup: {2}",
										clip._clipSetup, clip._metaSetup, clip._playerSetup), 
										MessageType.Error);
			}
			EditorGUILayout.EndVertical();
			GUI.backgroundColor = Color.white;
		}

		void OnInspectorGUI_PlaybackOptions()
		{
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.LabelField ("Playback Options", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (_autoLoadProp);
			EditorGUILayout.PropertyField (_autoPlayProp);
			if(_autoPlayProp.boolValue)
			{
				_autoLoadProp.boolValue = true;
				EditorGUILayout.PropertyField (_delaySecondsProp);
			}
			EditorGUILayout.Slider(_volumeProp, 0.0f, 1.0f);
			EditorGUILayout.PropertyField (_videoLoopsProp);
			EditorGUILayout.EndVertical();
		}

		void OnInspectorGUI_PlayerOptions(Clip clip)
		{
			EditorGUILayout.LabelField ("Player Options", EditorStyles.boldLabel);
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_playerTypeProp);
			if (EditorGUI.EndChangeCheck () || (_playerTypeProp.enumValueIndex != cachedPlayerType)) {
				_resetPlayerTypeProp.boolValue = true;
				cachedPlayerType = _playerTypeProp.enumValueIndex;
			}

			//VIDEO PLAYER
			if (clip._playerType == (AvailablePlayerType)PlayerType.UnityVideoPlayer) {
				EditorGUI.BeginChangeCheck ();
				try {EditorGUILayout.PropertyField (_videoClipProp);}
				catch (System.NullReferenceException){
					if (_playerTypeProp.enumValueIndex < 0) {
						Debug.LogError ("Invalid player for current build target. Use the 'Mobile' prefab when using a mobile build target and 'Standalone' prefab when using a standalone build target.");
					}
				}
				if (EditorGUI.EndChangeCheck ()) {
					_refreshPlayerValuesProp.boolValue = true;
				}
			
			}

			if (clip._playerType == (AvailablePlayerType)PlayerType.AVProVideo || 
				clip._playerType == (AvailablePlayerType)PlayerType.MPMP) {

				//MPMP
				if (clip._playerType == (AvailablePlayerType)PlayerType.MPMP) {
					string helpString = ""
					+ "For MPMP, your Movie Path should just be the name of your video with its extension, " 
					+ "something like 'MyClip.mp4'. \n\n"
					+ "MPMP also requires your video be in the Assets/StreamingAssets folder,"
					+ "otherwise it will not be read and playback will not happen. If you do not already have a StreamingAssets folder "
					+ "in your project, just make a new folder called 'StreamingAssets' in Assets.";
					EditorGUILayout.HelpBox(helpString, MessageType.Info);
				}

				//AVPROVIDEO
				if (clip._playerType == (AvailablePlayerType)PlayerType.AVProVideo) {
					
					EditorGUI.BeginChangeCheck ();
					
					string helpString = ""
					+ "For AVProVideo, your Movie Path is the name of your video with its extension (MyClip.mp4 for example), relative to what "
					+ "you selected for 'File Location'. We recommend simply placing your video in the Assets/StreamingAssets folder "
					+ "(or creating it if it doesn't already exist) and setting File Location to 'Relative to Streaming Assets folder' "
					+ "as this is the easiest way to quickly get going. \n\n"
					+ "For more advanced functionality, check out the AVProVideo documentation.";
					EditorGUILayout.HelpBox(helpString, MessageType.Info);
					
					EditorGUILayout.PropertyField (_fileLocationProp);

					if (EditorGUI.EndChangeCheck ()) {
						_refreshPlayerValuesProp.boolValue = true;
					}
				
				}
				
				EditorGUI.BeginChangeCheck ();
				EditorGUILayout.DelayedTextField(_moviePathProp);
				if (EditorGUI.EndChangeCheck ()) {
					_refreshPlayerValuesProp.boolValue =true;
				}
			}
		}

		void OnInspectorGUI_PlayerMetaInfo()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_metaDataFileProp);
			EditorGUILayout.PropertyField (_posterProp);
			if (EditorGUI.EndChangeCheck ()) {
				_refreshMetadataProp.boolValue = true;
			}
		}

		void OnInspectorGUI_CheckForUndo()
		{
			if(_needToUndoRedo)
			{
				_refreshPlayerValuesProp.boolValue = true;
				_needToUndoRedo = false;
			}
		}
	}
}