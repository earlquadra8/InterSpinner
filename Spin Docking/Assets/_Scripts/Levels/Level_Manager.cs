using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Manager : MonoBehaviour
{
    private static Level_Manager _instance;
    public static Level_Manager Instance { get { return _instance; } }

    public int levelNum;
    public bool levelDoTimer;
    public float levelTime;
    public float levelMaxFuel;
    public bool levelDoScore;
    public float levelThreeStarScore;

    public bool dontSave = false;

    private void Awake()
    {
        _instance = this;
    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 0)
        {
            levelNum = SceneManager.GetActiveScene().buildIndex;
        }
    }
    bool isSaved;
    public void SaveStars()
    {
        if (!isSaved && !dontSave)
        {
            isSaved = true;
            if (UI_Manager.Instance.StarCount > PlayerPrefs.GetInt(("level" + levelNum + "StarCount"), 0))
            {
                PlayerPrefs.SetInt(("level" + levelNum + "StarCount"), UI_Manager.Instance.StarCount);
                print("saved " + UI_Manager.Instance.StarCount);
            }
        }
    }
}
