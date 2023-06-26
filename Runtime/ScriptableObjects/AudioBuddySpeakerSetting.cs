using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioBuddyTool
{
    public class AudioBuddySpeakerSetting : MonoBehaviour
    {
        public bool PreLoad = false;
        public bool hasSettings
        {
            get
            {
                return AudioSettings == null;
            }
        }
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
        private AnimationCurve _customRollofCurve;
        public AnimationCurve CustomRollofCurve
        {
            get
            {
                if (_customRollofCurve == null)
                {
                    _customRollofCurve = new AnimationCurve();
                }
                return _customRollofCurve;
            }
            set
            {
                _customRollofCurve = value;
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

        public void Awake()
        {
            AudioSettings.SetCustomCurve(AudioSourceCurveType.Spread ,AnimationCurve.Linear(0,0,0,0));
        }

        public void OnEnable()
        {
            EnsureAudioSettings();
        }
    }
}

