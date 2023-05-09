using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    [SerializeField] Vector2 gridSize = new Vector2(10,10);

    [Header("X grid size")]
    [SerializeField] int xSize = 10;

    [Header("Y grid size")]
    [SerializeField] int ySize = 10;

    [SerializeField] Cell[,] cells;
    //GRID SIZE;

    public void Generate()
    {
        cells = new Cell[(int)gridSize.x, (int)gridSize.y];
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                Cell cell = new Cell();
                cell.gridPoz = new Vector2(i,j);
                cells[i,j] = cell;
            }
        }
        //bool allCellsCollapsed = false;

        List<Cell> uncollapsedCells = new List<Cell>();
        do
        {
            uncollapsedCells.Clear();
            foreach(Cell cell in cells)
            {
                if(cell.colaplsed)
                {
                    continue;
                }
                else
                {
                    uncollapsedCells.Add(cell);
                }
            }

            UpdatePossibleTransforms(uncollapsedCells);

        }while(uncollapsedCells.Count > 0);
    }

    private void UpdatePossibleTransforms(List<Cell> uncollapsed)
    {
        foreach(Cell cell in uncollapsed)
        {
            //Checkleft
            if(cell.gridPoz.x >= 1)
            {
                if(cells[(int)(cell.gridPoz.x - 1), (int)(cell.gridPoz.y)].colaplsed)
                {
                    //foreach(var connection in cell.)
                }
            }
        }
    }
    //iterate throgh all cells,
    //find the one with least amount of possible connections
    //make it choose a form
    //iterate through all to update the possible connecitons again
}
