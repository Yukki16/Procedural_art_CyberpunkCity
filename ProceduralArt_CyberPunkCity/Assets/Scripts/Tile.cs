using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles", order = 1)]
[System.Serializable]
public class Tile : ScriptableObject
{
    public GameObject tilePrefab;

    public float weight = 1;

    public Tile[] upNeighbors;
    public Tile[] downNeighbors;
    public Tile[] leftNeighbors;
    public Tile[] rightNeighbors;

    public Tile[] topLeftNeighbors;
    public Tile[] topRightNeighbors;
    public Tile[] bottomLeftNeighbors;
    public Tile[] bottomRightNeighbors;
}
