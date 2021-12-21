using System;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace AudioBuddyTool
{
    [CreateAssetMenu(fileName = "Sound Audio Buddy", menuName = "AudioBuddy/Simple Sound")]
    [Serializable]
    public class AudioBuddySound : AudioBuddyObject
    {
        public AudioClip File
        {
            get
            {
                if (_file == null)
                {
#if UNITY_EDITOR
                    _file = AssetDatabase.LoadAssetAtPath<AudioClip>(FilePath);
                    if (_file == null)
                    {
                        AudioBuddy.Importer.RegisterFaultyABObject(this);
                    }
#endif
                }
                return _file;
            }
            set
            {
                _file = value;
                #if UNITY_EDITOR
                FilePath = AssetDatabase.GetAssetPath(File);
                #endif
            }
        }
        private AudioClip _file;
        public bool IsLoop;
        public bool CustomName;
        public string FilePath;

        public override float GetDuration()
        {
            return File.length;
        }
    }
}

