using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeSceneScript : MonoBehaviour
{
    public GameObject sceneSelectPanel;

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
}
