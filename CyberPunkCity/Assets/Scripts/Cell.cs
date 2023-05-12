using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2 gridPoz = new Vector2();
    public bool colaplsed = false;

    public List<Connections> possibleConnections = new List<Connections>();
}
