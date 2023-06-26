using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AudioBuddyTool
{
    [CustomEditor(typeof(AudioBuddySpeakerSetting))]
    public class ABSpeakerSettingsEditor : Editor
    {
        AudioBuddySpeakerSetting audioSetting;
        //AudioCurveRendering.AudioCurveEvaluator AudioCurveEvaluator = new AudioCurveRendering.AudioCurveEvaluator(floaty);
        private void OnEnable()
        {
            audioSetting = (AudioBuddySpeakerSetting)target;
        }

        private float floaty(float f)
        {
            return f;
        }
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();
            Rect recti = new Rect(0,0,0,100);
            Rect controlRect = EditorGUILayout.GetControlRect();
            recti.x = controlRect.x;
            recti.y = controlRect.y;
            recti.width = controlRect.width;

            //audioSetting.CustomRollofCurve = EditorGUILayout.CurveField("Custom Rollof", audioSetting.CustomRollofCurve, Color.red, recti);
            AudioCurveRendering.BeginCurveFrame(recti);
            
            AudioCurveRendering.EndCurveFrame();
            
            //AudioCurveRendering.DrawCurve(recti,  Color.red);

        }
    }
}

