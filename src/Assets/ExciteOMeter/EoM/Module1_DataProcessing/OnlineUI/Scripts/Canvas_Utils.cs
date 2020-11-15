using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_Utils : MonoBehaviour
{

    float copyCanvasGroupAlpha = 0;

    // Singleton
    public static Canvas_Utils instance;

    /// <summary>
    /// Set instance for settings object and initialize callbacks of UI
    /// </summary>
    private void Awake()
    {
        // Check singleton, each time the menu scene is loaded, the instance is replaced with the newest script
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// Switches the visibility of a Game Object
    public void ToggleVisibility(GameObject go)
    {
        if(go != null)
            go.SetActive(!go.activeSelf);
    }

    public void ToggleCanvasGroup(CanvasGroup cg)
    {
        if(cg.alpha != 0f)
        {
            copyCanvasGroupAlpha = cg.alpha;
            cg.alpha = 0f;
            cg.interactable = false;
        }
        else
        {
            cg.alpha = 1.0f;
            cg.interactable = true;
        }
    }

    public void SetVisibilityCanvasGroup(CanvasGroup cg, bool active)
    {
        if(active)
        {
            cg.alpha = 1.0f;
            cg.interactable = true;
        }
        else
        {
            cg.alpha = 0f;
            cg.interactable = false;
        }
    }
}
