using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AudioBuddyTool
{
    [CustomEditor(typeof(AudioBuddyList))]
    public class ABListEditor : Editor
    {
        public List<BuddyListEntry> _forDelete = new List<BuddyListEntry>();
        AudioBuddyList listObject;

        private int _entryWidthCounter;
        private BuddyListEntry _moveUp;
        private BuddyListEntry _moveDown;

        private void OnEnable()
        {
            listObject = (AudioBuddyList)target;
        }

        public override void OnInspectorGUI()
        {
            _moveUp = null;
            _moveDown = null;
            //base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField($"{listObject.name} Audio Buddy List", EditorStyles.whiteLargeLabel);
            GUILayout.Label($"{listObject.SoundList.Count} entries are in this list");
            GUILayout.Label($"Duration of List: {listObject.GetDuration()}");
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add entry"))
                {
                    listObject.SoundList.Add(new BuddyListEntry());
                }

                if (GUILayout.Button("Remove Entry") && listObject.SoundList.Count > 0)
                {
                    listObject.SoundList.RemoveAt(0);
                }
            }
            GUILayout.EndVertical();

            //listObject.Duration = 0;
            _entryWidthCounter = 21;
            foreach (BuddyListEntry entry in listObject.SoundList)
            {
                DrawListEntry(entry);
                float entryDuration = entry.BuddyEntry != null ? entry.BuddyEntry.Duration : 0f;
            }

            ApplyDeletions();
            ApplyMovement();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        public void DrawListEntry(BuddyListEntry entry)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    //Vector2 baseposition = EditorGUILayout.GetControlRect().position;
                    _entryWidthCounter += (entry.BuddyEntry == null ? 57 : 64) + 5;
                    Vector2 baseposition = new Vector2(1, _entryWidthCounter);
                    Vector2 size = new Vector2(20, 28);
                    Rect upRect = new Rect(baseposition, size);
                    Rect downRect = new Rect(baseposition.x, baseposition.y + size.y + 0, size.x, size.y);

                    //GUIStyle pristineButton = GUI.skin.button;
                    //GUI.skin.button.stretchHeight = true;
                    //GUI.skin.button.stretchWidth = true;
                    //GUI.skin.button.padding = new RectOffset(1,1,0,0);

                    if (GUI.Button(upRect, AudioBuddy.ArrowUp))
                    {
                        _moveUp = entry;
                    }
                    if (GUI.Button(downRect, AudioBuddy.ArrowDown))
                    {
                        _moveDown = entry;
                    }

                    //GUI.skin.button = pristineButton;
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(AudioBuddy.SubtleBG);
                {

                    EditorGUILayout.BeginHorizontal();
                    {

                        EditorGUILayout.LabelField(entry.NameInList, EditorStyles.whiteLabel);
                        if (GUILayout.Button("X"))
                            _forDelete.Add(entry);
                    }
                    EditorGUILayout.EndHorizontal();

                    DrawEntryObjectFields(entry);

                    //TODO: Adapt Speaker to work with delay
                    //entry.Delay = EditorGUILayout.FloatField("Delay",entry.Delay);

                    DrawEntryDuration(entry);

                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        public void DrawEntryObjectFields(BuddyListEntry entry)
        {
            EditorGUILayout.BeginHorizontal();
            {
                entry.BuddyEntry = (AudioBuddyObject)EditorGUILayout.ObjectField(entry.BuddyEntry, typeof(AudioBuddyObject), false);
                if (entry.BuddyEntry != null)
                {
                    entry.NameInList = entry.BuddyEntry.name;
                    if (GUILayout.Button("Reset"))
                    {
                        entry.BuddyEntry = null;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (entry.BuddyEntry == null)
            {
                GUILayout.Label("Please assign a valid Audio Buddy Object", EditorStyles.centeredGreyMiniLabel);
            }
        }

        public void DrawEntryDuration(BuddyListEntry entry)
        {
            EditorGUILayout.BeginHorizontal();
            {

                if (entry.BuddyEntry != null)
                {
                    float _originalWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 80;
                    EditorGUILayout.FloatField("Length", entry.BuddyEntry.GetDuration(), EditorStyles.helpBox);
                    entry.Delay = EditorGUILayout.FloatField("Delay", entry.Delay);
                    EditorGUILayout.FloatField("Timestamp", entry.Timestamp, EditorStyles.helpBox);
                    EditorGUIUtility.labelWidth = _originalWidth;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public void ApplyDeletions()
        {
            foreach (BuddyListEntry item in _forDelete)
            {
                listObject.SoundList.Remove(item);
            }
            _forDelete.Clear();
        }

        public void ApplyMovement()
        {
            if (_moveUp != null && _moveUp != listObject.SoundList[0])
            {
                int moveIndex = listObject.SoundList.FindIndex(e => e == _moveUp);
                listObject.SoundList[moveIndex] = listObject.SoundList[moveIndex - 1];
                listObject.SoundList[moveIndex - 1] = _moveUp;
            }
            else if (_moveDown != null && _moveDown != listObject.SoundList[listObject.SoundList.Count - 1])
            {
                int moveIndex = listObject.SoundList.FindIndex(e => e == _moveDown);
                listObject.SoundList[moveIndex] = listObject.SoundList[moveIndex + 1];
                listObject.SoundList[moveIndex + 1] = _moveDown;
            }
        }
    }
}