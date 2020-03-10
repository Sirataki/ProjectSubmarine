using UnityEngine;
using System.Collections;

public class TextureScroll : MonoBehaviour
{
    public float waveSpeed = 0.2f;  // Texture scroll speed
    public bool direction = true;   // true = Right, false = Left

    void Update()
    {
        if (direction)
        {
            float scroll = Mathf.Repeat(Time.time * -waveSpeed, 1);
            Vector2 offset = new Vector2(scroll, 0);
            GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
        else if (!direction)
        {
            float scroll = Mathf.Repeat(Time.time * waveSpeed, 1);
            Vector2 offset = new Vector2(scroll, 0);
            GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
        }

    }
}