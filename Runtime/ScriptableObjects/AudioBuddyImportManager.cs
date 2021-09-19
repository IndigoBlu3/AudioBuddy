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

        public void OnEnable()
        {
            AudioBuddyReferenceManager ReferenceManager = Resources.Load<AudioBuddyReferenceManager>("AudioBuddyReferenceManager");
            ReferenceManager.ImportManager = this;
            DiscoverAudioBuddyObjects();
        }


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

        #if UNITY_EDITOR
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
                newObjectCounter++;
            }
            DiscoverAudioBuddyObjects();
            Debug.Log($"Added {newObjectCounter} new Audio buddy Sounds to database at {CollectionAddress}");
        }

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

        public void RebuildAudioBuddyObjectCollection()
        {
            if (AssetDatabase.IsValidFolder(CollectionAddress))
            {
                Debug.LogWarning("Rebuilding Audio Buddy database. This might take a bit...");

                RescanAudioBuddyObjects();
                List<string> deleteFailed = new List<string>();
                //AssetDatabase.DeleteAssets(ABDatabase.Values.Select(o => AssetDatabase.GetAssetPath(o)).ToArray(), deleteFailed);
                AssetDatabase.DeleteAssets(ABObjectCollection.Select(o => AssetDatabase.GetAssetPath(o)).ToArray(), deleteFailed);
                if (deleteFailed.Count > 0)
                {
                    Debug.LogWarning($"Deleting of {deleteFailed.Count} AudioBuddy objects failed! Failed assets are:");
                    foreach (string remainingSound in deleteFailed)
                    {
                        Debug.LogWarning(remainingSound);
                    }
                }
                ABObjectCollection.Clear();
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
                Debug.Log("Creating Instances done. Renaming assets...");
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
                    Debug.Log("Ensuring is Expensive");
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
        #endif
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
