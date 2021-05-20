using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AudioBuddyTool
{
    public static class AudioBuddy
    {
        public static AudioBuddySpeaker Play(string name, float volumeMultiplier, GameObject speaker)
        {
            return Manager.PlayAtLocation(FindSoundByName(name), volumeMultiplier, speaker.transform.position);
        }
        public static AudioBuddySpeaker Play(string name, GameObject speaker)
        {
            return Manager.PlayAtLocation(FindSoundByName(name), 1f, speaker.transform.position);
        }
        public static AudioBuddySpeaker Play(string name, float volumeMultiplier, Vector3 location)
        {
            return Manager.PlayAtLocation(FindSoundByName(name), volumeMultiplier, location);
        }
        public static AudioBuddySpeaker Play(string name, Vector3 location)
        {
            return Manager.PlayAtLocation(FindSoundByName(name), 1f, location);
        }

        public static AudioBuddySpeaker Play(string name, float volumeMultiplier)
        {
            return Manager.Play2D(FindSoundByName(name), volumeMultiplier);
        }
        public static AudioBuddySpeaker Play(string name)
        {
            return Manager.Play2D(FindSoundByName(name), 1f);
        }

        public static AudioBuddySpeaker Play(AudioBuddyObject abobject)
        {
            return Manager.Play2D(abobject, 1f);
        }
        public static AudioBuddySpeaker Play(AudioBuddyObject abobject, float volumeMultiplier)
        {
            return Manager.Play2D(abobject, volumeMultiplier);
        }

        public static AudioBuddySpeaker Play(AudioBuddyObject abobject, float volumeMultiplier, GameObject speakerPosition)
        {
            return Manager.PlayAtLocation(abobject, volumeMultiplier, speakerPosition.transform.position);
        }
        public static AudioBuddySpeaker Play(AudioBuddyObject abobject, GameObject speakerPosition)
        {
            return Manager.PlayAtLocation(abobject, 1f, speakerPosition.transform.position);
        }
        public static AudioBuddySpeaker Play(AudioBuddyObject abobject, float volumeMultiplier, Vector3 location)
        {
            return Manager.PlayAtLocation(abobject, volumeMultiplier, location);
        }
        public static AudioBuddySpeaker Play(AudioBuddyObject abobject, Vector3 location)
        {
            return Manager.PlayAtLocation(abobject, 1f, location);
        }

        public static AudioBuddySpeaker AttachSound(string name, GameObject speaker)
        {
            AudioSource source = speaker.AddComponent<AudioSource>();
            AudioBuddySpeaker player = speaker.AddComponent<AudioBuddySpeaker>();
            player.DisableDynamicManagement();
            player.SourcePlayer = source;
            source.spatialBlend = 1;
            player.PlaySound(FindSoundByName(name), 1f);
            return player;
        }
        public static AudioBuddySpeaker AttachSound(AudioBuddyObject abobject, GameObject speaker)
        {
            AudioSource source = speaker.AddComponent<AudioSource>();
            AudioBuddySpeaker player = speaker.AddComponent<AudioBuddySpeaker>();
            player.DisableDynamicManagement();
            source.spatialBlend = 1;
            player.PlaySound(abobject, 1f);
            return player;
        }
        public static AudioBuddySpeaker AttachSound(string name, float volumeMultiplier, GameObject speaker)
        {
            AudioSource source = speaker.AddComponent<AudioSource>();
            AudioBuddySpeaker player = speaker.AddComponent<AudioBuddySpeaker>();
            player.DisableDynamicManagement();
            player.SourcePlayer = source;
            source.spatialBlend = 1;
            player.PlaySound(FindSoundByName(name), volumeMultiplier);
            return player;
        }
        public static AudioBuddySpeaker AttachSound(AudioBuddyObject abobject, float volumeMultiplier, GameObject speaker)
        {
            AudioSource source = speaker.AddComponent<AudioSource>();
            AudioBuddySpeaker player = speaker.AddComponent<AudioBuddySpeaker>();
            player.DisableDynamicManagement();
            player.SourcePlayer = source;
            source.spatialBlend = 1;
            player.PlaySound(abobject, volumeMultiplier);
            return player;
        }

        public static AudioBuddyObject FindSoundByName(string name)
        {
            //TODO improve by caching library of all AudioBuddySound objects in import manager database
            foreach (string GUID in AssetDatabase.FindAssets("t:AudioBuddyObject"))
            {
                AudioBuddyObject sound = AssetDatabase.LoadAssetAtPath<AudioBuddyObject>(AssetDatabase.GUIDToAssetPath(GUID));
                if (sound.Name == name)
                {
                    return sound;
                }
            }
            throw new ArgumentOutOfRangeException(name, "No sound with this name could be found in the database");
        }

        //Resources
        private static AudioBuddyManager _manager;
        public static AudioBuddyManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("AudioBuddyManagerPrefab").First()))).GetComponent<AudioBuddyManager>();
                }
                return _manager;
            }
            set { _manager = value; } //TODO: Figure out why =value is nessecary, also ask about hideous expression in get
        }

        private static GUIStyle _subtleBG;
        public static GUIStyle SubtleBG
        {
            get
            {
                if (_subtleBG != null)
                {
                    return _subtleBG;
                }
                else
                {
                    GUIStyle subtle = new GUIStyle();
                    subtle.normal.background = Resources.Load<Texture2D>("AudioBuddy_GBG");
                    _subtleBG = subtle;
                    return _subtleBG;
                }
            }
        }

        private static GUIStyle _rightAligned;
        public static GUIStyle RightAligned
        {
            get
            {
                if (_rightAligned != null)
                {
                    return _rightAligned;
                }
                else
                {
                    GUIStyle right = new GUIStyle();
                    right.alignment = TextAnchor.MiddleRight;
                    right.normal.textColor = EditorStyles.label.normal.textColor;
                    right.padding = new RectOffset(0, 12, 0, 0);
                    _rightAligned = right;
                    return _rightAligned;
                }
            }
        }

        private static Texture _arrowUp;
        public static Texture ArrowUp
        {
            get
            {
                if (_arrowUp != null)
                {
                    return _arrowUp;
                }
                else
                {
                    _arrowUp = Resources.Load<Texture2D>("AudioBuddy_ArrowUp");
                    return _arrowUp;
                }
            }
        }

        private static Texture _arrowDown;
        public static Texture ArrowDown
        {
            get
            {
                if (_arrowDown != null)
                {
                    return _arrowDown;
                }
                else
                {
                    _arrowDown = Resources.Load<Texture2D>("AudioBuddy_ArrowDown");
                    return _arrowDown;
                }
            }
        }
    }
}

