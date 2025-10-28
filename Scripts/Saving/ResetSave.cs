using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.SceneManagement;

//[ExecuteInEditMode]
public class ResetSave : MonoBehaviour
{
    AResetSave[] saveableObjects;

    private bool canLoad = true;

    [SerializeField]
    private TitleScreenChanger sceneChanger;
    //private void OnGUI()
    //{
    //    if (GUILayout.Button("Delete Save"))
    //    {
    //        Activate();
    //    }
    //}

    //[MenuItem("Editor Tools/Reset Save")]
    //public static void ShowWindow()
    //{
    //    GetWindow(typeof(ResetSave)); // Inherited from EditorWindow. (Which is why we don't have monobehavior)
    //}

    public void Activate()
    {
        if (canLoad)
        {
            StartCoroutine(ActivateCo());
        }
    }
    AsyncOperation asyncLoadLevel;

    private IEnumerator ActivateCo()
    {        canLoad = false;
        
        sceneChanger.canStart = false;
        asyncLoadLevel = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);

        Debug.Log("Start Loading!");
        asyncLoadLevel.allowSceneActivation = true;

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
            Debug.Log("Forever wait");
        }
        Debug.Log("Done Loading!");
        saveableObjects = FindObjectsByType<AResetSave>(FindObjectsSortMode.InstanceID);

        foreach (AResetSave deleteSave in saveableObjects)
        {
            deleteSave.ResetSave();
        }

        asyncLoadLevel = SceneManager.UnloadSceneAsync(1);
        SceneManager.UnloadSceneAsync(2);
        yield return new WaitUntil(() => asyncLoadLevel.isDone);
        sceneChanger.canStart = true;
        sceneChanger.StartGame(null);


    }
}
