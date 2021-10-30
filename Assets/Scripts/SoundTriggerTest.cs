using UnityEngine;
using AudioBuddyTool;
using System.Collections;

public class SoundTriggerTest : MonoBehaviour
{
    public Vector3 SoundOffset;
    [Range(0,1)]
    public float VolumeMultiplier;
    public GameObject AttachTarget;
    public AudioBuddySpeaker Speaker;
    public RotatingMovement Rotation;
    public AudioBuddyObject Alpha1;
    public AudioBuddyObject Alpha2;
    public AudioBuddyObject Alpha3;

    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AudioBuddy.Play("20");
            Debug.Log(Alpha1.name);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Speaker != null)
            {
                if (Speaker.CheckAvailable())
                {
                    Speaker.FadeOut(5);
                }
                else
                {
                    Speaker.PlaySound(AudioBuddy.FindSoundByName("Music"));
                    Speaker.FadeIn(5);
                }
            }
            Speaker ??= AudioBuddy.FadeIn("Music", 10f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AudioBuddy.Play(Alpha3);
            Debug.Log(Alpha3.name);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AudioBuddy.Play("Gegenstrom");
            Debug.Log("Alpha");
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AudioBuddy.Play("Take_charge");
            Debug.Log("Liste");
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AudioBuddy.Play("Aussagen");
            Debug.Log("Aussage");
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Rotation.enabled = true;
            AudioBuddy.AttachSound("DropLoop",AttachTarget);
            Debug.Log("AttachLoop");
        }
    }   

    public void DebugSound(AudioBuddyObject sound)
    {
        Debug.Log(sound.name);
    }

    public void RemoveFromActions(AudioBuddySpeaker speaker)
    {
        speaker.OnSpeakerReassign -= RemoveFromActions;
    }

    public void Start()
    {
        //Charlotte = AudioBuddy.Play("Feenklang", Vector3.zero);
    }
}
