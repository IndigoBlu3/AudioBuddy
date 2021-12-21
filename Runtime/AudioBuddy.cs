using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AudioBuddyTool
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class AudioBuddy
    {
        /// <summary>
        /// Determines how many steps there will be in a second when AudioBuddy fades sound volumes
        /// </summary>
        public static int FadeStepsPerSecond = 30;

        static AudioBuddy()
        {
            RelinkImporter();
        }

        //3D Sounds
        public static AudioBuddySpeaker Play(string name, float volumeMultiplier, GameObject speakerPosition)
        {
            return Manager.PlayAtLocation(FindSoundByName(name), volumeMultiplier, speakerPosition.transform.position);
        }
        public static AudioBuddySpeaker Play(string name, GameObject speakerPosition)
        {
            return Manager.PlayAtLocation(FindSoundByName(name), 1f, speakerPosition.transform.position);
        }
        public static AudioBuddySpeaker Play(string name, float volumeMultiplier, Vector3 location)
        {
            return Manager.PlayAtLocation(FindSoundByName(name), volumeMultiplier, location);
        }
        public static AudioBuddySpeaker Play(string name, Vector3 location)
        {
            return Manager.PlayAtLocation(FindSoundByName(name), 1f, location);
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
        //2D Sounds
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
        //Fades
        public static AudioBuddySpeaker FadeIn(string name, float Time, float targetVolume = 1f)
        {
            AudioBuddySpeaker speaker = Play(name);
            speaker.FadeIn(Time,targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeIn(AudioBuddyObject abobject, float Time, float targetVolume = 1f)
        {
            AudioBuddySpeaker speaker = Play(abobject);
            speaker.FadeIn(Time, targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeIn(string name, GameObject speakerPosition, float Time, float targetVolume = 1f)
        {
            AudioBuddySpeaker speaker = Play(name, speakerPosition);
            speaker.FadeIn(Time, targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeIn(AudioBuddyObject abobject, GameObject speakerPosition, float Time, float targetVolume = 1f)
        {
            AudioBuddySpeaker speaker = Play(abobject, speakerPosition);
            speaker.FadeIn(Time, targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeIn(string name, Vector3 location, float Time, float targetVolume = 1f)
        {
            AudioBuddySpeaker speaker = Play(name, location);
            speaker.FadeIn(Time, targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeIn(AudioBuddyObject abobject, Vector3 location, float Time, float targetVolume = 1f)
        {
            AudioBuddySpeaker speaker = Play(abobject, location);
            speaker.FadeIn(Time, targetVolume);
            return speaker;
        }

        public static AudioBuddySpeaker FadeOut(string name, float Time, float targetVolume = 0f)
        {
            AudioBuddySpeaker speaker = Play(name);
            speaker.FadeOut(Time, targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeOut(AudioBuddyObject abobject, float Time, float targetVolume = 0f)
        {
            AudioBuddySpeaker speaker = Play(abobject);
            speaker.FadeOut(Time, targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeOut(string name, GameObject speakerPosition, float Time, float targetVolume = 0f)
        {
            AudioBuddySpeaker speaker = Play(name, speakerPosition);
            speaker.FadeOut(Time, targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeOut(AudioBuddyObject abobject, GameObject speakerPosition, float Time, float targetVolume = 0f)
        {
            AudioBuddySpeaker speaker = Play(abobject, speakerPosition);
            speaker.FadeOut(Time, targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeOut(string name, Vector3 location, float Time, float targetVolume = 0f)
        {
            AudioBuddySpeaker speaker = Play(name, location);
            speaker.FadeOut(Time, targetVolume);
            return speaker;
        }
        public static AudioBuddySpeaker FadeOut(AudioBuddyObject abobject, Vector3 location, float Time, float targetVolume = 0f)
        {
            AudioBuddySpeaker speaker = Play(abobject, location);
            speaker.FadeOut(Time, targetVolume);
            return speaker;
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
            AudioBuddyObject abo = Importer.ABObjectCollection.Where(sound => sound.name == name).FirstOrDefault();
            if (abo != null)
            {
                return abo;
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
                    _manager = GameObject.Instantiate(Referencer.ManagerPrefab).GetComponent<AudioBuddyManager>();
                }
                return _manager;
            }
        }

        private static AudioBuddyImportManager _importer;
        public static AudioBuddyImportManager Importer
        {
            get
            {
                if (_importer == null)
                {
                    _importer = Referencer.ImportManager;
                }
#if UNITY_EDITOR
                string firstGUID = AssetDatabase.FindAssets("t:AudioBuddyImportManager").FirstOrDefault();
                if (firstGUID != "")
                {
                    _importer = AssetDatabase.LoadAssetAtPath<AudioBuddyImportManager>(AssetDatabase.GUIDToAssetPath(firstGUID));
                }
                if (_importer == null)
                {
                    _importer = ScriptableObject.CreateInstance<AudioBuddyImportManager>();
                    AssetDatabase.CreateAsset(_importer,"Assets/AudioBuddyImportManager.asset");
                    Debug.LogWarning($"Automatically created an Audio Buddy Importer {_importer} at {AssetDatabase.GetAssetPath(_importer)}");
                }
                _importer.Linked = true;
#endif
                return _importer;
            }
        } 
        
                private static AudioBuddyReferenceManager _referencer;
                public static AudioBuddyReferenceManager Referencer
                {
                    get
                    {
                        if (_referencer == null)
                        {
                            _referencer = Resources.Load<AudioBuddyReferenceManager>("AudioBuddyReferenceManager");
                        }
                        return _referencer;
                    }
                }
                       
        public static void RelinkImporter()
        {
            if (Importer == null)
            {
                return;
            }
            Importer.Linked = false;
            Debug.Log($"Relinked AudioBuddy with Import Manager {Importer}");
        }


#if UNITY_EDITOR
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
#endif
    }
}

