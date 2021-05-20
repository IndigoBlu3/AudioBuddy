using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AudioBuddyTool
{
    public class AudioBuddyManager : MonoBehaviour
    {

        public GameObject SpeakerPrefab;

        public List<AudioBuddySpeaker> Speakers = new List<AudioBuddySpeaker>();

        public AudioBuddySpeaker Play2D(AudioBuddyObject abobject, float volumeMultiplier)
        {
            AudioBuddySpeaker player = NextAvailablePlayerOrNew();
            //TODO: Adjust rollof
            player.SourcePlayer.spatialBlend = 0;
            player.PlaySound(abobject, volumeMultiplier);
            return player;
        }

        public AudioBuddySpeaker PlayAtLocation(AudioBuddyObject abobject, float volumeMultiplier, Vector3 location)
        {
            AudioBuddySpeaker player = NextAvailablePlayerOrNew();
            player.SourcePlayer.spatialBlend = 1;
            player.gameObject.transform.position = location;
            player.PlaySound(abobject, volumeMultiplier);
            return player;
        }

        public AudioBuddySpeaker NextAvailablePlayer()
        {
            if (Speakers.Any(s => !s.CheckAvailable()))
            {
                return Speakers.First(s => !s.CheckAvailable());
            }
            return null;
        }

        public AudioBuddySpeaker NextAvailablePlayerOrNew()
        {
            AudioBuddySpeaker player = NextAvailablePlayer();
            if (player == null)
            {
                player = Instantiate(SpeakerPrefab).GetComponent<AudioBuddySpeaker>();
                Speakers.Add(player);
            }
            return player;
        }

    }

}
