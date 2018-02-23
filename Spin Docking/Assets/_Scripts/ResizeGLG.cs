using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeGLG : MonoBehaviour
{
    GridLayoutGroup targetGLG;
    public int paddingLeftIn1600to900, paddingRightIn1600to900, paddingTopIn1600to900, paddingBottomIn1600to900;
    public Vector2 sizeIn1600to900;
    public Vector2 spacingIn1600to900;
    void Start()
    {
        //targetGLG = GetComponent<GridLayoutGroup>();
        //targetGLG.padding.left = Screen.width * paddingLeftIn1600to900 / 1600;
        //targetGLG.padding.right = Screen.width * paddingRightIn1600to900 / 1600;
        //targetGLG.padding.top = Screen.height * paddingTopIn1600to900 / 900;
        //targetGLG.padding.bottom = Screen.height * paddingBottomIn1600to900 / 900;
        //targetGLG.cellSize = new Vector2(Screen.width * sizeIn1600to900.x / 1600, Screen.height * sizeIn1600to900.y / 900);
        //targetGLG.spacing = new Vector2(Screen.width * spacingIn1600to900.x / 1600, Screen.height * spacingIn1600to900.y / 900);
    }
}
