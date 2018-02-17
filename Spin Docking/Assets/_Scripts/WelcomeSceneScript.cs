using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeSceneScript : MonoBehaviour
{
    public delegate void DeleteStars();
    public static DeleteStars DeletedStars;

    public GameObject sceneSelectPanel;
    public GameObject levelParent;

    private void Start()
    {
        sceneSelectPanel.SetActive(false);
    }
    public void ToggleSceneSelection()
    {
        if (sceneSelectPanel !=null)
        {
            if (!sceneSelectPanel.activeSelf)
            {
                sceneSelectPanel.SetActive(true);
            }
            else
            {
                sceneSelectPanel.SetActive(false);
            }
        }
    }
    public void ResetScore()
    {
        PlayerPrefs.DeleteAll();
        if (DeletedStars != null)
        {
            DeletedStars();
        }
    }
}
