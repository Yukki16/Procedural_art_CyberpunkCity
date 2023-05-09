using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2 gridPoz = new Vector2();
    public bool colaplsed = false;

    public Connections connection = new Connections();

    public CellType cellType = CellType.NULL;

    public Connections[] availableTransformations;

    public void Collapse()
    {
        colaplsed = true;
        int randomchance = Random.Range(0, availableTransformations.Length);

        connection = availableTransformations[randomchance];
        cellType = connection.cellType;
    }


}
