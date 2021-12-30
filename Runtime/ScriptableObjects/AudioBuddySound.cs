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
        private bool _faulty;
        [SerializeField]
        public string FilePath;
        public AudioClip File
        {
            get
            {
                if (_file == null && !_faulty)
                {
#if UNITY_EDITOR
                    if (FilePath == "" && AudioBuddy.Importer.SoundsCreatedThroughImport.ContainsKey(this))
                    {
                        FilePath = AudioBuddy.Importer.SoundsCreatedThroughImport[this];
                    }
                    _file = AssetDatabase.LoadAssetAtPath<AudioClip>(FilePath);
                    //Debug.Log($"Recovered from Path {FilePath}");
                    if (_file == null)
                    {
                        AudioBuddy.Importer.RegisterFaultyABObject(this);
                        _faulty = true;
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
                    //Debug.Log($"Set from file to {FilePath}");
                    _faulty = false;
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

