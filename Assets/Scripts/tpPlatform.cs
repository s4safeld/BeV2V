using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tpPlatform : MonoBehaviour
{
    public Material selected;
    public Material notSelected;
    private Renderer rend;

    public void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = notSelected;
    }

    public void select()
    {
        rend.sharedMaterial = selected;
    }
    public void unselect()
    {
        rend.sharedMaterial = notSelected;
    }
}
