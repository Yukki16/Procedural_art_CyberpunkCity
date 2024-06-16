using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyScraper : MonoBehaviour
{
    [Serializable]
    struct building
    {
        public string name;
        public GameObject buildingBase;
        public GameObject buildingPart;
        public GameObject roof;
        public float distanceBetweenFloors;
    }

    [SerializeField] List<building> buildingTypes;

    int randomBuildType = 0;
    [SerializeField] int buildingHeight;

    
    [Vector2IntCondition][SerializeField] Vector2Int buildingHeightRandomParams;

    List<Sign> signsToSpawn = new List<Sign>();

    List<GameObject> buildParts = new List<GameObject>();
    Vector3 houseScale;

    public void StartBuilding(Vector3 scale)
    {
        randomBuildType = UnityEngine.Random.Range(0, buildingTypes.Count);
        buildingHeight = UnityEngine.Random.Range(buildingHeightRandomParams.x, buildingHeightRandomParams.y);

        houseScale = scale;
        GenerateBuilding(scale);
    }

    private void GenerateBuilding(Vector3 scale)
    {
        buildParts.Add(Instantiate(buildingTypes[randomBuildType].buildingBase, this.transform.position, this.transform.rotation, this.transform)); //the base

        for (int i = 1; i <= buildingHeight; i++)
        {
            buildParts.Add(Instantiate(buildingTypes[randomBuildType].buildingPart, this.transform.position + new Vector3(0, buildingTypes[randomBuildType].distanceBetweenFloors * i, 0),
                this.transform.rotation, this.transform)); //just a floor type in the assets from SyntysStudios pack
        }

        buildParts.Add(Instantiate(buildingTypes[randomBuildType].roof, this.transform.position + new Vector3(0, buildingTypes[randomBuildType].distanceBetweenFloors * (buildingHeight + 1), 0),
            this.transform.rotation, this.transform)); //the roof

        signsToSpawn.AddRange(this.GetComponentsInChildren<Sign>());
        foreach (Sign sign in signsToSpawn)
        {
            sign.SpawnSign(scale);
        }
    }

    public void RedoBuilding()
    {
        for (int i = buildParts.Count - 1; i >= 0 ; i--)
        {
            Destroy(buildParts[i]);
        }

        buildParts.Clear();
        signsToSpawn.Clear();
        GenerateBuilding(houseScale);
    }
}
