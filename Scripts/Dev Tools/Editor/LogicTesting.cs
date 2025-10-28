using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LogicTesting : EditorWindow
{

    float number;

    private void LogicTest()
    {
        Debug.Log((int)number + " (int)");
        Debug.Log(Mathf.FloorToInt(number) + " floor");
    }

    [MenuItem("Editor Tools/Logic Testing")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LogicTesting)); // Inherited from EditorWindow. (Which is why we don't have monobehavior)
    }

    private void OnGUI() // Impliment your own editor GUI here
    {
        number = EditorGUILayout.FloatField("Number", number);

        if (GUILayout.Button("Run Function"))
        {
            LogicTest();
        }
    }
}