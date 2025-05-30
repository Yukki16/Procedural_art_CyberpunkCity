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

    int justARandomNumberToSeeIfIShouldGenerateASign = 1;

    [SerializeField] GameObject signSpawned;
    public void SpawnSign(Vector3 scale)
    {
        if (!justARandomNumberToSeeIfIShouldGenerateASign.Equals(UnityEngine.Random.Range(0, 5)))
        {
            return;
        }
        GameObject signToSpawn = ReturnARandomSign();
        var spawnedSign = Instantiate(signToSpawn, this.transform.position, this.transform.rotation * signToSpawn.transform.rotation, this.transform);

        Vector3 adjustedLocalScale = GetAdjustedLocalScale(signToSpawn.transform, scale);

        // Set the parent of the spawned sign to the current object while retaining the world position
        //spawnedSign.transform.SetParent(this.transform, worldPositionStays: true);

        // Apply the adjusted local scale to the sign
        spawnedSign.transform.localScale = adjustedLocalScale;
        //spawnedSign.transform.localScale = scale;

        signSpawned = spawnedSign;
    }
    /*void OnDrawGizmos()
    {
        if (this.transform.rotation.eulerAngles.y % 180 == 0)

            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.transform.position, 1);
    }*/ // used to debug the rotation of the sign
    //This thing ate me so much time due to uneven scalings. I thought I would be done in 1h at most, it took me 3h and another person to figure out why the signs were
    //scaling in so many weird ways. I am so done with this one.
    Vector3 GetAdjustedLocalScale(Transform childTransform, Vector3 scale)
    {
        // Get the local scale of the parent
        //Vector3 parentScale = parentTransform.localScale;

        Vector3 adjustedLocalScale = new Vector3();
        // Calculate the adjusted local scale to retain the original world scale
        if (this.transform.rotation.eulerAngles.y % 180 == 0)
        {
           //Debug.Log($"{childTransform.transform.position} entered 0/180");
            adjustedLocalScale = new Vector3(
                childTransform.localScale.x / scale.x,
                childTransform.localScale.y / scale.y,
                childTransform.localScale.z / scale.z
            );

        }
        else if(this.transform.rotation.eulerAngles.y % 90 == 0) //Case for 90 and 270 deg
        {
            //Debug.Log($"{childTransform.transform.position} entered 90/270");
            adjustedLocalScale = new Vector3(
                childTransform.localScale.x / scale.z,
                childTransform.localScale.y / scale.y,
                childTransform.localScale.z / scale.x
            );
        }
        /*else if(this.transform.rotation.eulerAngles.y == 135)
        {
            adjustedLocalScale = new Vector3(
                childTransform.localScale.x / scale.z,
                childTransform.localScale.y / scale.y,
                childTransform.localScale.z / scale.x
            );
        }*/
        else //I have some signs that are on 45 deg so this is also needed, order of execution matters :p
        {
            //Debug.Log($"{childTransform.transform.localPosition} entered 45");
            adjustedLocalScale = new Vector3(
                childTransform.localScale.x / (scale.x > scale.z? scale.x: scale.z),
                childTransform.localScale.y / scale.y,
                childTransform.localScale.z
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
