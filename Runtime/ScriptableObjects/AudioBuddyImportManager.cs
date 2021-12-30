using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AudioBuddyTool
{
    [CreateAssetMenu(fileName = "Audio Buddy Importer", menuName = "AudioBuddy/Importer")]
    public class AudioBuddyImportManager : ScriptableObject , ISerializationCallbackReceiver
    {

        public AudioBuddyReferenceManager ReferenceManager;
        public string CollectionAddress = "Paste the path to where you want AudioBuddy to build the database in here";
        public List<AudioBuddyObject> ABObjectCollection
        {
            get
            {
                if (_abObjectCollection == null)
                {
                    _abObjectCollection = new List<AudioBuddyObject>();
                    Debug.LogWarning("Created Database"); //BUG: Darf nicht jedes Mal, wenn die Engine gestartet wird neu created werden
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
        public List<AudioBuddyObject> FaultyABObjects
        {
            get
            {
                return _faultyABOjects ??= new List<AudioBuddyObject>();
            }
        }
        private List<AudioBuddyObject> _faultyABOjects = new List<AudioBuddyObject>();
        public Dictionary<AudioBuddySound, string> SoundsCreatedThroughImport = new Dictionary<AudioBuddySound, string>();

        /// <summary>
        /// Do not use this. The dictionary has not been fully implemented yet. Use ABObjectCollection instead
        /// </summary>
        [Obsolete("This feature is not yet fully implemented! Use ABObjectCollection instead.",true)]
        public Dictionary<string, AudioBuddyObject> ABDatabase
        {
            get
            {
                if(_abDatabase == null)
                {
                    _abDatabase = new Dictionary<string, AudioBuddyObject>();
                }
                return _abDatabase;
            }
            set
            {
                _abDatabase = value;
            }
        }

        [SerializeField]
        private Dictionary<string, AudioBuddyObject> _abDatabase;
        public bool Linked;
        public bool CreateABOjectsOnClipImport = true;

        public void OnEnable()
        {
            AudioBuddyReferenceManager ReferenceManager = Resources.Load<AudioBuddyReferenceManager>("AudioBuddyReferenceManager");
            ReferenceManager.ImportManager = this;
            #if UNITY_EDITOR
            DiscoverAudioBuddyObjects();
            #endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Clears the internal database and readds every AudioBuddyObject asset in the project. Only works in editor.
        /// </summary>
        public void RescanAudioBuddyObjects()
        {
            ABObjectCollection.Clear();
            //ABDatabase.Clear();
            foreach (string GUID in AssetDatabase.FindAssets("t:AudioBuddyObject"))
            {
                AudioBuddyObject soundObject = AssetDatabase.LoadAssetAtPath<AudioBuddyObject>(AssetDatabase.GUIDToAssetPath(GUID));
                ABObjectCollection.Add(soundObject);
                //ABDatabase[soundObject.Name] = soundObject;
            }
        }
        /// <summary>
        /// Returns a list of all AudioClip assets in the project. Only works in editor.
        /// </summary>
        /// <returns></returns>
        public List<AudioClip> FindAllClips()
        {
            List<AudioClip> allClips = new List<AudioClip>();
            foreach (string GUID in AssetDatabase.FindAssets("t:AudioClip"))
            {
                allClips.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(GUID)));
            }
            return allClips;
        }
        /// <summary>
        /// Returns a list of all AudioBuddyObject assets in the project. Only works in editor.
        /// </summary>
        /// <returns></returns>
        public List<AudioBuddyObject> FindAllABObjects()
        {
            List<AudioBuddyObject> allObjects = new List<AudioBuddyObject>();
            foreach (string GUID in AssetDatabase.FindAssets("t:AudioBuddyObject"))
            {
                allObjects.Add(AssetDatabase.LoadAssetAtPath<AudioBuddyObject>(AssetDatabase.GUIDToAssetPath(GUID)));
            }
            return allObjects;
        }
        /// <summary>
        /// Compares all AudioClip and AudioBuddySound assets in the project and creates AudioBuddySound assets for every AudioClip that is not part of the databse yet. Only works in editor.
        /// </summary>
        public void CreateMissingAudioBuddyObjects()
        {
            EnsureAssetDBPath();
            List<AudioBuddySound> onlySounds = new List<AudioBuddySound>();
            foreach (AudioBuddyObject sound in ABObjectCollection) //ABDatabase.Values)
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
                //ABDatabase[absound.Name] = absound;
                ABObjectCollection.Add(absound);
                AssetDatabase.CreateAsset(absound, $"{CollectionAddress}/{newClip.name}.asset");
                EditorUtility.SetDirty(absound);
                newObjectCounter++;
            }
            DiscoverAudioBuddyObjects();
            Debug.Log($"Added {newObjectCounter} new Audio buddy Sounds to database at {CollectionAddress}");
        }
        /// <summary>
        /// Allows for the manual creation of an AudioBuddySound asset based on an AudioClip. Only works in editor.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public AudioBuddySound CreateAudioBuddySoundFromClip(AudioClip clip)
        {
            AudioBuddySound absound = CreateInstance<AudioBuddySound>();
            absound.File = clip;
            ABObjectCollection.Add(absound);
            AssetDatabase.CreateAsset(absound, $"{CollectionAddress}/{clip.name}.asset");
            return absound;
        }
        /// <summary>
        /// Scans the project for AudioBuddyObject assets that are not in the database and adds them. Will throw a warning if objects with duplicate names are found. Only works in editor.
        /// </summary>
        public void DiscoverAudioBuddyObjects()
        {
            foreach (AudioBuddyObject foundObject in FindAllABObjects().Where(o => !ABObjectCollection.Contains(o)))
            {
                if (ABObjectCollection.Select(o => o.name).Contains(foundObject.name)) //ABDatabase.ContainsKey(foundObject.name))
                {
                    Debug.LogWarning($"The name of the sound {foundObject.name} is already in use! Please double check the naming of {foundObject}");
                }
                else
                {
                    ABObjectCollection.Add(foundObject);
                    EditorUtility.SetDirty(foundObject);
                    //ABDatabase[foundObject.Name] = foundObject;
                }
            }
            EnsureAssetDBPath();
        }
        /// <summary>
        /// Permanently deletes all AudioBuddyObjects and rebuilds AudioBuddySound objects from all available AudioClip assets in the project. This means that all settings on individual objects and randomizers and lists will be gone. Use with caution. Only works in editor.
        /// </summary>
        public void RebuildAudioBuddyObjectCollection()
        {
            if (AssetDatabase.IsValidFolder(CollectionAddress))
            {
                Debug.LogWarning("Rebuilding Audio Buddy database. This might take a bit...");

                RescanAudioBuddyObjects();
                DeletePartOfDatabase(ABObjectCollection);
                //ABDatabase.Clear();
                foreach (AudioClip clip in FindAllClips())
                {
                    AudioBuddySound absound = CreateInstance<AudioBuddySound>();
                    absound.File = clip;
                    //ABDatabase[absound.File.name] = absound;
                    ABObjectCollection.Add(absound);
                    AssetDatabase.CreateAsset(absound, $"{CollectionAddress}/{clip.name}.asset");
                    EditorUtility.SetDirty(absound);
                }
                Debug.Log("Creating Instances done. Saving assets...");
                AssetDatabase.SaveAssets();
                Debug.Log($"Done rebuilding Audio Buddy database into {CollectionAddress}");
                return;
            }
            Debug.LogError("Enter a valid path to build Audio Buddy database");
        }

        private void EnsureAssetDBPath()
        {
            if (!AssetDatabase.IsValidFolder(CollectionAddress))
            {
                if (ABObjectCollection.Count >= 1)
                {
                    CollectionAddress = TrimAssetPath(AssetDatabase.GetAssetPath(ABObjectCollection[0]));
                    Debug.LogWarning($"Automatically set {CollectionAddress} as database adress from existing Audio Buddy Object");
                }
                else
                {
                    Debug.Log("Ensuring the database");
                    string firstABObject = AssetDatabase.FindAssets("t:AudioBuddyObject").FirstOrDefault();
                    if (firstABObject != "")
                    {
                        CollectionAddress = TrimAssetPath(AssetDatabase.GUIDToAssetPath(firstABObject));
                    }
                    else
                    {
                        throw new ArgumentException("Please enter a valid database path and rebuild sound objects.");
                    }
                }
            }
        }
        /// <summary>
        /// Marks an AudioBuddyObject as potentially faulty to the importer to set it up for deletion by the user. Only works in editor.
        /// </summary>
        /// <param name="abobject"></param>
        public void RegisterFaultyABObject(AudioBuddyObject abobject)
        {
            if (!_faultyABOjects.Contains(abobject))
            {
                _faultyABOjects.Add(abobject);
            }
        }
        /// <summary>
        /// Deletes all AudioBuddyObject assets that were marked as faulty permanently. Only works in editor.
        /// </summary>
        public void DeleteFaultyABObjects()
        {
            DeletePartOfDatabase(FaultyABObjects);
            Debug.LogWarning("Deleted AudioBuddyObjects marked as faulty");
        }
        /// <summary>
        /// Deletes all AudioBuddyObject assets that are fed into the method as a list permanently. Use with caution. Only works in editor.
        /// </summary>
        /// <param name="toBeDeleted"></param>
        private void DeletePartOfDatabase(List<AudioBuddyObject> toBeDeleted)
        {
            List<string> deleteFailed = new List<string>();
            AssetDatabase.DeleteAssets(toBeDeleted.Select(o => AssetDatabase.GetAssetPath(o)).ToArray(), deleteFailed);
            if (deleteFailed.Count > 0)
            {
                Debug.LogWarning($"Deleting of {deleteFailed.Count} AudioBuddy objects failed! Failed assets are:");
                foreach (string remainingSound in deleteFailed)
                {
                    Debug.LogWarning(remainingSound);
                }
            }
            toBeDeleted.Clear();
        }
        #endif
        /// <summary>
        /// Returns the part of an asset path that contains only the adress of the asset folder
        /// </summary>
        /// <param name="origPath"></param>
        /// <returns></returns>
        protected string TrimAssetPath(string origPath)
        {
            bool backfound = false;
            while (!backfound && origPath.Contains("/"))
            {
                backfound = origPath.EndsWith("/");
                origPath = origPath.Substring(0, origPath.Length - 1);
            }
            return origPath;
        }

        public void OnBeforeSerialize()
        {
            //
            //throw new NotImplementedException();
        }

        public void OnAfterDeserialize()
        {
            //throw new NotImplementedException();
        }
    }

}
