using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueResponseEvents))]

public class DialogueResponseEventsEditor : Editor
{ // This script is editor only. When you modify a dialogue object in the folder,
  // it updates the dialogue already attached to the object with dialogue.
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueResponseEvents responseEvents = (DialogueResponseEvents)target;

        if (GUILayout.Button("Refresh"))
        {
            responseEvents.OnValidate();
        }
    }

}
