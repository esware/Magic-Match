using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JellyGarden.Scripts.Editor
{
    [InitializeOnLoad]
    public static class EditorMenu
    {

        [MenuItem("EWGames/Scenes/Main scene")]
        public static void MainScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/main.unity");
        }

        [MenuItem("EWGames/Scenes/Game scene")]
        public static void GameScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/game.unity");
        }
    }
}