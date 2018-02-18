using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeSceneScript : MonoBehaviour
{
    public delegate void DeleteStars();
    public static DeleteStars DeletedStars;

    public Text verNum;
    public GameObject sceneSelectPanel;

    private void Start()
    {
        sceneSelectPanel.SetActive(false);
        verNum.text = string.Format("Ver: {0}", Application.version);
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
