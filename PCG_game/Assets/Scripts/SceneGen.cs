using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGen : MonoBehaviour
{
    public GameObject prefabToInstantiate;

    private void Start()
    {
        // Generate and load Scene 1
        Scene scene1 = GenerateScene("Scene1");
        LoadScene(scene1);

        // Generate and load Scene 2
        Scene scene2 = GenerateScene("Scene2");
        LoadScene(scene2);

        //SwitchToScene("Scene2");
    }

    private Scene GenerateScene(string sceneName)
    {
        // Create a new empty scene
        Scene generatedScene = SceneManager.CreateScene(sceneName);

        // Instantiate objects and position them
        GameObject spawnedObj = Instantiate(prefabToInstantiate, new Vector3(0f, 0f, 0f), Quaternion.identity);
        spawnedObj.name = sceneName;
        // Configure and customize the scene and objects

        return generatedScene;
    }

    private void LoadScene(Scene scene)
    {
        // Load the generated scene
        SceneManager.LoadScene(scene.buildIndex, LoadSceneMode.Additive);
    }

    private void UnloadScene(Scene scene)
    {
        // Unload the scene
        SceneManager.UnloadScene(scene.buildIndex);
    }

    public void SwitchToScene(string sceneName)
    {
        // Find the scene by name
        Scene targetScene = SceneManager.GetSceneByName(sceneName);

        if (targetScene.IsValid())
        {
            // Unload the current scene
            Scene currentScene = SceneManager.GetActiveScene();
            UnloadScene(currentScene);

            // Load the target scene
            LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("Scene '" + sceneName + "' not found!");
        }
    }
}
