#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tarodev.AutoSave
{
    [InitializeOnLoad]
    public static class AutoSave 
    {
        private const string FOLDER_NAME = "AutoSave";
        private static double _lastSaveTime;


        static AutoSave()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            try
            {   var config = AutoSaveScriptable.instance.Config;
            
                var timeSinceStartup = EditorApplication.timeSinceStartup;
                if(_lastSaveTime + config.Frequency * 60 > timeSinceStartup) return;
            
                if (!config.Enabled || Application.isPlaying || BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling) return;
                if (!UnityEditorInternal.InternalEditorUtility.isApplicationActive) return;
            
                var scene = SceneManager.GetActiveScene();
                if(string.IsNullOrEmpty(scene.path)) return;

                var scenePath = $"{scene.path[..scene.path.LastIndexOf("/", StringComparison.Ordinal)]}";

                if (!AssetDatabase.IsValidFolder($"{scenePath}/{FOLDER_NAME}")) AssetDatabase.CreateFolder(scenePath, FOLDER_NAME);
          
                scenePath += $"/{FOLDER_NAME}";

                var time = $"{DateTime.Now:MMM HH-mmtt}".Replace(".", "").Replace("AM","am").Replace("PM","pm");
                var path = $"{scenePath}/{scene.name} {time}.unity";
          
                EditorSceneManager.SaveScene(scene, path, true);
          
                if (config.Logging) Debug.Log($"Auto-saved at {time}");
            
                _lastSaveTime = timeSinceStartup;

            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
    }
}

#endif