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
    public class DepthKitMenu
    {
        [MenuItem("DepthKit/Create DepthKit Clip")]
        static void Create()
        {
            EditorApplication.ExecuteMenuItem("GameObject/DepthKit/DepthKit Clip");
        }

        [MenuItem("GameObject/DepthKit/DepthKit Clip", false, 0)]
        static void CreateDepthKitClip(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("DepthKit Clip");
            go.AddComponent<Clip>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}
