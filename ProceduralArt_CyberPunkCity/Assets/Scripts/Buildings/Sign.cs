using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [Serializable]
    struct sign
    {
        public GameObject signPrefab;
        [Min(0f)] public float weight;
    }

    [SerializeField] List<sign> signList;

    public void SpawnSign(Vector3 scale)
    {
        GameObject signToSpawn = ReturnARandomSign();

        var spawnedSign = Instantiate(signToSpawn, this.transform.position, this.transform.rotation, this.transform);

        Vector3 adjustedLocalScale = GetAdjustedLocalScale(signToSpawn.transform, scale);

        // Set the parent of the spawned sign to the current object while retaining the world position
        //spawnedSign.transform.SetParent(this.transform, worldPositionStays: true);

        // Apply the adjusted local scale to the sign
        spawnedSign.transform.localScale = adjustedLocalScale;
        //spawnedSign.transform.localScale = scale;
    }
    void OnDrawGizmos()
    {
        if (this.transform.rotation.eulerAngles.y % 180 == 0)

            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.transform.position, 1);
    }
    Vector3 GetAdjustedLocalScale(Transform childTransform, Vector3 scale)
    {
        // Get the local scale of the parent
        //Vector3 parentScale = parentTransform.localScale;

        Vector3 adjustedLocalScale = new Vector3();
        // Calculate the adjusted local scale to retain the original world scale
        if (this.transform.rotation.eulerAngles.y % 180 == 0)
        {
            Debug.Log($"{childTransform.transform.position} entered 0/180");
            adjustedLocalScale = new Vector3(
                childTransform.localScale.x / scale.x,
                childTransform.localScale.y / scale.y,
                childTransform.localScale.z / scale.z
            );

        }
        else
        {
            Debug.Log($"{childTransform.transform.position} entered 90/270");
            adjustedLocalScale = new Vector3(
                childTransform.localScale.x / scale.z,
                childTransform.localScale.y / scale.y,
                childTransform.localScale.z / scale.x
            );
        }

        return adjustedLocalScale;
    }

    GameObject ReturnARandomSign()
    {
        float totalWeight = 0;
        foreach (var sign in signList)
        {
            totalWeight += sign.weight;
        }

        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        foreach (var sign in signList)
        {
            cumulativeWeight += sign.weight;
            if (randomValue <= cumulativeWeight)
            {
                return sign.signPrefab;
            }
        }

        return new GameObject("I should not exist. Wtf?");
    }
}
