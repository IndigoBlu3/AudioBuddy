using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorStyleViewer : EditorWindow
{
    private Vector2 _scrollPosition = new Vector2(0, 0);
    private string _search = "";

    [MenuItem("Window/Editor Style Viewer")]
    static void Init()
    {
        GetWindow<EditorStyleViewer>();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        GUILayout.Label("Click a Sample to copy its Name to your Clipboard", "MiniBoldLabel");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Search:");
        _search = EditorGUILayout.TextField(_search);

        GUILayout.EndHorizontal();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        foreach (GUIStyle style in GUI.skin.customStyles)
        {
            if (style.name.ToLower().Contains(_search.ToLower()))
            {
                GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
                GUILayout.Space(7);
                if (GUILayout.Button(style.name, style))
                {

                    EditorGUIUtility.systemCopyBuffer = "\"" + style.name + "\"";
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.SelectableLabel("\"" + style.name + "\"");
                GUILayout.EndHorizontal();
                GUILayout.Space(11);
            }
        }
        GUILayout.EndScrollView();
    }
}

