using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioBuddyTool
{
    public class AudioBuddySpeakerSetting : MonoBehaviour
    {
        public bool PreLoad = false;
        private AudioSource _audioSettings;
        [HideInInspector]
        public AudioSource AudioSettings
        {
            get
            {
                EnsureAudioSettings();
                return _audioSettings;
            }
            set
            {
                _audioSettings = value;
            }
        }

        private void EnsureAudioSettings()
        {
            if (_audioSettings == null)
            {
                AudioSource foundSettings;
                if (gameObject.TryGetComponent(out foundSettings))
                {
                    _audioSettings = foundSettings;
                }
                else
                {
                    _audioSettings = gameObject.AddComponent<AudioSource>();
                }
            }
        }

        public void OnEnable()
        {
            EnsureAudioSettings();
        }
    }
}

