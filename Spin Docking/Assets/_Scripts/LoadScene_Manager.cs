using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene_Manager : MonoBehaviour
{
    public void LoadSceneByID(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
    public void LadScenByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
