using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Connections")]
public class Connections : ScriptableObject
{
    public int meshRotation;
    public GameObject prefab;

    [Header("Mesh connections")]

    public int positiveX;
    public int positiveZ;
    public int negativeX;
    public int negativeZ;

    //The higher the connection the more things you can connect to it
}