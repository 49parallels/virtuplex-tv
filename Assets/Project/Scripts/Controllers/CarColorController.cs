using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarColorController : MonoBehaviour
{

    private Color currentColor = Color.yellow;
    // Update is called once per frame
    public void OnColorChanged(Color carColor)
    {
        currentColor = carColor;
        Shader.SetGlobalColor("_GlobalColor", currentColor);
    }
    
    private void Update()
    {
        Shader.SetGlobalColor("_GlobalColor", currentColor);
    }
}
