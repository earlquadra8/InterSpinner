using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    
    public int levelNum;
    public Text levelText;
    public GameObject starParent;

    private void OnEnable()
    {
        WelcomeSceneScript.DeletedStars += DisableAllStars;

        levelNum = transform.GetSiblingIndex() + 1;
    }
    private void OnDisable()
    {
        WelcomeSceneScript.DeletedStars -= DisableAllStars;
    }
    private void Start()
    {
        DisableAllStars();
        ChangeText();
        EnableStars();
    }

    void ChangeText()
    {
        levelText.text = string.Format("Level {0}", (transform.GetSiblingIndex() + 1).ToString("00"));
    }
    void EnableStars()
    {
        int starCount = PlayerPrefs.GetInt(("level" + levelNum + "StarCount"), 0);
        if (starCount <= starParent.transform.childCount)
        {
            for (int i = 0; i < starCount; i++)
            {
                starParent.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
    void DisableAllStars()
    {
        for (int i = 0; i < starParent.transform.childCount; i++)
        {
            starParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ButtonLoadSceneByID()
    {
        LoadScene_Manager.Instance.LoadSceneByID(levelNum);
    }
}
