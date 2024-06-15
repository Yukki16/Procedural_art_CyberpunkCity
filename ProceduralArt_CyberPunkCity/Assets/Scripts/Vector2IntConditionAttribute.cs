using System;
using UnityEngine;
using UnityEngine.Scripting;

/// <summary>
/// Attribute created to restrict the parameter in the SkyScraper building size (the random parameter)
/// It won't allow the y to be < x as the UnityEngine.Random.Range will give weird values otherwise.
/// </summary>
public class Vector2IntConditionAttribute : PropertyAttribute
{
    public Vector2IntConditionAttribute()
    {
    }
}