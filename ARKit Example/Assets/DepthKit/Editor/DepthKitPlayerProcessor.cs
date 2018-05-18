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
using System.Collections.Generic;
using System;
using System.Reflection;
namespace DepthKit
{
    public class DepthKitPlayerProcesser : AssetPostprocessor
        {

            static void AddPlayerDefine(string target)
            {

                //set the target across all supported platforms
                for (int pIndex = 0; pIndex < DepthKitPlugin.SupportedPlatforms.Length; pIndex++)
                {
                    //get the exisiting defines
                    string existingDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(DepthKitPlugin.SupportedPlatforms[pIndex]);
                    int defineIndex;
                    List<string> defineList;
                    if(!DefineExistsInPlatformDefines(existingDefines, target, out defineList, out defineIndex))
                    {
                        //add the new define
                        defineList.Add(target);

                        //combine the strings back into the proper define style
                        string newDefines = string.Join(";", defineList.ToArray());

                        //add the defines
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(DepthKitPlugin.SupportedPlatforms[pIndex], newDefines);
                    }
                }
            }

            static void RemovePlayerDefine(string target)
            {
                //remove the target across all supported platforms
                for (int pIndex = 0; pIndex < DepthKitPlugin.SupportedPlatforms.Length; pIndex++)
                {
                    //get the exisiting defines
                    string existingDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(DepthKitPlugin.SupportedPlatforms[pIndex]);
                    int defineIndex;
                    List<string> defineList;
                    if(DefineExistsInPlatformDefines(existingDefines, target, out defineList, out defineIndex))
                    {
                        defineList.RemoveAt(defineIndex);

                        //combine the strings back into the proper define style
                        string newDefines = string.Join(";", defineList.ToArray());
                        
                        //add the defines
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(DepthKitPlugin.SupportedPlatforms[pIndex], newDefines);
                    }
                }
            }

            static bool DefineExistsInPlatformDefines(string platformDefines, string targetDefine, out List<string> defineList, out int index)
            {
                //assign index a bum value
                index = 0;

                //split the platform defines
                string[] defines = platformDefines.Split(';');

                //make the new define list
                defineList = new List<string>(defines);

                //check if the define exists
                for (int i = defineList.Count-1; i >= 0; i--)
                {
                    if(defines[i].Contains(targetDefine))
                    {
                        index = i;
                        return true;
                    }
                }

                return false;
            }

            public static List<PlayerType> GetSupportedPlayersInAssembly()
            {
                string existingDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                string[] defines = existingDefines.Split(';');
                List<PlayerType> supportedPlayers = new List<PlayerType>();

                //find out if this has already been definied
                for (int i = 0; i < defines.Length; i++ )
                {
                    if(defines[i].Contains("DK_"))
                    {
                        supportedPlayers.Add(DepthKitPlugin.DirectiveDict[defines[i]]);
                    }
                }

                return supportedPlayers;
            }

            [InitializeOnLoadMethod]
            static void OnUnityReloaded()
            {
                bool mpmpChanged = false;
                if (Assembly.Load("Assembly-CSharp").GetType("monoflow.MPMP") != null)
                {
                    AddPlayerDefine("DK_USING_MPMP");
                    mpmpChanged = true;
                }
                else 
                {
                    RemovePlayerDefine("DK_USING_MPMP");
                    mpmpChanged = true;
                }
                //////////////////
                bool avproChanged = false;
                if (Assembly.Load("Assembly-CSharp").GetType("RenderHeads.Media.AVProVideo.MediaPlayer") != null) 
                {
                    AddPlayerDefine("DK_USING_AVPRO");
                    avproChanged = true;
                }
                else 
                {
                    RemovePlayerDefine("DK_USING_AVPRO");
                    avproChanged = true;                
                }

                if (avproChanged) {ResetClipsAffectedByDefineChange(PlayerType.AVProVideo);}
                if (mpmpChanged) {ResetClipsAffectedByDefineChange(PlayerType.MPMP);}
            }

            static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
            {       
                bool _mpmpPinged;
                bool _avproPinged;
                _mpmpPinged = _avproPinged = false;


                foreach (string str in deletedAssets) 
                {
                    if (!_mpmpPinged && str.Contains("MPMP.cs"))
                    {
                        ResetClipsAffectedByDefineChange(PlayerType.MPMP);
                        RemovePlayerDefine("DK_USING_MPMP");
                        _mpmpPinged = true;
                    }

                    if (!_avproPinged && str.Contains("MediaPlayer.cs"))
                    {
                        ResetClipsAffectedByDefineChange(PlayerType.AVProVideo);                    
                        RemovePlayerDefine("DK_USING_AVPRO");
                        _avproPinged = true;
                    }
                }

                if (_avproPinged) {ResetClipsAffectedByDefineChange(PlayerType.AVProVideo);}
                if (_mpmpPinged) {ResetClipsAffectedByDefineChange(PlayerType.MPMP);}
            }
            

            static void ResetClipsAffectedByDefineChange(PlayerType targetType)
            {
                Clip[] clips = Resources.FindObjectsOfTypeAll<Clip>();
                for (int i = 0; i < clips.Length; i++)
                {
                    //if this clip matches the type being updated by the directive shift
                    if(clips[i]._playerType == (AvailablePlayerType)targetType)
                    {
                        clips[i]._playerType = Clip._defaultPlayerType;
                        clips[i]._needToResetPlayerType = true;
                        clips[i].ResetPlayer();
                    }
                }
            }
        }
    }

