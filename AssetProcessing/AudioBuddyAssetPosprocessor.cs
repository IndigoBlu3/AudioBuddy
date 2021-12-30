using AudioBuddyTool;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AudioBuddyTool
{
    public class AudioBuddyAssetPosprocessor : AssetPostprocessor
    {
        AudioBuddyImportManager abImportManager;
        void OnPostprocessAudio(AudioClip clip) //Is called when assets are imported but also when they are reimported
        {
            abImportManager ??= AudioBuddy.Importer;
            if (abImportManager.CreateABOjectsOnClipImport && AssetDatabase.IsValidFolder(abImportManager.CollectionAddress))
            {
                string clipName = ExtractAssetNameFromPath(assetPath);
                AudioBuddySound absound = ScriptableObject.CreateInstance<AudioBuddySound>();
                AssetDatabase.CreateAsset(absound, $"{abImportManager.CollectionAddress}/{clipName}.asset");
                abImportManager.ABObjectCollection.Add(absound);
                absound.FilePath = assetPath; // This for some reason gets reset to default values at some stage
                abImportManager.SoundsCreatedThroughImport.Add(absound,assetPath);
                Debug.Log($"Automatically created AudioBuddySound for {clipName} at {absound.FilePath}");
            }
        }

        private string ExtractAssetNameFromPath(string path)
        {
            //Debug.Log(path);
            int dotIndex = path.Length - 1;
            while (path[dotIndex] != '.')
            {
                dotIndex--;
            }
            int lastBackIndex = path.Length - 1;
            while (path[lastBackIndex] != '/')
            {
                lastBackIndex--;
            }
            return path.Substring(lastBackIndex + 1, dotIndex - lastBackIndex - 1);
        }
    }
}

