using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles", order = 1)]
public class Tile : ScriptableObject
{
    public GameObject tilePrefab;

    public Tile[] upNeighbors;
    public Tile[] leftNeighbors;
    public Tile[] downNeighbors;
    public Tile[] rightNeighbors;
}
