using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AudioBuddyTool
{
    [CustomEditor(typeof(AudioBuddyImportManager))]
    public class ABImporterEditor : Editor
    {
        AudioBuddyImportManager importer;
        public enum SortType { Name, Length, Type };
        private SortType _sortMode;
        private string _sortText;
        private bool _showDatabase;
        private bool _confirmRebuild;
        private int _dbCounter;
        private bool _dbbroken;

        private void OnEnable()
        {
            importer = (AudioBuddyImportManager)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Audio Buddy Import Manager", EditorStyles.whiteLargeLabel);
                EditorGUILayout.LabelField("Alpha: a0520", AudioBuddy.RightAligned);
                EditorGUILayout.EndHorizontal();
            } //Title
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"There are {importer.ABObjectCollection.Count} sounds in database");
                if (GUILayout.Button($"{(_showDatabase ? "Hide Database" : "Show Database")}", EditorStyles.miniButtonRight))
                {
                    _showDatabase = !_showDatabase;
                }
                EditorGUILayout.EndHorizontal();
            } //Subtitle + Show Database Button

            importer.CollectionAddress = EditorGUILayout.TextField("Database Address:", importer.CollectionAddress);

            {
                EditorGUILayout.BeginHorizontal();

                if (!_confirmRebuild)
                {
                    if (GUILayout.Button("Rebuild Entire Database"))
                    {
                        _confirmRebuild = true;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Are you REALLY sure?", EditorStyles.boldLabel);
                    if (GUILayout.Button("No"))
                    {
                        _confirmRebuild = false;
                    }
                    if (GUILayout.Button("Yes"))
                    {
                        _confirmRebuild = false;
                        importer.RebuildAudioBuddyObjectCollection();
                    }
                }
                EditorGUILayout.EndHorizontal();
            } //Rebuild database

            if (GUILayout.Button("Import missing sounds"))
            {
                importer.CreateMissingAudioBuddyObjects();
            }

            if (GUILayout.Button("Discover new Audio Buddy Objects"))
            {
                importer.DiscoverAudioBuddyObjects();
                Debug.Log("Finished discovery");
            }

            EditorGUILayout.Space(10);

            if (_showDatabase)
            {
                EditorGUILayout.BeginHorizontal(AudioBuddy.SubtleBG);
                EditorGUILayout.LabelField("Sounds in database:", EditorStyles.boldLabel);
                if (GUILayout.Button($"Sorted by {_sortText}"))
                {
                    switch (_sortMode)
                    {
                        case SortType.Name:
                            importer.ABObjectCollection = importer.ABObjectCollection.OrderBy(n => n.name).ToList();
                            _sortMode = SortType.Length;
                            _sortText = "name";
                            break;
                        case SortType.Length:
                            importer.ABObjectCollection = importer.ABObjectCollection.OrderBy(d => d.GetDuration()).ToList();
                            _sortMode = SortType.Type;
                            _sortText = "length";
                            break;
                        case SortType.Type:
                            importer.ABObjectCollection = importer.ABObjectCollection.OrderBy(t => t.GetType().ToString()).ToList();
                            _sortMode = SortType.Name;
                            _sortText = "type";
                            break;
                        default:
                            _sortText = "none";
                            break;
                    }
                }
                EditorGUILayout.EndHorizontal();

                //Visualizes database
                if (importer.ABObjectCollection.Count > 0)
                {
                    _dbCounter = 0;
                    foreach (AudioBuddyObject aboject in importer.ABObjectCollection)
                    {
                        if (aboject == null)
                        {
                            importer.RescanAudioBuddyObjects();
                            Debug.LogWarning("AudioBuddy rescanned database to fix references for deleted or missing objects");
                            return;
                        }
                        _dbCounter++;
                        if (_dbCounter % 2 == 0)
                        {
                            EditorGUILayout.BeginHorizontal(AudioBuddy.SubtleBG);
                        }
                        else
                        {
                            EditorGUILayout.BeginHorizontal();
                        }
                        EditorGUILayout.LabelField($"{aboject.name}");
                        EditorGUILayout.LabelField($"{aboject.GetType()}", EditorStyles.helpBox);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Please import sounds before showing the database", EditorStyles.centeredGreyMiniLabel);
                }

            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }


}
