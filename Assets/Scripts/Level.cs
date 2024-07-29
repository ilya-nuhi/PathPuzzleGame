using UnityEngine;

public class Level : MonoBehaviour
{
    /// <summary>
    /// starting tile of the level and its starting point
    /// </summary>
    [SerializeField] public Tile startingTile;
    [SerializeField] public int startingPoint;
    [SerializeField] public int rowLength;
    [SerializeField] public int columnLength;
}