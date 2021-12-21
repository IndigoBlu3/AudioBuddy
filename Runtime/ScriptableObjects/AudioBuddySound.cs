using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AudioBuddyTool
{
    [CreateAssetMenu(fileName = "Sound Audio Buddy", menuName = "AudioBuddy/Simple Sound")]
    [Serializable]
    public class AudioBuddySound : AudioBuddyObject
    {

        [SerializeField]
        private AudioClip _file;
        public bool IsLoop;
        public bool CustomName;
        [SerializeField]
        public string FilePath;
        public AudioClip File
        {
            get
            {
                if (_file == null && FilePath != "")
                {
#if UNITY_EDITOR
                    _file = AssetDatabase.LoadAssetAtPath<AudioClip>(FilePath);
                    Debug.Log($"Recover from Path {FilePath}");
                    if (_file == null)
                    {
                        AudioBuddy.Importer.RegisterFaultyABObject(this);
                    }
                    //else
                    {
                        //Debug.Log($"Retrieved {File}");
                    }
#endif
                }
                return _file;
            }
            set
            {
                _file = value;
#if UNITY_EDITOR
                if (File != null)
                {
                    FilePath = AssetDatabase.GetAssetPath(File);
                    Debug.Log($"Set from file to {FilePath}");
                }
#endif
            }
        }


        public override float GetDuration()
        {
            return File.length;
        }
    }
}

