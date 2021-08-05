using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioBuddyTool
{
    //[CreateAssetMenu(fileName = "Audio Buddy Reference Manager", menuName = "AudioBuddy/ReferenceManager")]
    public class AudioBuddyReferenceManager : ScriptableObject
    {
        [HideInInspector]
        public GameObject ManagerPrefab;
        [HideInInspector]
        public GameObject SpeakerPrefab;
        //[HideInInspector]
        public AudioBuddyImportManager ImportManager;
    }
}

