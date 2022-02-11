using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Color Configuration")]

public class ColorConfig : ScriptableObject
{
    [SerializeField] [Range(0, 255)] float red = 0f;
    [SerializeField] [Range(0, 255)] float green = 0f;
    [SerializeField] [Range(0, 255)] float blue = 0f;

    public float GetRed() { return red / 255f; }
    public float GetGreen() { return green / 255f; }
    public float GetBlue() { return blue / 255f; }
}
