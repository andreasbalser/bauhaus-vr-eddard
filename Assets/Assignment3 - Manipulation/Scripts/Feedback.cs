using UnityEngine;
using System;
using Unity.Netcode;

public class Feedback : MonoBehaviour
{
    public int hoverBrigherValue;

    private Color initColor;
    private Color hoverColor;
    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        initColor = renderer.material.color;
        hoverColor = makeBrighter(initColor);
    }

    void OnMouseEnter()
    {
        renderer.material.color = hoverColor;
    }

    void OnMouseExit()
    {
        renderer.material.color = initColor;
    }

    void OnGrabStart()
    {
        
    }



    private Color makeBrighter(Color color)
    {
        Color retColor = new Color(
            color.r + hoverBrigherValue,
            color.g + hoverBrigherValue,
            color.b + hoverBrigherValue);

        return retColor;
    }
}