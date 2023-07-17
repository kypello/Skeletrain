using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelRender : MonoBehaviour
{
    public RenderTexture renderTexture;
    public int depth;

    void OnGUI() {
         //GUI.depth = depth;
         GUI.DrawTexture(new Rect(0,0, Screen.width * 0.8f, Screen.height * 0.8f), renderTexture);
         Canvas.ForceUpdateCanvases();
     }
}
