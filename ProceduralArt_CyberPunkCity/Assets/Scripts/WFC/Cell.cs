using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public List<Tile> tileOptions;
    public Vector2Int positionInGrid;

    public void CreateCell(bool collapseState, List<Tile> tiles, Vector2Int _positionInGrid)
    {
        collapsed = collapseState;
        tileOptions = tiles;
        positionInGrid = _positionInGrid;
    }

    public void RecreateCell(List<Tile> tiles)
    {
        tileOptions = tiles;
    }
}
