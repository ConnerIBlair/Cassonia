using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;


public class TilemapTools : Editor
{
    public Vector2Int TPosition;
    Tilemap tilemap;

    public enum Tile { DirtWall, StoneWall}
    public Tile[] dirtTiles;
    public Tile[] stoneTiles;

    //[MenuItem("Editor Tools/Tilemap")]
    //public static void ShowWindow()
    //{
    //    GetWindow(typeof(TilemapTools)); // Inherited from EditorWindow. (Which is why we don't have monobehavior)
    //}


    private void OnSceneGUI(SceneView sceneView)
    {
        //TPosition = EditorGUILayout.Vector2IntField("Vector2", TPosition);
        tilemap = EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), false) as Tilemap;

        SceneView.duringSceneGui += OnSceneGUI;

        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

        Debug.Log(mousePosition);
    }
}
