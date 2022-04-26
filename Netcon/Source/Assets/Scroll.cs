using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    private Vector2 vec2 = new Vector2(0.0f, 0.0f);
    private Renderer render = null;

    void Start()
    {
        render = this.GetComponent<Renderer>();
        vec2 = render.material.GetTextureScale("_MainTex");
        vec2 = new Vector2(vec2.x * 1.1f, vec2.y * 1.1f);
        render.material.SetTextureScale("_MainTex", vec2);
    }

    void Update()
    {
        vec2 = render.material.GetTextureOffset("_MainTex");
        vec2 = new Vector2(vec2.x - 0.0005f, vec2.y - 0.0005f);
        render.material.SetTextureOffset("_MainTex", vec2);
    }
}
