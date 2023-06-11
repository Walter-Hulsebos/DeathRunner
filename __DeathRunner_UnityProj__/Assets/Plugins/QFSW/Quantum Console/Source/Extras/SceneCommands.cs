using QFSW.QC.Suggestors.Tags;
using QFSW.QC.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFSW.QC.Extras
{
    public static class SceneCommands
    {
        private static async Task PollUntilAsync(int pollInterval, Func<bool> predicate)
        {
            while (!predicate())
            {
                await Task.Delay(millisecondsDelay: pollInterval);
            }
        }

        [Command(aliasOverride: "load-scene", description: "loads a scene by name into the game")]
        private static async Task LoadScene(
            [SceneName]
            string sceneName,

            [CommandParameterDescription(description: "'Single' mode replaces the current scene with the new scene, whereas 'Additive' merges them")]
            LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneUtilities.LoadSceneAsync(sceneName: sceneName, mode: loadMode);
            await PollUntilAsync(pollInterval: 16, predicate: () => asyncOperation.isDone);
        }

        [Command(aliasOverride: "load-scene-index", description: "loads a scene by index into the game")]
        private static async Task LoadScene(int sceneIndex,
        [CommandParameterDescription(description: "'Single' mode replaces the current scene with the new scene, whereas 'Additive' merges them")]LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex: sceneIndex, mode: loadMode);
            await PollUntilAsync(pollInterval: 16, predicate: () => asyncOperation.isDone);
        }

        [Command(aliasOverride: "unload-scene", description: "unloads a scene by name")]
        private static async Task UnloadScene([SceneName(LoadedOnly = true)] string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName: sceneName);
            await PollUntilAsync(pollInterval: 16, predicate: () => asyncOperation.isDone);
        }

        [Command(aliasOverride: "unload-scene-index", description: "unloads a scene by index")]
        private static async Task UnloadScene(int sceneIndex)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneBuildIndex: sceneIndex);
            await PollUntilAsync(pollInterval: 16, predicate: () => asyncOperation.isDone);
        }

        [Command(aliasOverride: "all-scenes", description: "gets the name and index of every scene included in the build")]
        private static Dictionary<int, string> GetAllScenes()
        {
            Dictionary<int, string> sceneData = new();
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                int sceneIndex = i;
                string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex: sceneIndex);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path: scenePath);

                sceneData.Add(key: sceneIndex, value: sceneName);
            }

            return sceneData;
        }

        [Command(aliasOverride: "loaded-scenes", description: "gets the name and index of every scene currently loaded")]
        private static IEnumerable<KeyValuePair<int, string>> GetLoadedScenes()
        {
            return SceneUtilities.GetLoadedScenes()
                .OrderBy(keySelector: x => x.buildIndex)
                .Select(selector: x => new KeyValuePair<int, string>(key: x.buildIndex, value: x.name));
        }

        [Command(aliasOverride: "current-scene", description: "gets the name of the active primary scene")]
        [Command(aliasOverride: "active-scene", description: "gets the name of the active primary scene")]
        private static string GetCurrentScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            return scene.name;
        }

        [Command(aliasOverride: "set-active-scene", description: "sets the active scene to the scene with name 'sceneName'")]
        private static void SetActiveScene([SceneName(LoadedOnly = true)] string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(name: sceneName);
            if (!scene.isLoaded)
            {
                throw new ArgumentException(message: $"Scene {sceneName} must be loaded before it can be set active");
            }

            SceneManager.SetActiveScene(scene: scene);
        }
    }
}
