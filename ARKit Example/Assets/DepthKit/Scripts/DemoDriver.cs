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
using DepthKit;

public class DemoDriver : MonoBehaviour {

	public GameObject[] _models;
	protected int _activeModel = 0;

	void OnEnable()
	{
		for (int i = 0; i < _models.Length; i++)
		{
			if (i == 0)
			{
				_models[i].SetActive (true);
			}

			else
			{
				_models[i].SetActive (false);
			}
		}
	}
	
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			pauseMovie (_activeModel);
			if ((_activeModel+1) < _models.Length) {
				_activeModel++;
			} else {
				_activeModel = 0;
			}
			activateMovie (_activeModel);
		}
	}

	protected void pauseMovie(int index)
	{
		Clip[] clips = _models [_activeModel].GetComponentsInChildren<Clip> ();
		foreach (Clip clip in clips) {
			clip.Controller.Pause ();
		}
		_models [_activeModel].SetActive (false);
	}

	protected void activateMovie(int index)
	{
		Clip[] clips = _models [_activeModel].GetComponentsInChildren<Clip> ();
		_models [_activeModel].SetActive (true);
		foreach (Clip clip in clips) {
			clip.Controller.Play ();
		}
	}
}