using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Connections")]
public class Connections : ScriptableObject
{
    public int meshRotation;
    public GameObject prefab;

    [Header("The type of the current building block")]
    public CellType cellType = CellType.NULL;

    [Header("Possible connections")]
    [Tooltip("Possible cell type connections for positive X")]
    public CellType[] posX;

    [Tooltip("Possible cell type connections for Negative X")]
    public CellType[] negX;

    [Tooltip("Possible cell type connections for positive Z")]
    public CellType[] posZ;

    [Tooltip("Possible cell type connections for negative Z")]
    public CellType[] negZ;
    //public List<Attribute> attributes = new List<Attribute>();
    //public NeighbourList validNeighbours;

}
[System.Serializable]
public class NeighbourList
{
    public List<Connections> posX = new List<Connections>();
    public List<Connections> posZ = new List<Connections>();
    public List<Connections> negX = new List<Connections>();
    public List<Connections> negZ = new List<Connections>();

}
public enum CellType { PAVEMENT, ROAD, CURB, BUILDING, NULL}
public enum WFC_Socket { Socket_Road, Socket_Curb, Socket_Buildings_Pos, Socket_Buildings_Neg, Socket_FullBuildings, Socket_Pavement }

[System.Serializable]
public class SocketConnection
{
    public WFC_Socket socket;
    public List<WFC_Socket> validConnections;
}