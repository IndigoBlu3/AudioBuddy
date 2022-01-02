using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioBuddyTool
{
    [CustomEditor(typeof(AudioBuddySound))]
    public class ABSoundEditor : Editor
    {
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
                    subtle.normal.background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/_BBG.png");
                    _subtleBG = subtle;
                    return _subtleBG;
                }
            }
        }
        private string _newName;
        AudioBuddySound soundObject;

        private void OnEnable()
        {
            soundObject = (AudioBuddySound)target;
            soundObject.CustomName = false;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            {
                EditorGUILayout.BeginHorizontal();
                if (soundObject.CustomName)
                {
                    _newName = EditorGUILayout.TextField(_newName);
                }
                else
                {
                    EditorGUILayout.LabelField($"{soundObject.name} Audio Buddy Sound", EditorStyles.whiteLargeLabel);
                }
                if (GUILayout.Button($"{(soundObject.CustomName ? "Finish Edit" : "Edit Name")}", EditorStyles.miniButtonRight))
                {
                    soundObject.CustomName = !soundObject.CustomName;
                    if (!soundObject.CustomName)
                    {
                        soundObject.name = _newName;
                        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(soundObject), _newName);
                    }
                    else
                    {
                        _newName = soundObject.name;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            //Simple user settings for this sound object
            soundObject.IsLoop = EditorGUILayout.Toggle("Looping", soundObject.IsLoop);
            soundObject.Volume = EditorGUILayout.Slider("Volume", soundObject.Volume, 0, 1);
            soundObject.Pitch = EditorGUILayout.Slider("Pitch", soundObject.Pitch, -4, 4);
            soundObject.File = (AudioClip)EditorGUILayout.ObjectField("File", soundObject.File, typeof(AudioClip), false);
            soundObject.MixerGroupOverride = (AudioMixerGroup)EditorGUILayout.ObjectField("Mixer Override", soundObject.MixerGroupOverride, typeof(AudioMixerGroup), false);
            EditorGUILayout.LabelField($"File Path: {soundObject.FilePath}",EditorStyles.label);
            
            EditorGUILayout.HelpBox("There are some heavy changes happening to how sound objects and speakers work in the background. Everything below this message is experimental.",MessageType.Info);
            EditorGUILayout.CurveField("Kurve", soundObject.Kurva);
            AudioCurveRendering.DrawCurve(EditorGUILayout.GetControlRect(), soundObject.Kurva.Evaluate, Color.red);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }

}