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
	[ExecuteInEditMode]
	/// <summary>
	/// The base class that any DepthKit Renderer implementation will derrive from 
	/// </summary>
	/// <remarks>
	/// This class provides methods that are implemented in child classes to allow
	/// a way for clip to be rendered in different ways
	/// </remarks>
	public abstract class ClipRenderer : MonoBehaviour
	{
		public enum MeshDensity {
			High,
			Medium,
			Low
		};

		protected bool _geometryDirty;
		protected bool _materialDirty;

		/// <summary>
		/// Texture that represents the current frame
		/// <summary>
		protected Texture _texture;
		// use the public getter/setter only when we need to mark the mesh dirty
		public Texture Texture {
			get { return _texture; }
			set { 
				_texture = value; 
			}
		}

		/// <summary>
		/// Set to true if the Texture is flipped from what default unity Textures would expect
		/// <summary>
		protected bool _textureIsFlipped;
		// use the public getter/setter only when we need to mark the mesh dirty
		public bool TextureIsFlipped {
			get { return _textureIsFlipped; }
			set { 
				_materialDirty = (_textureIsFlipped != value);
				_textureIsFlipped = value; 
			}
		}

		/// <summary>
		/// Texture placeholder for edit mode
		/// <summary>
		[SerializeField, HideInInspector]
		protected Texture2D _poster;
		// use the public getter/setter only when we need to mark the mesh dirty
		public Texture2D Poster {
			get { return _poster; }
			set { 
				_poster = value; 
			}
		}

		/// <summary>
		/// Metadata contains information about the current clip
		/// <summary>
		[SerializeField, HideInInspector]
		protected Metadata _metadata;
		// use the public getter/setter only when we need to mark the mesh dirty
		public Metadata Metadata {
			get { return _metadata; }
			set { 
				_geometryDirty = true;
				_metadata = value;
			}
		}

		public void SetGeometryDirty(){
			_geometryDirty = true;
		}

		public void SetMaterialDirty(){
			_materialDirty = true;
		}

		/// <summary>
		/// Render type returns the appropriate enum for each subclass implementation
		/// <summary>
		public abstract RenderType GetRenderType();
	}
}
