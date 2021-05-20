using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioBuddyTool
{
    [CreateAssetMenu(fileName = "Random Sound Audio Buddy", menuName = "AudioBuddy/Sound Randomizer")]
    public class AudioBuddyRandom : AudioBuddyObject
    {
        public List<BuddyRandomEntry> RandomList
        {
            get
            {
                if (_randomList == null)
                {
                    _randomList = new List<BuddyRandomEntry>();
                }
                return _randomList;
            }
            set
            {
                _randomList = value;
                RecalculateChances();
            }
        }
        [SerializeField]
        private List<BuddyRandomEntry> _randomList;

        public float TotalWeight;

        public BuddyRandomEntry GetRandomSound()
        {
            float sum = 0;
            float roll = UnityEngine.Random.Range(0, TotalWeight);
            for (int i = 0; i < _randomList.Count; i++)
            {
                if (roll <= sum + _randomList[i].Weight)
                {
                    return _randomList[i];
                }
                sum += _randomList[i].Weight;
            }
            return _randomList[_randomList.Count - 1];
        }

        public void RecalculateChances(bool considerLocked)
        {
            GetTotalWeight();
            if (!_randomList.Any(e => !e.LockChance))
            {
                Debug.LogWarning($"No unlocked chance could be found in {Name} so {_randomList[0].Name} had to be unlocked");
                _randomList[0].LockChance = false;
                RecalculateChances();
                return;
            }
            foreach (BuddyRandomEntry entry in _randomList)
            {
                if (entry.LockChance)
                {
                    entry.Weight = TotalWeight * entry.Chance;
                    if (considerLocked)
                    {
                        RecalculateChances(false);
                        return;
                    }
                }
                else
                {
                    entry.Chance = entry.Weight / TotalWeight;
                }
            }
        }

        public void RecalculateChances()
        {
            if (RandomList.Count > 0)
            {
                RecalculateChances(true);
            }
        }

        public float GetTotalWeight()
        {
            TotalWeight = 0;
            foreach (BuddyRandomEntry entry in RandomList)
            {
                TotalWeight += entry.Weight;
            }
            return TotalWeight;
        }

        public float GetLeftoverChance(BuddyRandomEntry targetEntry)
        {
            float sum = 0;
            foreach (BuddyRandomEntry entry in RandomList)
            {
                if (entry != targetEntry && entry.LockChance)
                {
                    sum += entry.Chance;
                }
            }
            return 0.99999f - sum;
        }

        public override float GetDuration()
        {
            float sum = 0;
            foreach (BuddyRandomEntry entry in RandomList)
            {
                sum += entry.SoundObject.GetDuration();
            }
            return sum / RandomList.Count;
        }
    }

    [Serializable]
    public class BuddyRandomEntry
    {
        public AudioBuddyObject SoundObject;
        public string Name = "New Random Entry";
        public float Weight = 1;
        public float Chance;
        public bool LockChance;
    }

}
