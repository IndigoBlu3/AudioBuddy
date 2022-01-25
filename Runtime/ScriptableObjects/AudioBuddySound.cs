using System;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AudioBuddyTool
{
    [CreateAssetMenu(fileName = "Sound AudioBuddy", menuName = "AudioBuddy/Simple Sound")]
    [Serializable]
    public class AudioBuddySound : AudioBuddyObject
    {
        public bool IsLoop;
        public bool CustomName;
        private bool _faulty;
        [SerializeField]
        public string FilePath;
        private AudioMixerGroup _mixerGroup;
        public AudioMixerGroup MixerGroupOverride 
        { 
            get 
            {
                return _mixerGroup;
            }
            set
            {
                _mixerGroup = value;
            }
        }
        public AnimationCurve Kurva
        {
            get
            {
                return _kurva;
            }
            set
            {
                _kurva = value;
            }
        }
        private AnimationCurve _kurva;
        [SerializeField]
        private AudioClip _file;
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

