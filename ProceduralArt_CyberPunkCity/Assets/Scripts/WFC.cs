using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WFC : MonoBehaviour
{
    public static WFC Instance;

    public SkyScraperGenerator houseGenerator;

    public int dimensions;
    public List<Tile> tileObjects;
    [SerializeField] List<Cell> gridComponents;
    public Cell cellObj;

    int iterations = 0;

    public Tile backupTile;

    GameObject parentOfCells;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    public List<Cell> GetGrid()
    {
        return gridComponents;
    }
    public void ResetGrid()
    {
        StopAllCoroutines();
        Destroy(parentOfCells);
        Destroy(SkyScraperGenerator.Instance.buildings);
        SkyScraperGenerator.Instance.allHouses = new List<SkyScraperGenerator.housePairs>();
        gridComponents = new List<Cell>();
        iterations = 0;
        InitializeGrid();
    }

    public Tile GetRandomTile(Cell collapsedCell)
    {
        float totalWeight = 0;
        foreach (var tile in collapsedCell.tileOptions)
        {
            totalWeight += tile.weight;
        }

        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        foreach (var tile in collapsedCell.tileOptions)
        {
            cumulativeWeight += tile.weight;
            if (randomValue <= cumulativeWeight)
            {
                return tile;
            }
        }

        // Fallback (should not happen)
        //Still happens with aprox 1/50 times with the weights as they are now. Sadly I can t find some tiles that would match the specific exceptions
        //in the SyntysStudious pack from which I took the tiles from.
        Debug.LogWarning($"Cell fallback at: X: {collapsedCell.positionInGrid.x} Z:{collapsedCell.positionInGrid.y}"+
            $"\nCheck the comment of this warning *insert big eyes hamster*");
        //StopAllCoroutines();
        return backupTile;
    }
    void InitializeGrid()
    {
        parentOfCells = new GameObject("Cells");
        for (int z = 0; z < dimensions; z++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector3(x * 5f, 0, z * 5f), Quaternion.identity, parentOfCells.transform);
                newCell.name = "Cell" + " " + z + " " + x;
                newCell.CreateCell(false, tileObjects, new Vector2Int(x, z));
                gridComponents.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }


    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);

        tempGrid.RemoveAll(c => c.collapsed);

        tempGrid.Sort((a, b) => { return a.tileOptions.Count - b.tileOptions.Count; });

        int arrLength = tempGrid[0].tileOptions.Count;
        int stopIndex = default;

        for (int i = 1; i < tempGrid.Count; i++)
        {
            if (tempGrid[i].tileOptions.Count > arrLength)
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
        if (tempGrid.Count > 0)
        {
            CollapseCell(tempGrid);
        }
    }

    void CollapseCell(List<Cell> tempGrid)
    {
        int index = 0;
        int minimumAmountOfOptions = 100000;
        //int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        for (int i = 0; i < tempGrid.Count; i++)
        {
            if(minimumAmountOfOptions > tempGrid[i].tileOptions.Count)
            {
                minimumAmountOfOptions = tempGrid[i].tileOptions.Count;
            }

            if (minimumAmountOfOptions == 1)
            {
                index = i;
                break;
            }
        }
        Cell cellToCollapse = tempGrid[index];

        cellToCollapse.collapsed = true;
        //Debug.Log(iterations);
        try
        {
            //Tile selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Count)];
            Tile selectedTile = GetRandomTile(cellToCollapse);
            
            cellToCollapse.tileOptions = new List<Tile>() { selectedTile };
            /*if(selectedTile.Equals(backupTile))
            {
                return;
            }*/
        }
        catch //no longer needed will move
        {
            Tile selectedTile = backupTile;
            cellToCollapse.tileOptions = new List<Tile>() { selectedTile };
            Debug.LogWarning($"Error at X:{cellToCollapse.positionInGrid.x} , Z:{cellToCollapse.positionInGrid.y}");
            return;
        }

        Tile foundTile = cellToCollapse.tileOptions[0];
        var generatedTile = Instantiate(foundTile.tilePrefab, cellToCollapse.transform.position, Quaternion.identity);
        generatedTile.transform.parent = cellToCollapse.transform;

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
                    //Debug.Log($"X: {x} Z: {z}");
                    List<Tile> options = new List<Tile>();
                    foreach (Tile t in tileObjects)
                    {
                        options.Add(t);
                    }
                    //update below
                    if (x < dimensions - 1)
                    {
                        Cell up = gridComponents[(x + 1) + z * dimensions];
                        if (up.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[(x + 1) + z * dimensions].tileOptions[0].downNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(x + 1) + z * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //update left
                    if (z > 0)
                    {
                        Cell right = gridComponents[(z - 1) * dimensions + x];
                        if (right.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[(z - 1) * dimensions + x].tileOptions[0].leftNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[z - 1 + x * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //update above
                    if (x > 0)
                    {
                        Cell down = gridComponents[(x - 1) + z * dimensions];
                        if (down.collapsed)
                        {

                            List<Tile> validOptions = gridComponents[(x - 1) + z * dimensions].tileOptions[0].upNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(x - 1) + z * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //Update based on tile to the right
                    if (z < dimensions - 1)
                    {
                        Cell left = gridComponents[(z + 1) * dimensions + x];
                        if (left.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[(z + 1) * dimensions + x].tileOptions[0].rightNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(z + 1) + x * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //Update based on tile to the top left
                    if (x > 0 && z > 0)
                    {
                        Cell bottomRight = gridComponents[(x - 1) + (z - 1) * dimensions];
                        if (bottomRight.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[(x - 1) + (z - 1) * dimensions].tileOptions[0].topLeftNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(z + 1) + x * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //Update based on tile to the top right
                    if(z < dimensions - 1 && x > 0)
                    {
                        Cell bottomLeft = gridComponents[(x - 1) + (z + 1) * dimensions];
                        if (bottomLeft.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[(x - 1) + (z + 1) * dimensions].tileOptions[0].topRightNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(z + 1) + x * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //Update based on tile to the bottom left
                    if(x < dimensions - 1 && z > 0)
                    {
                        Cell topRight = gridComponents[(z - 1) * dimensions + (x + 1)];
                        if (topRight.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[(z - 1) * dimensions + (x + 1)].tileOptions[0].bottomLeftNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(z + 1) + x * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    //Update based on tile to the bottom right
                    if (z < dimensions - 1 && x < dimensions - 1)
                    {
                        Cell bottomLeft = gridComponents[(x + 1) + (z + 1) * dimensions];
                        if (bottomLeft.collapsed)
                        {
                            List<Tile> validOptions = gridComponents[(x + 1) + (z + 1) * dimensions].tileOptions[0].bottomRightNeighbors.ToList();

                            CheckValidity(options, validOptions);
                            //newGenerationCell[(z + 1) + x * dimensions].RecreateCell(validOptions.ToArray());
                        }
                    }

                    List<Tile> newTileList = new List<Tile>();

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList.Add(options[i]);
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
        else
        {
            //TO COMMENT OUT AFTER TESTING
            //Restets the grid everytime after fully generating the tiles. Used to test and iterate the algorithm for the tiles :D
            //ResetGrid();

            SkyScraperGenerator.Instance.GenerateBuildings();
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
