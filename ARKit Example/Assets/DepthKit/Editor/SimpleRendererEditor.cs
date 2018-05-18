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
using UnityEditor;

namespace DepthKit {
	
	[CustomEditor(typeof(SimpleRenderer))]
	[CanEditMultipleObjects]
	public class SimpleRendererEditor : Editor
	{

		SerializedProperty _meshDensityProp;
		SerializedProperty _shaderProp;

		bool needToUndoRedo;
		void OnEnable () {
			// subscribe to the undo event
			Undo.undoRedoPerformed += OnUndoRedo; 
			needToUndoRedo = false;

			//set the property types
			_meshDensityProp    = serializedObject.FindProperty ("_meshDensity");
			_shaderProp     	= serializedObject.FindProperty ("_shader");

		}

		void OnUndoRedo() {
			needToUndoRedo = true;
 		}

	    public override void OnInspectorGUI()
	    {
			serializedObject.Update ();

			SimpleRenderer renderer = (SimpleRenderer)target;			

			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (_meshDensityProp);
			if (EditorGUI.EndChangeCheck ()) {
				renderer.SetGeometryDirty ();
			}

			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (_shaderProp);
			if (EditorGUI.EndChangeCheck ()) {
				renderer.SetMaterialDirty ();
			}

			if (needToUndoRedo) {
				renderer.SetMaterialDirty ();
				renderer.SetGeometryDirty ();
				needToUndoRedo = false;
			}

			//APPLY PROPERTY MODIFICATIONS
			serializedObject.ApplyModifiedProperties();
	    }
	}
}