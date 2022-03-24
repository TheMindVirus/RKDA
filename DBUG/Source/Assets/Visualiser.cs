using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualiser : MonoBehaviour
{
    public GameObject Backlight = null;
    List<GameObject> LEDs = new List<GameObject>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains("LED"))
            {
                LEDs.Add(child.gameObject);
            }
        }
        foreach (GameObject go in LEDs)
        {
            go.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
    }

    void SetBacklight(string data)
    {
        float intensity = 0.0f; 
        float.TryParse(data, out intensity);
        Backlight.GetComponent<Renderer>().material.SetVector("_Color", new Vector4(intensity, 0.0f, 0.0f, 1.0f));
    }

    void SetLED(string data)
    {
        string[] raw = data.Split(',');
        float[] values = new float[raw.Length];
        for (int i = 0; i < raw.Length; ++i) { float.TryParse(raw[i].Replace(" ", ""), out values[i]); }
        int index = 0;
        foreach (GameObject go in LEDs)
        {
            int channel = 0;
            int level = 0;
            int.TryParse(go.name.Substring(3, 1), out channel);
            int.TryParse(go.name.Substring(5), out level);
            go.GetComponent<Renderer>().material.SetVector("_EmissionColor", new Vector4(values[index], 0.0f, 0.0f, 0.5f));
            ++index;
        }
    }
}
