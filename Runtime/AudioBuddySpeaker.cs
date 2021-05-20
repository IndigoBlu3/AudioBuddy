using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AudioBuddyTool
{
    public class AudioBuddySpeaker : MonoBehaviour
    {
        public AudioBuddyObject SourceSound;
        public AudioSource SourcePlayer;

        public delegate void SoundListEvent(int index, string name);
        public static SoundListEvent NextListSound;
        public static SoundListEvent ListComplete;

        [SerializeField]
        private bool _busy;
        [SerializeField]
        private float _listDelay;
        [SerializeField]
        private bool _waitOver;
        public bool UseScaledTime;
        private float _externalVolumeMultipilier;
        [SerializeField]
        private List<AudioBuddyList> _playStack = new List<AudioBuddyList>();
        [SerializeField]
        private List<int> _positionStack = new List<int>();
        private bool _managedDynamically = true;

        private void Update() //TODO: Turn this into single coroutine
        {
            //List playback
            if (_busy && !SourcePlayer.isPlaying)
            {
                _listDelay -= UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
                if (_listDelay <= 0 && _waitOver)
                {
                    if (_playStack.Count > 0)
                    {
                        //Find out whether to move to next sound or next list
                        _positionStack[0]++;

                        if (_positionStack[0] >= _playStack[0].SoundList.Count)
                        {
                            //ListComplete(_playStack[0].SoundList.Count,_playStack[0].Name);
                            _positionStack.RemoveAt(0);
                            _playStack.RemoveAt(0);
                        }
                        else
                        {
                            if (_playStack[0].SoundList[_positionStack[0]].Delay > 0)
                            {
                                _listDelay = _playStack[0].SoundList[_positionStack[0]].Delay;
                                StartCoroutine("PlayNextSoundDelayed", _playStack[0].GetObjectAt(_positionStack[0]));
                            }
                            else
                            {
                                PlaySound(_playStack[0].GetObjectAt(_positionStack[0]));
                            }
                        }
                    }
                    else
                    {
                        //Playstack is empty, list has finished playback
                        _busy = false;
                        _waitOver = true;
                        _listDelay = 0;
                    }
                }
            }
        }

        public void PlaySourceSound(float volumeMultiplier)
        {
            //Adjustments to player according to ABObject and user input
            SourcePlayer.volume *= (volumeMultiplier * SourceSound.Volume);
            PlaySound(SourceSound);
        }

        public void PlaySound(AudioBuddyObject soundObject, float volumeMultiplier)
        {
            _externalVolumeMultipilier = volumeMultiplier;
            SourceSound = soundObject;
            switch (soundObject)
            {
                case AudioBuddySound sound:
                    PlaySimpleSound(sound);
                    break;
                case AudioBuddyList list:
                    PlayList(list);
                    break;
                case AudioBuddyRandom random:
                    PlayRandom(random);
                    break;
                default:
                    throw new ArgumentException($"{nameof(SourceSound)} is not a valid AudioBuddyObject");
                    //break;
            }
        }

        public void PlaySound(AudioBuddyObject soundObject)
        {
            PlaySound(soundObject, 1f);
        }

        private void PlaySimpleSound(AudioBuddySound sound)
        {
            SourcePlayer.clip = sound.File;
            SourcePlayer.volume = sound.Volume * _externalVolumeMultipilier;
            SourcePlayer.pitch = sound.Pitch;
            if (!(_playStack.Count > 0))
            {
                SourcePlayer.loop = sound.IsLoop;
            }
            SourcePlayer.Play();
        }

        private void PlayRandom(AudioBuddyRandom random)
        {
            PlaySound(random.GetRandomSound().SoundObject);
        }

        private void PlayList(AudioBuddyList list)
        {
            Debug.Log("Starting");
            IEnumerator ListInitializer = InitializeList(list);
            StartCoroutine(ListInitializer);
        }

        public IEnumerator PlayNextSoundDelayed(AudioBuddyObject soundObject)
        {
            _waitOver = false;
            yield return new WaitWhile(() => _listDelay > 0);
            //yield return new WaitForSecondsRealtime(_playStack[0].SoundList[_positionStack[0]].Delay); //Potentionally more accurate with player playback time in samples
            PlaySound(soundObject);
            _waitOver = true;
        }

        private IEnumerator InitializeList(AudioBuddyList list)
        {
            _playStack.Insert(0, list);
            _positionStack.Insert(0, 0);
            _listDelay = _playStack[0].SoundList[_positionStack[0]].Delay;
            _positionStack[0] = -1;
            _busy = true;
            yield return new WaitWhile(() => _listDelay > 0);
            //PlaySound(list.GetObjectAt(0));
            _waitOver = true;
        }

        public bool CheckAvailable() //?
        {
            return (SourcePlayer.isPlaying || _busy) && (_managedDynamically);
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
    }

}

