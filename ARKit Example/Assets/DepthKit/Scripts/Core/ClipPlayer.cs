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
    /// <summary>
    /// The base class that any DepthKit player implementation will derrive from </summary>
    /// <remarks>
    /// This class provides methods that are implemented in child classes to allow
    /// a way for clip to genericly interact with a given player backend. </remarks>
    [System.Serializable]
    public class ClipPlayer : MonoBehaviour
    {
        public bool VideoLoaded { get; protected set;}
        [SerializeField, HideInInspector]
        private PlayerEvents events = new PlayerEvents();
        public PlayerEvents Events 
        {
            get { return events; }
            private set { events = value; }
        }
        public void AssignEvents(PlayerEvents events) 
        {
            Events = events;
        }
    }
}