using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioBuddyTool
{
    [Serializable]
    public class AudioBuddyObject : ScriptableObject
    {
        public float Duration
        {
            get
            {
                return GetDuration();
            }
        }
        public float Volume = 1;
        public float Pitch = 1;

        public virtual float GetDuration()
        {
            throw new NotImplementedException();
        }
    }
}

