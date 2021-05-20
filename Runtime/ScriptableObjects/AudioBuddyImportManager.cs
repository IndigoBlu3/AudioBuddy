﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AudioBuddyTool
{
    [CreateAssetMenu(fileName = "Audio Buddy Importer", menuName = "AudioBuddy/Importer")]
    public class AudioBuddyImportManager : ScriptableObject
    {
        public string CollectionAddress = "Paste the path to where you want AudioBuddy to build the database in here";
        public List<AudioBuddyObject> ABObjectCollection
        {
            get
            {
                if (_abObjectCollection == null)
                {
                    _abObjectCollection = new List<AudioBuddyObject>();
                }
                return _abObjectCollection;
            }
            set
            {
                _abObjectCollection = value;
            }
        }
        [SerializeField]
        private List<AudioBuddyObject> _abObjectCollection;

        public void RescanAudioBuddyObjects()
        {
            ABObjectCollection.Clear();
            foreach (string GUID in AssetDatabase.FindAssets("t:AudioBuddyObject"))
            {
                ABObjectCollection.Add(AssetDatabase.LoadAssetAtPath<AudioBuddyObject>(AssetDatabase.GUIDToAssetPath(GUID)));
            }
        }

        public List<AudioClip> FindAllClips()
        {
            List<AudioClip> allClips = new List<AudioClip>();
            foreach (string GUID in AssetDatabase.FindAssets("t:AudioClip"))
            {
                allClips.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(GUID)));
            }
            return allClips;
        }

        public List<AudioBuddyObject> FindAllABObjects()
        {
            List<AudioBuddyObject> allObjects = new List<AudioBuddyObject>();
            foreach (string GUID in AssetDatabase.FindAssets("t:AudioBuddyObject"))
            {
                allObjects.Add(AssetDatabase.LoadAssetAtPath<AudioBuddyObject>(AssetDatabase.GUIDToAssetPath(GUID)));
            }
            return allObjects;
        }

        public void CreateMissingAudioBuddyObjects()
        {
            List<AudioBuddySound> onlySounds = new List<AudioBuddySound>();
            foreach (AudioBuddyObject sound in ABObjectCollection)
            {
                if (sound.GetType() == typeof(AudioBuddySound))
                {
                    onlySounds.Add((AudioBuddySound)sound);
                }
            }
            int newObjectCounter = 0;
            foreach (AudioClip newClip in FindAllClips().Where(c => !onlySounds.Select(s => s.File).Contains(c)))
            {
                AudioBuddySound absound = CreateInstance<AudioBuddySound>();
                absound.File = newClip;
                ABObjectCollection.Add(absound);
                AssetDatabase.CreateAsset(absound, $"{CollectionAddress}/{newClip.name}.asset");
                newObjectCounter++;
            }
            DiscoverAudioBuddyObjects();
            Debug.Log($"Added {newObjectCounter} new Audio buddy Sounds to database at {CollectionAddress}");
        }

        public void DiscoverAudioBuddyObjects()
        {
            foreach (AudioBuddyObject foundObject in FindAllABObjects().Where(o => !ABObjectCollection.Contains(o)))
            {
                ABObjectCollection.Add(foundObject);
            }
        }

        public void RebuildAudioBuddyObjectCollection()
        {
            if (AssetDatabase.IsValidFolder(CollectionAddress))
            {
                Debug.LogWarning("Rebuilding Audio Buddy database. This might take a bit...");

                RescanAudioBuddyObjects();
                List<string> deleteFailed = new List<string>();
                AssetDatabase.DeleteAssets(ABObjectCollection.Select(o => AssetDatabase.GetAssetPath(o)).ToArray(), deleteFailed);
                ABObjectCollection.Clear();
                foreach (AudioClip clip in FindAllClips())
                {
                    AudioBuddySound absound = CreateInstance<AudioBuddySound>();
                    absound.File = clip;
                    ABObjectCollection.Add(absound);
                    AssetDatabase.CreateAsset(absound, $"{CollectionAddress}/{clip.name}.asset");
                }
                Debug.Log("Creating Instances done. Renaming assets...");
                AssetDatabase.SaveAssets();
                Debug.Log($"Done rebuilding Audio Buddy database into {CollectionAddress}");
                return;
            }
            Debug.LogError("Enter a valid path to build Audio Buddy database");
        }
    }

}