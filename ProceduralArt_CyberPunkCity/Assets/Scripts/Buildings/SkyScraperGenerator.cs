using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyScraperGenerator : MonoBehaviour
{
    public static SkyScraperGenerator Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [HideInInspector]
    public GameObject buildings;

    [Serializable]
    public struct housePairs
    {
        public List<Cell> pairOfHouses;
    }

    public List<housePairs> allHouses = new List<housePairs>();

    [SerializeField] Tile houseTile;

    [SerializeField] GameObject housePrefab;

    public void GenerateBuildings()
    {
        List<Cell> cellList = WFC.Instance.GetGrid();

        FindHousePairs(cellList);

        CreateBuidlingsBasedOnPairs();
    }

    public static (float width, float height) GetDimensions(List<Cell> tiles)
    {
        if (tiles == null || tiles.Count == 0)
        {
            throw new System.ArgumentException("The list of tiles must not be null or empty");
        }

        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (var tile in tiles)
        {
            if (tile.positionInGrid.x < minX) minX = tile.positionInGrid.x;
            if (tile.positionInGrid.x > maxX) maxX = tile.positionInGrid.x;
            if (tile.positionInGrid.y < minY) minY = tile.positionInGrid.y;
            if (tile.positionInGrid.y > maxY) maxY = tile.positionInGrid.y;
        }

        float width = maxX - minX + 1;
        float height = maxY - minY + 1;

        return (width, height);
    }
    void CreateBuidlingsBasedOnPairs()
    {
        buildings = new GameObject("Buildings");
        foreach(housePairs pair in allHouses)
        {
            float x = 0;
            float y = 0;
            foreach(var _house in pair.pairOfHouses)
            {
                x += _house.positionInGrid.x;
                y += _house.positionInGrid.y;
            }

            x /= pair.pairOfHouses.Count;
            y /= pair.pairOfHouses.Count;

            GameObject house = Instantiate(housePrefab, new Vector3(pair.pairOfHouses[pair.pairOfHouses.Count - 1].positionInGrid.x * 5f, 0, pair.pairOfHouses[pair.pairOfHouses.Count - 1].positionInGrid.y * 5f), this.transform.rotation, buildings.transform);

            (float width, float height) dimensions = GetDimensions(pair.pairOfHouses);
            house.transform.localScale = new Vector3(dimensions.width, 1, dimensions.height);

            house.GetComponent<SkyScraper>().StartBuilding(house.transform.localScale);
        }
    }
    /// <summary>
    /// VERY resource intensive, I wish I had a better idea 
    /// Based on ChatGPT calculation of the dificulty it got to O(n^8) ;-;  (Added 2 more checks since I wrote this so might be O(n^12) )
    /// I will try the optimisation with the HashSet of which GPT is giving as an option, but I never worked with them so I might opt out if I don't have enough time.
    /// </summary>
    /// <param name="grid"></param>
    public void FindHousePairs(List<Cell> grid) 
    {

        housePairs newHousePair = new housePairs();
        for (int x = 0; x < WFC.Instance.dimensions; x++) //Loop through the grid
        {
            for (int y = 0; y < WFC.Instance.dimensions; y++)
            {
                if (!grid[x * WFC.Instance.dimensions + y].tileOptions[0].Equals(houseTile))  //Not a house? Continue
                {
                    continue;
                }

                bool foundAlready = false;
                foreach (var pair in allHouses) //Another check to not create pairs of houses already in the pairs.
                {
                    if (pair.pairOfHouses.Contains(grid[x * WFC.Instance.dimensions + y]))
                    {
                        foundAlready = true;
                        break;
                    }
                }
                if (!foundAlready)
                {
                    newHousePair.pairOfHouses = new List<Cell>();
                    newHousePair.pairOfHouses.Add(grid[x * WFC.Instance.dimensions + y]); //Single house cell case

                    for (int z = x; z < WFC.Instance.dimensions; z++)  //Found a house and starts searching vertically
                    {
                        bool justQuit = false;
                        int maxW = 0;
                        if (z >= x + 1) //This is to check if it should continue to the left, if there are any houses there
                                        // This is an early check to break faster from the loop
                        {
                            if (!grid[z * WFC.Instance.dimensions + y].tileOptions[0].Equals(houseTile))
                            {
                                break;
                            }
                        }

                        bool enteredAnotherBuildingOnZ = false;
                        foreach (var pair in allHouses) //Another check to not create pairs of houses already in the pairs.
                        {
                            if (pair.pairOfHouses.Contains(grid[z * WFC.Instance.dimensions + y]))
                            {
                                enteredAnotherBuildingOnZ = true;
                                break;
                            }
                        }

                        if (enteredAnotherBuildingOnZ)
                        {
                            break;
                        }

                        newHousePair.pairOfHouses.Add(grid[z * WFC.Instance.dimensions + y]); //Cells on the z but not on w case

                        for (int w = y + 1; w < WFC.Instance.dimensions; w++)
                        {
                            if (!grid[x * WFC.Instance.dimensions + w].tileOptions[0].Equals(houseTile)) //Check if on the original line there is still a house on the same row
                                                                                                         //Otherwise just break
                            {
                                break;
                            }

                            if (!grid[z * WFC.Instance.dimensions + w].tileOptions[0].Equals(houseTile)) //Current cell not a house? end the search vertically
                            {
                                justQuit = true; //bool to break out of z loop since there is ground in between
                                maxW = w;
                                break;
                            }

                            bool enteredAnotherBuilding = false;
                            foreach (var pair in allHouses) //Another check to not create pairs of houses already in the pairs.
                            {
                                if (pair.pairOfHouses.Contains(grid[z * WFC.Instance.dimensions + w]))
                                {
                                    enteredAnotherBuilding = true;
                                    break;
                                }
                            }
                            if (enteredAnotherBuilding)
                            {
                                break;
                            }
                            //Didn't break from any of them? Then it is a house and should recreate the pair

                            newHousePair.pairOfHouses = new List<Cell>();

                            for (int i = x; i <= z; i++)
                            {
                                for (int j = y; j <= w; j++)
                                {
                                    newHousePair.pairOfHouses.Add(grid[i * WFC.Instance.dimensions + j]);
                                }
                            }
                        }

                        if(justQuit)
                        {
                            if(maxW == y + 1)
                                newHousePair.pairOfHouses.Remove(grid[z * WFC.Instance.dimensions + y]);
                            break;
                        }
                    }
                    if (newHousePair.pairOfHouses != null && newHousePair.pairOfHouses.Count > 0)
                    {
                        allHouses.Add(newHousePair);
                    }
                }
            }
        }

    }
}
