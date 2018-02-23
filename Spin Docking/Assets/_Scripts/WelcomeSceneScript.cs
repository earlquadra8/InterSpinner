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
    public GridLayoutGroup levelParentGLG;

    private void Start()
    {
        sceneSelectPanel.SetActive(false);
        verNum.text = string.Format("Ver: {0}", Application.version);
        //levelParentGLG.padding.left = (int)(Screen.width * 30 / 1600);
        //levelParentGLG.padding.top = (int)(Screen.height * 30 / 900);
        //levelParentGLG.cellSize = new Vector2(Screen.width * 290 / 1600, Screen.height * 200 / 900);
        //levelParentGLG.spacing = new Vector2(Screen.width * 20 / 1600, Screen.height * 20 / 900);
    }
    private void Update()
    {
        
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
