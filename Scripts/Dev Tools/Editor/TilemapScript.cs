using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( Test ) )]
public class TilemapScript : Editor
{
    private void OnSceneGUI()
    {
        // Get the chosen GameObject
        Test t = target as Test;

        if (t == null || t.gameObjects == null)
            return;

        //Grab the center of the parent
        Vector3 center = t.transform.position;

        // Iterate over GameObject added to the array...
        for (int i = 0; i < t.gameObjects.Length; i++)
        {
            // ... and draw a line between them
            if (t.gameObjects[i] != null)
                Handles.DrawLine(center, t.gameObjects[i].transform.position);
        }
    }
}
