using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioBuddyTool
{

    public class AudioBuddySpeaker : MonoBehaviour
    {
        //Sound references
        public AudioSource SourcePlayer;

        //Delegates
        /// <summary>
        /// Informs you when this speaker starts playing an AudioBuddySound.
        /// </summary>
        public Action<AudioBuddySound> OnPlayingNextSound;
        /// <summary>
        /// Informs you when this speaker finishes playing an AudioBuddySound. If you want to know when AudioBuddy decides on a random sound subsribe to OnRandomPick
        /// </summary>
        public Action<AudioBuddyObject, float> OnRandomPick;
        /// <summary>
        /// Returns you a speaker that is repurposed by the dynamic AudioBuddy management so that you can unsubscribe from any of its actions if you wish.
        /// </summary>
        public Action<AudioBuddySpeaker> OnSpeakerReassign;

        //Fields
        private float _externalVolumeMultipilier = 1f;
        private float _internalVolumeMultipilier = 1f;
        private bool _managedDynamically = true;
        
        //List playback
        [SerializeField]
        private bool _busyPlayingList; //is true while this player is playing back a list
        [SerializeField]
        private List<PlaybackEntry> _playbackList = new List<PlaybackEntry>(); //List which stores the order of the Objects that need to be played when playing a List
        [SerializeField]
        private float _timeTillNextSound = 0;
        [SerializeField]
        public bool UseScaledTime;
        public bool IsLooping;
        private bool _deleteNext = true;
        [Serializable]
        public class PlaybackEntry
        {
            public float Delay;
            public AudioBuddyObject SoundEntry;

            public PlaybackEntry(AudioBuddyObject buddyEntry, float delay)
            {
                SoundEntry = buddyEntry;
                Delay = delay;
            }
            public PlaybackEntry(BuddyListEntry listEntry)
            {
                SoundEntry = listEntry.BuddyEntry;
                Delay = listEntry.Delay;
            }
        }

        private void OnEnable()
        {
            OnPlayingNextSound += CheckForReaddLoopingSound;
        }

        private void Update()
        {
            if (_playbackList.Count > 0) //While sounds should be played
            {
                _timeTillNextSound -= UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
                if (_timeTillNextSound <= 0) //Should the next sound be played?
                {
                    _timeTillNextSound = 0;
                    PlaySound(_playbackList[0].SoundEntry);
                    if (_playbackList[0].SoundEntry.GetType() == typeof(AudioBuddySound) && _deleteNext)
                    {
                        AudioBuddySound upcomingSound = (AudioBuddySound)_playbackList[0].SoundEntry;
                        OnPlayingNextSound?.Invoke(upcomingSound);
                        _playbackList.RemoveAt(0);
                        _timeTillNextSound += _playbackList.Count > 0 ? _playbackList[0].Delay : 0;
                    }
                    if (_playbackList.Count == 0)
                    {
                        _timeTillNextSound = 0;
                        IsLooping = false;
                        _busyPlayingList = false;
                    }
                    _deleteNext = true;
                }
            }
            UpdateSourcePlayerVolume();
        }

        /// <summary>
        /// Gives the speaker a sound to play. If it is already playing sounds from a list it will just play them in order. This should not be called while the speaker is busy.
        /// </summary>
        /// <param name="soundObject"></param>
        /// <param name="volumeMultiplier"></param>
        public void PlaySound(AudioBuddyObject soundObject, float volumeMultiplier)
        {
            _externalVolumeMultipilier = volumeMultiplier * soundObject.Volume;
            switch (soundObject)
            {
                case AudioBuddySound sound:
                    PlaySimpleSound(sound);
                    break;
                case AudioBuddyList list:
                    AddListToPlaybackQueue(list);
                    break;
                case AudioBuddyRandom random:
                    AddRandomToPlaybackQueue(random);
                    break;
                default:
                    throw new ArgumentException($"{nameof(soundObject)} is not a valid AudioBuddyObject: {(soundObject == null ?  "Assigned sound is null" : soundObject.name)}");
                    //break;
            }
        }
        /// <summary>
        /// Gives the speaker a sound to play. If it is already playing sounds from a list it will just play them in order. This should not be called while the speaker is busy.
        /// </summary>
        /// <param name="soundObject"></param>
        public void PlaySound(AudioBuddyObject soundObject)
        {
            PlaySound(soundObject, 1f);
        }
        /// <summary>
        /// Gives the speaker a sound to play. If it is already playing sounds from a list it will just play them in order. This should not be called while the speaker is busy.
        /// </summary>
        /// <param name="name"></param>
        public void PlaySound(string name)
        {
            PlaySound(AudioBuddy.FindSoundByName(name), 1f);
        }

        private void PlaySimpleSound(AudioBuddySound sound)
        {
            _timeTillNextSound += sound.Duration;
            SourcePlayer.clip = sound.File;
            SourcePlayer.volume = sound.Volume * _externalVolumeMultipilier * _internalVolumeMultipilier;
            SourcePlayer.pitch = sound.Pitch;
            SourcePlayer.Play();
        }

        private void AddRandomToPlaybackQueue(AudioBuddyRandom random)
        {
            RemoveFirstIfInList(random);
            BuddyRandomEntry draft = random.GetRandomSound();
            _playbackList.Insert(0, new PlaybackEntry(draft.SoundObject, 0));
            OnRandomPick?.Invoke(draft.SoundObject, draft.Chance);
            _deleteNext = false;
        }

        private void AddListToPlaybackQueue(AudioBuddyList list)
        {
            _deleteNext = !_busyPlayingList;
            _busyPlayingList = true;
            RemoveFirstIfInList(list);
            for (int i = list.SoundList.Count - 1; i >= 0; i--)
            {
                _playbackList.Insert(0, new PlaybackEntry(list.SoundList[i]));
            }
            _timeTillNextSound += _playbackList[0].Delay;
            
        }
        private void RemoveFirstIfInList(AudioBuddyObject abobject)
        {
            if (_playbackList.Count > 0)
            {
                if (_playbackList[0].SoundEntry == abobject)
                {
                    _playbackList.RemoveAt(0);
                }
            }
        }
        private void CheckForReaddLoopingSound(AudioBuddySound lastSound)
        {
            if (lastSound.IsLoop)
            {
                IsLooping = true;
                _playbackList.Add(_playbackList[0]); //First entry is itself - so it clones
            }
        }
        /// <summary>
        public bool CheckAvailable()
        {
            return (SourcePlayer.isPlaying || _busyPlayingList) && (_managedDynamically);
        }

        private void UpdateSourcePlayerVolume()
        {
            SourcePlayer.volume = (SourcePlayer.clip == null) ? _internalVolumeMultipilier : _internalVolumeMultipilier * _externalVolumeMultipilier;
        }

        /// <summary>
        /// Stops any sounds played currently.
        /// </summary>
        public void StopSound()
        {
            //Should I OnSoundFinished.Invoke(_playbackList[0]); with a sanity check?
            _playbackList.Clear();
            SourcePlayer.Stop();
            _timeTillNextSound = 0;
            IsLooping = false;
            _busyPlayingList = false;
            _deleteNext = true;
            SourcePlayer.clip = null;
            StopAllCoroutines();
            _internalVolumeMultipilier = 1;
            _externalVolumeMultipilier = 1;
        }

        /// <summary>
        /// This will allow the Audio Buddy system to manage this speaker. After this has been activated it should not be deactivated anymore as this player might get used to play crucial audio elsewhere.
        /// </summary>
        public void EnableDynamicManagement()
        {
            _managedDynamically = true;
        }

        /// <summary>
        /// Will take this speaker object out of the dynamic management and allows the user full control over it without Audio Buddy interfering.
        /// </summary>
        public void DisableDynamicManagement()
        {
            _managedDynamically = false;
        }
        /// <summary>
        /// Stops all sounds and gives this speaker object back to the dynamic management of AudioBuddy's audio sources. Will call OnSpeakerReassign so you can choose to unsubsrice from actions of this speaker.
        /// </summary>
        public void ReassignSpeaker()
        {
            _managedDynamically = true;
            StopSound();
            OnSpeakerReassign?.Invoke(this);
        }

        //Fades
        /// <summary>
        /// Animates the internal volume. Use AudioBuddy.FadeIn() if you dont want to start the sound manually.
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="endVolume"></param>
        public void FadeIn(float Time, float endVolume)
        {
            StartCoroutine(InterpolateVolumeLinearly(Time,Mathf.RoundToInt(Time)*AudioBuddy.FadeStepsPerSecond,0f,endVolume,false));
        }
        /// <summary>
        /// Animates the internal volume. Use AudioBuddy.FadeIn() if you dont want to start the sound manually.
        /// </summary>
        /// <param name="Time"></param>
        public void FadeIn(float Time)
        {
            StartCoroutine(InterpolateVolumeLinearly(Time, Mathf.RoundToInt(Time) * AudioBuddy.FadeStepsPerSecond, 0f, 1f, false));
        }
        /// <summary>
        /// Animates the internal volume. Use AudioBuddy.FadeOut() if you dont want to start the sound manually.
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="startVolume"></param>
        public void FadeOut(float Time, float startVolume)
        {
            StartCoroutine(InterpolateVolumeLinearly(Time, Mathf.RoundToInt(Time) * AudioBuddy.FadeStepsPerSecond, startVolume, 0f, true));
        }
        /// <summary>
        /// Animates the internal volume. Use AudioBuddy.FadeOut() if you dont want to start the sound manually.
        /// </summary>
        /// <param name="Time"></param>
        public void FadeOut(float Time)
        {
            StartCoroutine(InterpolateVolumeLinearly(Time, Mathf.RoundToInt(Time) * AudioBuddy.FadeStepsPerSecond, 1f, 0f, true));
        }
        /// <summary>
        /// Animates the internal volume to fade from a one to another value. Does not start the playback of a sound.
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="startVolume"></param>
        /// <param name="endVolume"></param>
        public void FadeBetween(float Time, float startVolume, float endVolume)
        {
            StartCoroutine(InterpolateVolumeLinearly(Time, Mathf.RoundToInt(Time) * AudioBuddy.FadeStepsPerSecond, startVolume, endVolume, false));
        }
        /// <summary>
        /// Animates the internal volume to fade from the current volume to a target value. Does not start the playback of a sound.
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="endVolume"></param>
        public void FadeTo(float Time, float endVolume)
        {
            StartCoroutine(InterpolateVolumeLinearly(Time, Mathf.RoundToInt(Time) * AudioBuddy.FadeStepsPerSecond, _internalVolumeMultipilier, endVolume, false));
        }
        private IEnumerator InterpolateVolumeLinearly(float Time, int Steps, float startVolume, float endVolume, bool stopAfterFade)
        {
            _internalVolumeMultipilier = startVolume;
            for (int i = 1; i <= Steps; i++)
            {
                yield return new WaitForSeconds(Time/Steps);
                _internalVolumeMultipilier = startVolume + ((endVolume - startVolume) * (float)i/Steps);
            }
            if (stopAfterFade)
            {
                StopSound();
            }
        }
    }
}

