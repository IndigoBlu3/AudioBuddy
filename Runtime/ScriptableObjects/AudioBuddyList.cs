using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static AudioBuddySpeaker;

namespace AudioBuddyTool
{
    [CreateAssetMenu(fileName = "Sound List AudioBuddy", menuName = "AudioBuddy/Sound List")]
    public class AudioBuddyList : AudioBuddyObject
    {
        public List<BuddyListEntry> SoundList
        {
            get
            {
                if (_soundList == null)
                {
                    _soundList = new List<BuddyListEntry>();
                }
                return _soundList;
            }
            set { _soundList = value; }
        }
        [SerializeField]
        private List<BuddyListEntry> _soundList;

        public override float GetDuration()
        {
            float sum = 0;
            foreach (BuddyListEntry entry in SoundList)
            {
                entry.Timestamp = sum + Mathf.Max(entry.Delay, 0);
                if (entry.BuddyEntry != null)
                {
                    sum += entry.BuddyEntry.GetDuration() + Mathf.Max(entry.Delay, 0);
                }
            }
            return sum;
        }

        public AudioBuddyObject GetObjectAt(int index)
        {

            if (index >= 0 && index < SoundList.Count)
            {
                return SoundList[index].BuddyEntry;
            }
            throw new ArgumentOutOfRangeException($"{this} does not have a buddy entry at {nameof(index)} {index}");
        }
    }

    [Serializable]
    public class BuddyListEntry
    {
        public string NameInList = "New List Entry";
        public float Delay; //TODO: Make sure only positive values are used
        public float Timestamp;
        public AudioBuddyObject BuddyEntry;
    }

}
