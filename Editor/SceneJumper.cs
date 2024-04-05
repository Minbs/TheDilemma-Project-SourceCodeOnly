#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class SceneJumper : EditorWindow
{
    private static readonly string mSceneReletivePath = "Assets/Scenes";

    // Priority group index
    private const int mPlayPriorityGroup = 10;
    private const int mIngameFolderPriority = 50;
    private const int mArtFolderPriority = 100;
    private const int mNonePriority = 200;

    // Play Project Via Initialize Scene
    [MenuItem("Jumper/Play Game", priority = mPlayPriorityGroup)]
    public static void playScene_1() => playScene(getScenePath(SceneFolderType.InGame, SceneType.scn_game_initialize));

    // InGame Unity Scene
    [MenuItem("Jumper/Initialize Scene", priority = mIngameFolderPriority)]
    private static void changeScene_1() => changeScene(getScenePath(SceneFolderType.InGame, SceneType.scn_game_initialize));

    [MenuItem("Jumper/Title Scene", priority = mIngameFolderPriority)]
    private static void changeScene_2() => changeScene(getScenePath(SceneFolderType.InGame, SceneType.scn_game_title));

    [MenuItem("Jumper/Hideout Scene", priority = mIngameFolderPriority)]
    private static void changeScene_3() => changeScene(getScenePath(SceneFolderType.InGame, SceneType.scn_game_hideout));

    [MenuItem("Jumper/Ingame Scene", priority = mIngameFolderPriority)]
    private static void changeScene_4() => changeScene(getScenePath(SceneFolderType.InGame, SceneType.scn_game_ingame));

    [MenuItem("Jumper/Loader Scene", priority = mIngameFolderPriority)]
    private static void changeScene_5() => changeScene(getScenePath(SceneFolderType.InGame, SceneType.scn_game_loader));

    // Art Unity Scene
    [MenuItem("Jumper/Art UI Concepts Scene", priority = mArtFolderPriority)]
    private static void changeSceneToArt_1() => changeScene(getScenePath(SceneFolderType.Art, SceneType.scn_art_ui_concepts));

	// Art Unity Scene
	[MenuItem("Jumper/Map Editor Scene", priority = mNonePriority)]
	private static void changeSceneToMapEditor() => changeScene(getScenePath(SceneFolderType.None, SceneType.scn_map_editor));

	#region Scene Management Functions

	public static void playScene(string scenePath)
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.ExitPlaymode();
        }

        changeScene(scenePath);
        EditorApplication.isPlaying = true;
    }

    public static void changeScene(string scenePath)
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.ExitPlaymode();
        }

        // If the scene has been modified, Editor ask to you want to save it.
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        EditorSceneManager.OpenScene(scenePath);
    }

    private static string getScenePath(SceneFolderType folderType, SceneType sceneName)
    {
        return $"{mSceneReletivePath}/{mSceneFolderTable[folderType]}/{sceneName.GetSceneName()}.unity";
    }

    private enum SceneFolderType
    {
        None,
        InGame,
        Art,
    }

    private static Dictionary<SceneFolderType, string> mSceneFolderTable = new Dictionary<SceneFolderType, string>()
    {
        { SceneFolderType.None, "" },
        { SceneFolderType.InGame, "InGame" },
        { SceneFolderType.Art, "Art" },
    };

    #endregion
}

#endif