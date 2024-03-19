using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WFC : MonoBehaviour
{
    public int dimensions;
    public Tile[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    int iterations = 0;

    public Tile backupTile;

    void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        GameObject parent = new GameObject("Cells");
        for (int z = 0; z < dimensions; z++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector3(x * 5f, 0, z * 5f), Quaternion.identity, parent.transform);
                newCell.name = "Cell" + " " + z + " " + x;
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }


    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        //tempGrid.Sort((a, b) => { return a.tileOptions.Length - b.tileOptions.Length; });

        int arrLength = tempGrid[0].tileOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].tileOptions.Length > arrLength)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
        {
            tempGrid.RemoveRange(stopIndex, tempGrid.Count - stopIndex);
        }

        yield return new WaitForSeconds(0.01f);

        CollapseCell(tempGrid);
    }

    void CollapseCell(List<Cell> tempGrid)
    {
        //int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[0];

        cellToCollapse.collapsed = true;
        //Debug.Log(iterations);
        try
        {
            Tile selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        }
        catch
        {
            Tile selectedTile = backupTile;
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        }

        Tile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile.tilePrefab, cellToCollapse.transform.position, Quaternion.identity);

        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);
        
        /**/
        for (int z = 0; z < dimensions; z++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                var index = x + z * dimensions;

                if (gridComponents[index].collapsed)
                {
                    newGenerationCell[index] = gridComponents[index];

                             
                }
                else
                {
                    List<Tile> options = new List<Tile>();
                    foreach (Tile t in tileObjects)
                    {
                        options.Add(t);
                    }
                    //update above
                    if (x < dimensions - 1)
                    {
                        Cell up = gridComponents[(x + 1) + z * dimensions];
                        if (up.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[(x + 1) + z * dimensions].tileOptions[0].upNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(x + 1) + z * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //update right
                    if (z > 0)
                    {
                        Cell right = gridComponents[z - 1 + x * dimensions];
                        if (right.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[z - 1 + x * dimensions].tileOptions[0].rightNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[z - 1 + x * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //update down
                    if (x > 0)
                    {
                        Cell down = gridComponents[(x - 1) + z * dimensions];
                        if (down.collapsed)
                        {

                            List<Tile> validOptions = gridComponents[(x - 1) + z * dimensions].tileOptions[0].downNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(x - 1) + z * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //update left
                    if (z < dimensions - 1)
                    {
                        Cell left = gridComponents[z + 1 + x * dimensions];
                        if (left.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[z + 1 + x * dimensions].tileOptions[0].leftNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(z + 1) + x * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    Tile[] newTileList = new Tile[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
                /**
                if (gridComponents[index].collapsed)
                {
                    Debug.Log("called");
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Tile> options = new List<Tile>();
                    foreach (Tile t in tileObjects)
                    {
                        options.Add(t);
                    }
                    //update above
                    if (x < dimensions - 1)
                    {
                        Cell up = gridComponents[(x + 1) + z * dimensions];
                        if (up.collapsed)
                        {
                            List<Tile> validOptions = new List<Tile>();

                            foreach (Tile possibleOptions in up.tileOptions)
                            {
                                var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                var valid = tileObjects[valOption].upNeighbors;

                                validOptions = validOptions.Concat(valid).ToList();
                            }

                            CheckValidity(options, validOptions);
                        }
                    }

                    //update right
                    if (z > 0)
                    {
                        Cell right = gridComponents[z - 1 + x * dimensions];
                        if (right.collapsed)
                        {
                            List<Tile> validOptions = new List<Tile>();

                            foreach (Tile possibleOptions in right.tileOptions)
                            {
                                var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                var valid = tileObjects[valOption].rightNeighbors;

                                validOptions = validOptions.Concat(valid).ToList();
                            }

                            CheckValidity(options, validOptions);
                        }
                    }

                    //look down
                    if (x > 0)
                    {
                        Cell down = gridComponents[(x - 1) + z * dimensions];
                        if (!down.collapsed)
                        {

                            List<Tile> validOptions = new List<Tile>();

                            foreach (Tile possibleOptions in down.tileOptions)
                            {
                                var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                var valid = tileObjects[valOption].downNeighbors;

                                validOptions = validOptions.Concat(valid).ToList();
                            }

                            CheckValidity(options, validOptions);
                        }
                    }

                    //look left
                    if (z < dimensions - 1)
                    {
                        Cell left = gridComponents[z + 1 + x * dimensions];
                        if (!left.collapsed)
                        {

                            List<Tile> validOptions = new List<Tile>();

                            foreach (Tile possibleOptions in left.tileOptions)
                            {
                                var valOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);
                                var valid = tileObjects[valOption].leftNeighbors;

                                validOptions = validOptions.Concat(valid).ToList();
                            }

                            CheckValidity(options, validOptions);
                        }
                    }

                    Tile[] newTileList = new Tile[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
                //*/
            }
        }
        //*/

       

        gridComponents = newGenerationCell;
        iterations++;

        if (iterations < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }

    }


    void CheckValidity(List<Tile> optionList, List<Tile> validOption)
    {
        for (int x = optionList.Count - 1; x >= 0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
