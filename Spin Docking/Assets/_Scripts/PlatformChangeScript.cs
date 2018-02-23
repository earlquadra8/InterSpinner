using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformChangeScript : MonoBehaviour {

    [System.Serializable]
    public class PlatformText
    {
        public Text targetText;
        public string Standalone;
        public string WebGL;
    }

    public PlatformText[] plaformTexts;

    private void Start()
    {
        foreach (PlatformText text in plaformTexts)
        {
            ChangeText(text);
        }
    }

    public void ChangeText(PlatformText text)
    {
        text.Standalone = text.Standalone.Replace("\\NW", "\n");
        text.WebGL = text.WebGL.Replace("\\NW", "\n");
#if UNITY_STANDALONE
        text.targetText.text = text.Standalone;
#endif
#if UNITY_WEBGL
        text.targetText.text = text.WebGL;
#endif
    }
}
