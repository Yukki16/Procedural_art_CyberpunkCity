using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProps : MonoBehaviour
{
    public List<GameObject> spawnList;

    int randomNuberOfProps = 0;
    private void Awake()
    {
        randomNuberOfProps = UnityEngine.Random.Range(0, spawnList.Count + 1);

        List<GameObject> copy = spawnList;

        while(randomNuberOfProps > 0)
        {
            randomNuberOfProps--;
            copy[Random.Range(0, copy.Count)].SetActive(true);
        }
    }
}
