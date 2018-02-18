using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene_Manager : MonoBehaviour
{
    private static LoadScene_Manager _instance;

    public static LoadScene_Manager Instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
    }
    public void LoadSceneByID(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
    public void LadScenByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
