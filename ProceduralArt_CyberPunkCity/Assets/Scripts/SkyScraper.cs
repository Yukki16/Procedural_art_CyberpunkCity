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

    private void Awake()
    {
        randomBuildType = UnityEngine.Random.Range(0, buildingTypes.Count);
        buildingHeight = UnityEngine.Random.Range(10, 25);

        GenerateBuilding();
    }

    private void GenerateBuilding()
    {
        Instantiate(buildingTypes[randomBuildType].buildingBase, this.transform.position, this.transform.rotation, this.transform); //the base

        for (int i = 1; i <= buildingHeight; i++)
        {
            Instantiate(buildingTypes[randomBuildType].buildingPart, this.transform.position + new Vector3(0, buildingTypes[randomBuildType].distanceBetweenFloors * i, 0),
                this.transform.rotation, this.transform); //just a floor type in the assets from SyntysStudios pack
        }

        Instantiate(buildingTypes[randomBuildType].roof, this.transform.position + new Vector3(0, buildingTypes[randomBuildType].distanceBetweenFloors * (buildingHeight + 1), 0),
            this.transform.rotation, this.transform); //the roof
    }
}