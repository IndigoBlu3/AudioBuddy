using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioBuddyTool
{
    [CreateAssetMenu(fileName = "Sound Audio Buddy", menuName = "AudioBuddy/Simple Sound")]
    [Serializable]
    public class AudioBuddySound : AudioBuddyObject
    {
        public AudioClip File;
        public bool IsLoop;
        public bool CustomName;

        public override float GetDuration()
        {
            return File.length;
        }
    }
}

