using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AudioBuddyTool
{
    [CustomEditor(typeof(AudioBuddyRandom))]
    public class ABRandomEditor : Editor
    {
        private List<BuddyRandomEntry> _forDelete = new List<BuddyRandomEntry>();

        private bool _orderDescending;

        AudioBuddyRandom randomObject;

        private void OnEnable()
        {
            randomObject = (AudioBuddyRandom)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField($"{randomObject.name} Audio Buddy List", EditorStyles.whiteLargeLabel);
            GUILayout.Label($"{randomObject.RandomList.Count} entries are in this list");
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add entry"))
                {
                    randomObject.RandomList.Add(new BuddyRandomEntry());
                }

                if (GUILayout.Button("Remove Entry") && randomObject.RandomList.Count > 0)
                {
                    randomObject.RandomList.RemoveAt(0);
                }

                if (GUILayout.Button($"Order {(_orderDescending ? "ascending" : "descending")}") && randomObject.RandomList.Count > 0)
                {
                    _orderDescending = !_orderDescending;
                    if (_orderDescending)
                    {
                        randomObject.RandomList = randomObject.RandomList.OrderByDescending(c => c.Chance).ToList();
                    }
                    else
                    {
                        randomObject.RandomList = randomObject.RandomList.OrderBy(c => c.Chance).ToList();
                    }
                }
            }
            GUILayout.EndVertical();

            foreach (BuddyRandomEntry entry in randomObject.RandomList)
            {
                DrawRandomEntry(entry);
            }

            ApplyDeletions();
            if (EditorGUI.EndChangeCheck())
            {
                randomObject.RecalculateChances();
                EditorUtility.SetDirty(target);
            }
        }

        public void DrawRandomEntry(BuddyRandomEntry entry)
        {
            EditorGUILayout.BeginVertical(AudioBuddy.SubtleBG);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(entry.NameInRandom, EditorStyles.whiteLabel);
                    if (GUILayout.Button("X"))
                        _forDelete.Add(entry);
                }
                EditorGUILayout.EndHorizontal();

                DrawEntryObjectFields(entry);

                //DrawEntryDuration(entry);


            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(3);
        }

        public void ApplyDeletions()
        {
            foreach (BuddyRandomEntry entry in _forDelete)
            {
                randomObject.RandomList.Remove(entry);
            }
            _forDelete.Clear();
        }

        public void DrawEntryObjectFields(BuddyRandomEntry entry)
        {
            EditorGUILayout.BeginHorizontal();
            entry.SoundObject = (AudioBuddyObject)EditorGUILayout.ObjectField(entry.SoundObject, typeof(AudioBuddyObject), false);
            if (entry.SoundObject != null)
            {
                entry.NameInRandom = entry.SoundObject.name;
                if (GUILayout.Button("Reset"))
                {
                    entry.SoundObject = null;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (entry.SoundObject == null)
            {
                GUILayout.Label("Please assign a valid Audio Buddy Object", EditorStyles.centeredGreyMiniLabel);
            }
            else    //Draw Weights and Chances
            {
                EditorGUILayout.BeginHorizontal();
                Rect ControllRect = EditorGUILayout.GetControlRect();
                Rect _firstThird = ControllRect;
                _firstThird.width /= 3.1f;
                Rect _secondThird = _firstThird;
                _secondThird.x += ControllRect.width / 3f;
                Rect _thirdThird = _secondThird;
                _thirdThird.x += ControllRect.width / 3f;

                float _originalWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 80;

                if (entry.LockChance)
                {
                    EditorGUI.FloatField(_firstThird, "Weight", entry.Weight, EditorStyles.helpBox);
                    entry.Chance = Mathf.Clamp(EditorGUI.FloatField(_secondThird, "Chance", entry.Chance), 0, randomObject.GetLeftoverChance(entry));
                }
                else
                {
                    entry.Weight = Mathf.Max(EditorGUI.FloatField(_firstThird, "Weight", entry.Weight), 0);
                    EditorGUI.FloatField(_secondThird, "Chance", entry.Chance, EditorStyles.helpBox);
                }
                entry.LockChance = EditorGUI.Toggle(_thirdThird, "Lock Chance", entry.LockChance);
                EditorGUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = _originalWidth;
            }
        }
    }
}
