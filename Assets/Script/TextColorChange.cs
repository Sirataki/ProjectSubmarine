using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextColorChange : MonoBehaviour {
    
    private Text text;
    private float textAlpha;
    private bool alphaFlag = true;

    [SerializeField]
    private float fadeSpeed = 0.02f;    // fadeSpeed/1F

    public void TextFade()
    {
        if (text == null)
        {
            text = this.GetComponent<Text>();
            textAlpha = text.color.a;
        }

        if (!alphaFlag)
        {
            textAlpha -= fadeSpeed;
            if (textAlpha <= 0)
            {
                alphaFlag = true;
            }
        }
        else if (alphaFlag)
        {
            textAlpha += fadeSpeed;
            if (textAlpha >=1)
            {
                alphaFlag = false;
            }
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, textAlpha);
    }
}
