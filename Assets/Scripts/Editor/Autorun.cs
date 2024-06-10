using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using Editor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class Autorun
{
    static Autorun()
    {
        EditorApplication.update += InitProject;

    }

    static void InitProject()
    {
        EditorApplication.update -= InitProject;
        if (EditorApplication.timeSinceStartup < 10 || !EditorPrefs.GetBool(Application.dataPath+"AlreadyOpened"))
        {
            if (SceneManager.GetActiveScene().name != "game" && Directory.Exists("Assets/Scenes"))
            {
                EditorSceneManager.OpenScene("Assets/Scenes/game.unity");

            }
            LevelMakerEditor.Init();
            LevelMakerEditor.ShowHelp();
            EditorPrefs.SetBool(Application.dataPath+"AlreadyOpened", true);
        }

    }
}